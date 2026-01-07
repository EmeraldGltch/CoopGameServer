namespace CoopGame.Shared.World.Terrain;

public static class TerrainUtils {
    public static int[] flattenTerrain(TerrainType[,] terrain) {
        int width = terrain.GetLength(0);
        int height = terrain.GetLength(1);
        int[] flat = new int[width * height];

        int i = 0;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                flat[i++] = (int)terrain[x, y];
            }
        }

        return flat;
    }

    public static TerrainType[,] unflattenTerrain(int[] flat, int width, int height) {
        if(flat.Length != width * height) {
            throw new ArgumentException($"Flat terrain length {flat.Length} does not match {width}x{height}");
        }

        TerrainType[,] terrain = new TerrainType[width, height];

        int i = 0;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                terrain[x, y] = (TerrainType)flat[i++];
            }
        }

        return terrain;
    }

    public static float[] flattenFloat(float[,] values) {
        int sizeX = values.GetLength(0);
        int sizeY = values.GetLength(1);
        float[] result = new float[sizeX * sizeY];

        int i = 0;
        
        for(int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                result[i++] = values[x, y];
            }
        }

        return result;
    }

    public static float[,] unflattenFloat(float[] values, int size) {
        float[,] result = new float[size, size];

        int i = 0;

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                result[x, y] = values[i++];
            }
        }

        return result;
    }
}
