using CoopGame.Shared.Networking;
using CoopGame.Shared.World.Terrain;

namespace CoopGame.Shared.Networking.Messages;

public class ChunkDataMessage : IMessage {
    public int chunkX { get; set; }
    public int chunkY { get; set; }
    public int chunkSize { get; set; }

    public int[] terrainTypes { get; set; }             // Flattened array of every tile's terrain type
    public float[] pollutionLevels { get; set; }       // Flattened array of every tile's pollution level

    public static ChunkDataMessage fromChunk(int chunkX, int chunkY, TerrainType[,] terrainTypes, float[,] pollutionLevels) {
        return new ChunkDataMessage {
            chunkX = chunkX,
            chunkY = chunkY,
            chunkSize = terrainTypes.GetLength(0),

            terrainTypes = TerrainUtils.flattenTerrain(terrainTypes),
            pollutionLevels = TerrainUtils.flattenFloat(pollutionLevels)
        };
    }
}
