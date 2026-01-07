using System;
using System.Collections.Generic;

using CoopGame.Server.Simulation.Simulator;
using CoopGame.Server.World;

namespace CoopGame.Server.Core.Generation;

public class ChunkManager {
    private readonly int chunkSize;
    private readonly int seed;

    private readonly Dictionary<(int, int), Chunk> chunks = [];

    private readonly HashSet<Chunk> simulatedChunks = [];

    public ChunkSimulator simulator;

    public ChunkManager(int chunkSize = 16, int seed = 0) {
        this.chunkSize = chunkSize;
        this.seed = seed;

        simulator = new ChunkSimulator();
    }

    // Returns a chunk at the specified coordinates / Orders the creation of chunks if they're missing
    public Chunk getChunk(int chunkX, int chunkY) {
        var key = (chunkX, chunkY);

        if (!chunks.TryGetValue(key, out var chunk)) {
            chunk = ChunkFactory.generateChunk(chunkX, chunkY, chunkSize, seed);
            chunks[key] = chunk;
        }

        return chunk;
    }

    public IEnumerable<Chunk> getAllSimulatedChunks() {
        foreach (var chunk in simulatedChunks) {
            if (chunk.state == ChunkState.Simulated || chunk.state == ChunkState.Rendered) {
                yield return chunk;
            }
        }
    }

    // Determine which chunks need to be loaded near players
    public void updateChunks(IEnumerable<Player> players, int simulationRadius) {
        simulatedChunks.Clear();

        foreach (var player in players) {
            for (int dx = -simulationRadius; dx <= simulationRadius; dx++) {
                for (int dy = -simulationRadius; dy <= simulationRadius; dy++) {
                    int cx = player.chunkX + dx;
                    int cy = player.chunkY + dy;

                    Chunk chunk = getChunk(cx, cy);
                    simulatedChunks.Add(chunk);

                    if (chunk.state == ChunkState.Dormant) {
                        chunk.state = ChunkState.Simulated;
                    }
                }
            }
        }
    }

    // Come on man, it's in the name
    public void unloadChunks(HashSet<Chunk> keepChunks) {
        List<(int, int)> keysToRemove = [];

        foreach (var kvp in chunks) {
            if (!keepChunks.Contains(kvp.Value)) {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove) {
            chunks.Remove(key);
        }
    }

    public IEnumerable<Chunk> getAllLoadedChunks() => chunks.Values;
}
