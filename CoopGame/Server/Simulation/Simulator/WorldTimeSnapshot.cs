using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopGame.Server.Simulation.Simulator;

public struct WorldTimeSnapshot {
	public float timeOfDay;
	public int dayCount;
	public bool isNight;
}
