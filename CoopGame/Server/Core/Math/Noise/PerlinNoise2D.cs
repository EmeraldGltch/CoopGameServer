using System;

namespace CoopGame.Server.Core.Math.Noise;

public class PerlinNoise2D : INoise2D {
    private readonly int seed;

    public PerlinNoise2D(int seed) {
        this.seed = seed;
    }

    public float noise(float x, float y) {
        // Grid coordinates
        int x0 = fastFloor(x);
        int y0 = fastFloor(y);
        int x1 = x0 + 1;
        int y1 = y0 + 1;

        // Local coordinates
        float sx = x - x0;
        float sy = y - y0;

        // Corner gradients
        var g00 = gradient(x0, y0);
        var g10 = gradient(x1, y0);
        var g01 = gradient(x0, y1);
        var g11 = gradient(x1, y1);

        // Distance
        float dx0 = sx;
        float dy0 = sy;
        float dx1 = sx - 1f;
        float dy1 = sy - 1f;

        // Dot Product
        float n00 = g00.x * dx0 + g00.y * dy0;
        float n10 = g10.x * dx1 + g10.y * dy0;
        float n01 = g01.x * dx0 + g01.y * dy1;
        float n11 = g11.x * dx1 + g11.y * dy1;

        // Smooth interpolating
        float u = fade(sx);
        float v = fade(sy);

        float nx0 = lerp(n00, n10, u);
        float nx1 = lerp(n01, n11, u);

        return lerp(nx0, nx1, v);
    }

    //////////////////////
    // Helper Functions //
    //////////////////////

    private static int fastFloor(float f) {
        return f >= 0 ? (int)f : (int)f - 1;
    }

    private static float fade(float t) {
        // 6t^5 - 15t^4 + 10t^3
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    private static float lerp(float a, float b, float t) {
        return a + t * (b - a);
    }

    private (float x, float y) gradient(int x, int y) {
        int h = hash(x, y);

        // Gradient Directions
        switch (h & 7) {
            case 0:
                return (1f, 1f);
            case 1:
                return (-1f, 1f);
            case 2:
                return (1f, -1f);
            case 3:
                return (-1f, -1f);
            case 4:
                return (1f, 0f);
            case 5:
                return (-1f, 0f);
            case 6:
                return (0f, 1f);
            default:
                return (0f, -1f);
        }
    }

    private int hash(int x, int y) {
        unchecked {
            int h = seed;

            h ^= x * 374761393;
            h ^= y * 668265263;
            h = (h ^ (h >> 13)) * 1274126177;

            return h;
        }
    }
}
