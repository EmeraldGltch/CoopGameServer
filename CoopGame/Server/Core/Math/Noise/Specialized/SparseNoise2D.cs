using CoopGame.Server.Core.Math.Noise;

namespace CoopGame.Server.Core.Math.Noise.Specialized;

public class SparseNoise2D : INoise2D {
	private readonly INoise2D baseNoise;
	private readonly float threshold;

	public SparseNoise2D(INoise2D baseNoise, float threshold = 0.7f) {
		this.baseNoise = baseNoise;
		this.threshold = threshold;
	}

	public float noise(float x, float y) {
		float value = baseNoise.noise(x, y);
		return value > threshold ? 1f : 0f;
	}
}
