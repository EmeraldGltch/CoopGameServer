using System;
using System.Collections.Generic;

using CoopGame.Server.Core.Dirty;

using CoopGame.Shared.World.Terrain;

namespace CoopGame.Server.World;

public enum ChunkState {
    Dormant,
    Simulated,
    Rendered
}

public class Chunk : IReadOnlyChunk {
    public int chunkX { get; }
    public int chunkY { get; }
    public int size => tiles.GetLength(0);

    public Tile[,] tiles { get; }

    public IReadOnlyTile getTile(int x, int y) {
        return tiles[x, y];
    }

    public ChunkState state { get; set; }
    public DateTime lastSimulatedTime { get; set; }

    public int pendingTicks { get; set; } = 0;

    // Dirty Tile Tracking
    private readonly Dictionary<TileDirtyReason, HashSet<(int x, int y)>> dirtyTiles = [];

    // DEBUG
    public int totalSimulatedTicks { get; private set; } = 0;
    public int totalCatchUpTicks { get; private set; } = 0;

    public void onSimulated(bool isCatchUp) {
        totalSimulatedTicks++;

        if (isCatchUp) {
            totalCatchUpTicks++;
        }
    }

    public Chunk(int chunkX, int chunkY, int size) {
        this.chunkX = chunkX;
        this.chunkY = chunkY;

        state = ChunkState.Dormant;
        tiles = new Tile[size, size];

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                tiles[x, y] = new Tile();
            }
        }

        lastSimulatedTime = DateTime.UtcNow;
    }

    public void scheduleCatchUp(DateTime currentWorldTime, float tickRate) {
        if (state != ChunkState.Dormant) {
            return;
        }

        state = ChunkState.Simulated;

        double elapsedSeconds = (currentWorldTime - lastSimulatedTime).TotalSeconds;
        pendingTicks = (int)Math.Floor(elapsedSeconds * tickRate);
    }

    // Dirty Marker API
    public void markTileDirty(int x, int y, TileDirtyReason reason) {
        if (!dirtyTiles.TryGetValue(reason, out var set)) {
            set = [];
            dirtyTiles[reason] = set;
        }

        set.Add((x, y));
    }

    public IReadOnlyCollection<(int x, int y)> getDirtyTiles(TileDirtyReason reason) {
        if (dirtyTiles.TryGetValue(reason, out var set)) {
            return set;
        }

        return [];
    }

    public bool hasDirtyTiles(TileDirtyReason reason) {
        return dirtyTiles.TryGetValue(reason, out var set) && set.Count > 0;
    }

    public void clearDirty(TileDirtyReason reason) {
        if (dirtyTiles.TryGetValue(reason, out var set)) {
            set.Clear();
        }
    }

    public void clearAllDirty() {
        foreach (var set in dirtyTiles.Values) {
            set.Clear();
        }
    }
}
