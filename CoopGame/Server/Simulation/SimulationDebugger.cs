using System;
using System.Collections.Generic;

using CoopGame.Server.World;

namespace CoopGame.Server.Simulation;

public class SimulationDebugger {
    public bool isEnabled { get; set; } = false;

    public void validateChunk(Chunk chunk) {
        if (!isEnabled)
            return;
    }

    public void logPlayerAction(int chunkX, int chunkY, int tileX, int tileY, float amount) {
        if (!isEnabled)
            return;
        Console.WriteLine($"Player changed tile ({tileX},{tileY}) in chunk ({chunkX},{chunkY}) by {amount:F3}");
    }

    public void logError(string message) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[SIM ERR] " + message);
        Console.ResetColor();
    }

    public void logInfo(string message) {
        if (!isEnabled)
            return;
        Console.WriteLine("[SIM INFO] " + message);
    }
}
