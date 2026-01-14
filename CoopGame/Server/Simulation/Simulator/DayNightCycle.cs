using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopGame.Server.Simulation.Simulator;

public class DayNightCycle {
	public float timeOfDay { get; private set; }
	public int dayCount { get; private set; }

	private readonly float dayLengthInSeconds;

	public event Action<int>? onNewDay;

	public DayNightCycle(float dayLengthInSeconds) {
		this.dayLengthInSeconds = dayLengthInSeconds;
		timeOfDay = 0f;
		dayCount = 0;
	}

	public void tick(float deltaSeconds) {
		float deltaNormalized = (float)(deltaSeconds / dayLengthInSeconds);
		timeOfDay += deltaNormalized;

		if(timeOfDay >= 1f) {
			timeOfDay -= 1f;
			dayCount++;

			onNewDay?.Invoke(dayCount);
		}
	}

	public bool isDay() {
		return timeOfDay >= 0.25f && timeOfDay < 0.75f;
	}

	public bool isNight() {
		return !isDay();
	}

	public float sunAngle() {
		return timeOfDay * 360f;
	}
}
