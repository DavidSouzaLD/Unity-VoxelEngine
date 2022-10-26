using UnityEngine;

public static class VoxelSettings
{
    // Value of blocks by textures
    public static int textureAtlasSize
    {
        get
        {
            return 16;
        }
    }

    // Normalized texture value
    public static float textureNormalizedSize
    {
        get
        {
            return 1f / textureAtlasSize;
        }
    }

    // Number of chunks to be generated in the world from the central point
    public static Vector3Int worldSize
    {
        get
        {
            return new Vector3Int(24, 5, 24); // 256, 5, 256
        }
    }

    // Full chunk size
    public static Vector3Int chunkSize
    {
        get
        {
            return new Vector3Int(15, 55, 15); // Only odd numbers 1, 3, 5, 7, 9
        }
    }
}