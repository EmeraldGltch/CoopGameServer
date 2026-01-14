using CoopGame.Server.Core.Math.Noise;

namespace CoopGame.Server.Core.Math.Noise.Specialized;

public class RidgedNoise2D : INoise2D {
	private readonly INoise2D baseNoise;

	public RidgedNoise2D(INoise2D baseNoise) {
		this.baseNoise = baseNoise;
	}

	public float noise(float x, float y) {
		float value = baseNoise.noise(x, y);
		return 1f - MathF.Abs(value);
	}
}
