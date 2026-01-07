using System;
using System.Collections.Generic;

using CoopGame.Server.Core.Dirty;

using CoopGame.Shared.World.Terrain;

namespace CoopGame.Server.World;

public class Tile : IDirtyMarker<TileDirtyReason>, IReadOnlyTile {
    public float pollutionLevel { get; set; }
    public float simulationPollutionLevel;
    public float fertilityLevel { get; private set; }

    public TerrainType terrainType { get; set; }

    private readonly HashSet<TileDirtyReason> dirtyMarkers = [];

    public bool isDirty => dirtyMarkers.Count > 0;
    public IReadOnlySet<TileDirtyReason> dirtyReasons => dirtyMarkers;

    public Tile(float initialPollution = 0f) {
        simulationPollutionLevel = initialPollution;
        pollutionLevel = Math.Max(0f, initialPollution);

        // Default terrain type
        terrainType = TerrainType.Grass;

        updateFertility();
    }

    public Tile(TerrainType type, float initialPollution = 0f) {
        simulationPollutionLevel = initialPollution;
        pollutionLevel = Math.Max(0f, initialPollution);

        // Default terrain type
        terrainType = type;

        updateFertility();
    }

    public void addPollution(float amount) {
        if (amount == 0f)
            return;

        simulationPollutionLevel += amount;
        syncPollution();
    }

    public void removePollution(float amount) {
        if (amount == 0f)
            return;

        simulationPollutionLevel -= amount;
        syncPollution();
    }

    public void syncPollution() {
        float newPollution = Math.Max(0f, simulationPollutionLevel);

        if (Math.Abs(newPollution - pollutionLevel) > 0.0001f) {
            pollutionLevel = Math.Max(0f, simulationPollutionLevel);
            markDirty(TileDirtyReason.PollutionChanged);
            updateFertility();
        }
    }

    private void updateFertility() {
        float newFertilityLevel = Math.Clamp(1f - pollutionLevel, 0f, 1f);

        if (Math.Abs(newFertilityLevel - fertilityLevel) > 0.0001f) {
            fertilityLevel = newFertilityLevel;
            markDirty(TileDirtyReason.FertilityChanged);
        }
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
