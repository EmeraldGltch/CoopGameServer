using System;
using System.Collections.Generic;

using CoopGame.Server.Core.Dirty;
using CoopGame.Server.World;

namespace CoopGame.Server.Simulation.Simulator;

public class ChunkSimulator {
    private readonly float pollutionDecayRate;
    private readonly float diffusionRate;

    public ChunkSimulator(float pollutionDecayRate = 0f, float diffusionRate = 0.005f) {
        this.pollutionDecayRate = pollutionDecayRate;
        this.diffusionRate = diffusionRate;
    }

    // Simulate a given chunk for one tick
    public void simulateChunk(Chunk chunk, double deltaTime) {
        if (chunk == null)
            return;

        //////////////////////////////////////
        // All per-chunk mechanics go here! //
        //////////////////////////////////////
        
        simulatePollution(chunk);
        //simulateWeather(chunk, deltaTime);
        //simulateCrops(chunk, deltaTime);

        chunk.lastSimulatedTime = DateTime.UtcNow;
    }

    private void simulatePollution(Chunk chunk) {
        Tile[,] tiles = chunk.tiles;
        int size = tiles.GetLength(0);

        // Only simulate marked tiles
        var dirtyTiles = chunk.getDirtyTiles(TileDirtyReason.PollutionChanged);

        if (dirtyTiles.Count == 0)
            return; // Skip simulation

        int[,] offsets = {
            {0, -1}, {0, 1}, {-1, 0}, {1, 0},
            {-1, -1}, {1, -1}, {-1, 1}, {1, 1}
        };

        float diagonalWeight = 0.7071f;

        Dictionary<(int x, int y), float> deltaBuffer = new();

        foreach (var (x, y) in dirtyTiles) {
            float current = tiles[x, y].simulationPollutionLevel;

            for (int i = 0; i < offsets.GetLength(0); i++) {
                int nx = x + offsets[i, 0];
                int ny = y + offsets[i, 1];

                if (nx < 0 || nx >= size || ny < 0 || ny >= size)
                    continue; // skips out-of-bounds

                bool isDiagonal = Math.Abs(offsets[i, 0]) + Math.Abs(offsets[i, 1]) == 2;
                float weight = isDiagonal ? diagonalWeight : 1f;
                float flow = (tiles[nx, ny].simulationPollutionLevel - current) * diffusionRate * weight;

                if (!deltaBuffer.ContainsKey((x, y)))
                    deltaBuffer[(x, y)] = 0f;
                deltaBuffer[(x, y)] += flow;

                if (!deltaBuffer.ContainsKey((nx, ny)))
                    deltaBuffer[(nx, ny)] = 0f;
                deltaBuffer[(nx, ny)] -= flow;
            }
        }

        // Apply the deltaBuffer
        foreach (var kvp in deltaBuffer) {
            int x = kvp.Key.x;
            int y = kvp.Key.y;

            tiles[x, y].simulationPollutionLevel += kvp.Value;
            tiles[x, y].simulationPollutionLevel -= tiles[x, y].simulationPollutionLevel * pollutionDecayRate;
            tiles[x, y].syncPollution();

            // Clear the marker from the tile
            chunk.clearDirty(TileDirtyReason.PollutionChanged);
        }
    }
}
