using System;

using CoopGame.Server.Core.Math.Noise;
using CoopGame.Server.World;

using CoopGame.Shared.World.Terrain;
using CoopGame.Server.Core.Math.Noise.Specialized;

namespace CoopGame.Server.Core.Generation;

public static class ChunkFactory {
    public static Chunk generateChunk(int chunkX, int chunkY, int chunkSize, int seed) {
        Chunk chunk = new(chunkX, chunkY, chunkSize);

		PerlinNoise2D perlinNoise = new(seed);
        FractalNoise2D fractalNoise = new(perlinNoise);
        RidgedNoise2D ridgedNoise = new(fractalNoise);
        SparseNoise2D sparseNoise = new(ridgedNoise);

        var elevationNoise = new FractalNoise2D(new PerlinNoise2D(seed), octaves: 4, persistence: 0.45f, lacunarity: 2.2f);
        var moistureNoise = new FractalNoise2D(new PerlinNoise2D(seed + 1), octaves: 7, persistence: 0.4f, lacunarity: 12f);
        var temperatureNoise = new FractalNoise2D(new PerlinNoise2D(seed + 2), octaves: 3, persistence: 0.5f, lacunarity: 10f);

		TerrainNoiseSampler noiseSampler = new TerrainNoiseSampler(elevationNoise, moistureNoise, temperatureNoise);
		TerrainSample[,] samples = new TerrainSample[chunkSize, chunkSize];

		for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                // Global coordinates
                float globalX = chunkX * chunkSize + x;
                float globalY = chunkY * chunkSize + y;
                
                samples[x, y].elevation = MathF.Abs(Clamp(elevationNoise.noise(globalX, globalY), 0f, 1f));
				samples[x, y].moisture = MathF.Abs(Clamp(moistureNoise.noise(globalX, globalY), 0f, 1f));
                samples[x, y].temperature = MathF.Abs(Clamp(temperatureNoise.noise(globalX, globalY), 0.25f, 1f));

                TerrainSample sample = samples[x, y];

				chunk.tiles[x, y].terrainType = TerrainNoiseSampler.determineBiome(sample);
                chunk.tiles[x, y].elevation = sample.elevation;
			}
        }

        return chunk;
    }

    // Helper
    private static float Clamp(float value, float min, float max) {
        if(value < min) return min;
        if(value > max) return max;
        return value;
	}
}
