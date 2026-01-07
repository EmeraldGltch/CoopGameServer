using System;
using System.Collections.Generic;
using System.Linq;

using CoopGame.Server.Core.Generation;
using CoopGame.Server.World;

namespace CoopGame.Server.Simulation;
public class SimulationScheduler {
    private readonly ChunkManager chunkManager;
    private readonly List<Player> players;
    private readonly int maxChunksPerTick;
    private readonly int maxCatchUpTicksPerTick;

    private readonly SimulationDebugger debugger = new();

    public SimulationScheduler(ChunkManager chunkManager, List<Player> players, int maxChunksPerTick = 50, int maxCatchUpTicksPerTick = 6) {
        this.chunkManager = chunkManager;
        this.players = players;
        this.maxChunksPerTick = maxChunksPerTick;
        this.maxCatchUpTicksPerTick = maxCatchUpTicksPerTick;
    }

    private IEnumerable<Chunk> prioritizeChunks(IEnumerable<Chunk> chunks) {
        return chunks.OrderByDescending(c => calculateChunkPriority(c));
    }

    private int calculateChunkPriority(Chunk chunk) {
        int minDistance = int.MaxValue;

        foreach (var player in players) {
            int dx = chunk.chunkX - player.chunkX;
            int dy = chunk.chunkY - player.chunkY;
            int distance = Math.Abs(dx) + Math.Abs(dy);

            if (distance < minDistance)
                minDistance = distance;
        }

        return -minDistance; // Makes closer chunks have higher priority
    }

    public void tick(double delta) {
        // Update simulated chunks near players
        chunkManager.updateChunks(players, simulationRadius: 4);

        var allChunks = chunkManager.getAllSimulatedChunks();
        var prioritizedChunks = prioritizeChunks(allChunks).Take(maxChunksPerTick);

        HashSet<Chunk> alreadySimulated = [];

        foreach (var chunk in prioritizedChunks) {
            int ticksToProcess = Math.Min(chunk.pendingTicks, maxCatchUpTicksPerTick);

            // Catch-up Simulation
            for (int i = 0; i < ticksToProcess; i++) {
                chunkManager.simulator.simulateChunk(chunk, delta);
                chunk.onSimulated(isCatchUp: true);
                chunk.pendingTicks--;
            }

            // Up-to-date ticks
            if (!alreadySimulated.Contains(chunk)) {
                chunkManager.simulator.simulateChunk(chunk, delta);
                chunk.onSimulated(isCatchUp: false);
                alreadySimulated.Add(chunk);
            }
        }
    }
}
