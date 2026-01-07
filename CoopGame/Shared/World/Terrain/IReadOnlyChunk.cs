namespace CoopGame.Shared.World.Terrain;

public interface IReadOnlyChunk {
    int chunkX { get; }
    int chunkY { get; }

    int size { get; }

    IReadOnlyTile getTile(int x, int y);
}
