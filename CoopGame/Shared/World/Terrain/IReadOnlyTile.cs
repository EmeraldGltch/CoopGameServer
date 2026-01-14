namespace CoopGame.Shared.World.Terrain;

public interface IReadOnlyTile {
    TerrainType terrainType { get; }
    float elevation { get; }
}
