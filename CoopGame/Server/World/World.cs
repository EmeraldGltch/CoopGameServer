using System;
using System.Collections.Generic;
using System.Linq;

using CoopGame.Server.Simulation;

namespace CoopGame.Server.World;

public class World {
    private readonly SimulationDebugger debugger = new SimulationDebugger();

    private readonly int chunkSize;
    private readonly float tickRate; // Default: 24TPS

    public Chunk testChunk;

    public World(int chunkSize = 16, float tickRate = 24f) {
        this.chunkSize = chunkSize;
        this.tickRate = tickRate;

        testChunk = new Chunk(0, 0, chunkSize);
    }

    public void applyPlayerPollution(int tileX, int tileY, float amount, bool removePollution = false) {
        if (removePollution) {
            testChunk.tiles[tileX, tileY].removePollution(amount);
        } else {
            testChunk.tiles[tileX, tileY].addPollution(amount);
        }

        debugger.logPlayerAction(0, 0, tileX, tileY, removePollution ? -amount : amount);
    }

    public void tick() {
        // TODO: ChunkManager
    }
}
