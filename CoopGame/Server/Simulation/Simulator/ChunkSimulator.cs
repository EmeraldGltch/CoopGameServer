using System;
using System.Collections.Generic;

using CoopGame.Server.Core.Dirty;
using CoopGame.Server.World;

namespace CoopGame.Server.Simulation.Simulator;

public class ChunkSimulator {
    public ChunkSimulator() {

    }

    // Simulate a given chunk for one tick
    public void simulateChunk(Chunk chunk, double deltaTime) {
        if (chunk == null)
            return;

        //////////////////////////////////////
        // All per-chunk mechanics go here! //
        //////////////////////////////////////
        
        //simulatePollution(chunk);
        //simulateWeather(chunk, deltaTime);
        //simulateCrops(chunk, deltaTime);

        chunk.lastSimulatedTime = DateTime.UtcNow;
    }
}
