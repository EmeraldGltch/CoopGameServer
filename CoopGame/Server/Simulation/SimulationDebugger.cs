using System;
using System.Collections.Generic;

using CoopGame.Server.World;

namespace CoopGame.Server.Simulation;

public class SimulationDebugger {
    public bool isEnabled { get; set; } = false;

    public void validateChunk(Chunk chunk) {
        if (!isEnabled)
            return;

        for (int x = 0; x < chunk.tiles.GetLength(0); x++) {
            for (int y = 0; y < chunk.tiles.GetLength(0); y++) {
                var tile = chunk.tiles[x, y];

                if (tile.pollutionLevel < 0f || tile.pollutionLevel > 1f)
                    logError($"Tile ({x},{y}) in Chunk ({chunk.chunkX},{chunk.chunkY}) has invalid pollution: {tile.pollutionLevel}");
                if (tile.fertilityLevel < 0f || tile.fertilityLevel > 1f)
                    logError($"Tile ({x},{y}) in Chunk ({chunk.chunkX},{chunk.chunkY}) has invalid fertility: {tile.fertilityLevel}");
            }
        }
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
