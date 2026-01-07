using System;

using CoopGame.Server.Core.Math.Noise;
using CoopGame.Server.World;

using CoopGame.Shared.World.Terrain;

namespace CoopGame.Server.Core.Generation;

public static class ChunkFactory {
    public static Chunk generateChunk(int chunkX, int chunkY, int chunkSize, int seed) {
        Chunk chunk = new(chunkX, chunkY, chunkSize);

        PerlinNoise2D perlinNoise = new(seed);
        FractalNoise2D fractalNoise = new(perlinNoise, octaves: 4, persistence: 0.45f, lacunarity: 2f);

        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                // Global coordinates
                float globalX = chunkX * chunkSize + x;
                float globalY = chunkY * chunkSize + y;

                float noiseValue = fractalNoise.noise(globalX, globalY);

                if (noiseValue < 0) {
                    noiseValue = -noiseValue;
                }

                chunk.tiles[x, y].pollutionLevel = 0f;

                chunk.tiles[x, y].terrainType = determineTerrain(noiseValue);
            }
        }

        return chunk;
    }

    // Assign Terrain Based on Final Noise
    private static TerrainType determineTerrain(float noiseValue) {
        if (noiseValue < 0.040f)
            return TerrainType.Water;
        if (noiseValue < 0.065f)
            return TerrainType.Sand;
        if (noiseValue < 0.200f)
            return TerrainType.Grass;
        if (noiseValue < 0.230f)
            return TerrainType.Dirt;
        if (noiseValue < 0.350f)
            return TerrainType.Rock;
        if (noiseValue >= 0.350f)
            return TerrainType.Snow;

        return TerrainType.Snow;
    }
}
