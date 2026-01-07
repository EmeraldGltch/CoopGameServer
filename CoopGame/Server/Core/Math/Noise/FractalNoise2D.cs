using System;

namespace CoopGame.Server.Core.Math.Noise;

public class FractalNoise2D : INoise2D {
    private readonly INoise2D baseNoise;
    private readonly int octaves;
    private readonly float persistence;
    private readonly float lacunarity;

    public FractalNoise2D(INoise2D baseNoise, int octaves = 4, float persistence = 0.5f, float lacunarity = 2f) {
        this.baseNoise = baseNoise ?? throw new ArgumentNullException(nameof(baseNoise));
        this.octaves = octaves;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
    }

    public float noise(float x, float y) {
        float total = 0f;
        float amplitude = 1f;
        float frequency = 0.005f;
        float maxValue = 0f;

        for (int i = 0; i < octaves; i++) {
            total += baseNoise.noise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        // Normalize
        return total / maxValue;
    }
}
