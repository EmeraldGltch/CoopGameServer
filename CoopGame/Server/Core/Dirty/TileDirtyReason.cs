using System;

namespace CoopGame.Server.Core.Dirty;

public enum TileDirtyReason {
    PollutionChanged,
    FertilityChanged,
    TerrainChanged
}
