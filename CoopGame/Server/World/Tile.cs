using System;
using System.Collections.Generic;

using CoopGame.Server.Core.Dirty;

using CoopGame.Shared.World.Terrain;

namespace CoopGame.Server.World;

public class Tile : IDirtyMarker<TileDirtyReason>, IReadOnlyTile {
    public TerrainType terrainType { get; set; }
    public float elevation { get; set; }

	private readonly HashSet<TileDirtyReason> dirtyMarkers = [];

    public bool isDirty => dirtyMarkers.Count > 0;
    public IReadOnlySet<TileDirtyReason> dirtyReasons => dirtyMarkers;

    public Tile() {
        // Default terrain type
        terrainType = TerrainType.Grass;
        elevation = 0f;
	}

    public Tile(TerrainType type) {
        // Default terrain type
        terrainType = type;
    }

    public void setTerrain(TerrainType newType) {
        if (terrainType == newType)
            return;

        terrainType = newType;
        markDirty(TileDirtyReason.TerrainChanged);
    }

    // Dirty Marker API
    public void markDirty(TileDirtyReason reason) {
        dirtyMarkers.Add(reason);
    }

    public void clearDirty() {
        dirtyMarkers.Clear();
    }
}
