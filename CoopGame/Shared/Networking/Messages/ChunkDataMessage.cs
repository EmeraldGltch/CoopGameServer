using CoopGame.Shared.Networking;
using CoopGame.Shared.World.Terrain;

namespace CoopGame.Shared.Networking.Messages;

public class ChunkDataMessage : IMessage {
    public int chunkX { get; set; }
    public int chunkY { get; set; }
    public int chunkSize { get; set; }

	public int[] terrainTypes { get; set; }             // Flattened array of every tile's terrain type
	public float[] elevations { get; set; }             // Flattened array of every tile's elevation

	public static ChunkDataMessage fromChunk(int chunkX, int chunkY, TerrainType[,] terrainTypes, float[,] elevations) {
        return new ChunkDataMessage {
            chunkX = chunkX,
            chunkY = chunkY,
            chunkSize = terrainTypes.GetLength(0),

            elevations = TerrainUtils.flattenFloat(elevations),
			terrainTypes = TerrainUtils.flattenTerrain(terrainTypes)
        };
    }
}
