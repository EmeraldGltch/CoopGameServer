using CoopGame.Server.Core.Math.Noise;
using CoopGame.Shared.World.Terrain;

namespace CoopGame.Server.Core.Generation;

public class TerrainNoiseSampler {
	private readonly INoise2D elevationNoise;
	private readonly INoise2D moistureNoise;
	private readonly INoise2D temperatureNoise;

	public TerrainNoiseSampler(INoise2D elevationNoise, INoise2D moistureNoise, INoise2D temperatureNoise) {
		this.elevationNoise = elevationNoise;
		this.moistureNoise = moistureNoise;
		this.temperatureNoise = temperatureNoise;
	}

	public TerrainSample sample(float worldX, float worldY) {
		return new TerrainSample {
			elevation = normalize(elevationNoise.noise(worldX, worldY)),
			moisture = normalize(moistureNoise.noise(worldX, worldY)),
			temperature = normalize(temperatureNoise.noise(worldX, worldY))
		};
	}

	private static float normalize(float value) {
		return (value + 1f) / 2f;
	}

	public static TerrainType determineBiome(TerrainSample sample) {
		if (sample.elevation <= 0.05f) {
			return TerrainType.Water;
		} else if (sample.elevation < 0.1f) {
			if (sample.temperature < 0.6f && sample.temperature > 0.2f) {
				if (sample.moisture <= 0.35f) {
					return TerrainType.Sand;
				} else {
					return TerrainType.Water;
				}
			} else if (sample.temperature <= 0.2f) {
				if (sample.moisture <= 0.15f) {
					return TerrainType.Sand;
				} else {
					return TerrainType.Snow;
				}
			} else {
				return TerrainType.Sand;
			}
		} else if (sample.elevation < 0.25f) {
			if (sample.temperature < 0.5f && sample.temperature > 0.1f) {
				if (sample.moisture <= 0.35f) {
					return TerrainType.Grass;
				} else {
					return TerrainType.Water;
				}
			} else if (sample.temperature <= 0.1f) {
				if (sample.moisture >= 0.15f && sample.moisture <= 0.25f) {
					return TerrainType.Dirt;
				} else {
					return TerrainType.Snow;
				}
			} else {
				if (sample.moisture >= 0.15f && sample.moisture <= 0.25f) {
					return TerrainType.Dirt;
				} else {
					return TerrainType.Rock;
				}
			}
		} else if (sample.elevation < 0.4f) {
			if (sample.temperature < 0.7f && sample.temperature > 0.1f) {
				if (sample.moisture >= 0.15f && sample.moisture <= 0.25f) {
					return TerrainType.Dirt;
				} else {
					return TerrainType.Grass;
				}
			} else if (sample.temperature <= 0.1f) {
				return TerrainType.Snow;
			} else {
				if (sample.moisture >= 0.15f && sample.moisture <= 0.25f) {
					return TerrainType.Dirt;
				} else {
					return TerrainType.Grass;
				}
			}
		} else if (sample.elevation < 0.65f) {
			if (sample.temperature < 0.2f) {
				if (sample.moisture >= 0.4 && sample.moisture <= 0.5f) {
					return TerrainType.Snow;
				} else if (sample.moisture < 0.4f && sample.moisture > 0.2f) {
					return TerrainType.Dirt;
				} else {
					return TerrainType.Rock;
				}
			} else if (sample.temperature >= 0.2f && sample.temperature <= 0.3f) {
				if (sample.moisture >= 0.6f && sample.moisture <= 0.8f) {
					return TerrainType.Grass;
				} else if (sample.moisture > 0.8f) {
					return TerrainType.Water;
				} else {
					return TerrainType.Rock;
				}
			} else {
				if (sample.moisture >= 0.4f && sample.moisture <= 0.45f) {
					return TerrainType.Dirt;
				} else {
					return TerrainType.Rock;
				}
			}
		} else {
			if (sample.temperature < 0.4f) {
				if (sample.moisture >= 0.3 && sample.moisture <= 0.7f) {
					return TerrainType.Snow;
				} else if (sample.moisture < 0.3f && sample.moisture > 0.25f) {
					return TerrainType.Dirt;
				} else {
					return TerrainType.Rock;
				}
			} else if (sample.temperature >= 0.2f && sample.temperature <= 0.3f) {
				if (sample.moisture >= 0.6f && sample.moisture <= 0.8f) {
					return TerrainType.Grass;
				} else if (sample.moisture > 0.8f) {
					return TerrainType.Water;
				} else {
					return TerrainType.Rock;
				}
			} else {
				if (sample.moisture >= 0.4f && sample.moisture <= 0.45f) {
					return TerrainType.Dirt;
				} else {
					return TerrainType.Rock;
				}
			}
		}
	}
}
