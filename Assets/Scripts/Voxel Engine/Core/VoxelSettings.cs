using UnityEngine;

public static class VoxelSettings
{
    public static readonly int textureAtlasSize = 16;
    public static readonly float textureNomalizedSize = 1f / textureAtlasSize;
    public static readonly Vector3Int worldSize = new Vector3Int(24, 5, 24); // 256, 5, 256
    public static readonly Vector3Int chunkSize = new Vector3Int(15, 55, 15); // Only odd numbers 1, 3, 5, 7, 9
}