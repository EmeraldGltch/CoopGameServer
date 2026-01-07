using System.Collections.Generic;

namespace CoopGame.Server.Core.Dirty;

public interface IDirtyMarker<TileDirtyReason> {
    bool isDirty { get; }
    IReadOnlySet<TileDirtyReason> dirtyReasons { get; }

    void markDirty(TileDirtyReason reason);
    void clearDirty();
}
