using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopGame.Server.Simulation.Simulator;

public class ServerSimulator {
	private readonly DayNightCycle dayNightCycle;

	public ServerSimulator(float dayLengthInSeconds = 60f) {
		dayNightCycle = new DayNightCycle(dayLengthInSeconds);
	}

	public void tick(double deltaSeconds) {
		dayNightCycle.tick((float)deltaSeconds);
	}

	// Read Only

	public float getTimeOfDay() {
		return dayNightCycle.timeOfDay;
	}

	public int getDayCount() {
		return dayNightCycle.dayCount;
	}

	public bool isNight() {
		return dayNightCycle.isNight();
	}

	public WorldTimeSnapshot getWorldTimeSnapshot() {
		return new WorldTimeSnapshot {
			dayCount = dayNightCycle.dayCount,
			timeOfDay = dayNightCycle.timeOfDay,
			isNight = dayNightCycle.isNight()
		};
	}
}
