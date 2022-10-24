using UnityEngine;

public static class VoxelSettings
{
    public static readonly int maxViewDistance = 4;
    public static readonly int textureAtlasSize = 16;
    public static readonly float textureNomalizedSize = 1f / textureAtlasSize;
    public static readonly Vector3Int worldSize = new Vector3Int(24, 16, 24); // 256, 15, 256
    public static readonly Vector3Int chunkSize = new Vector3Int(15, 15, 15);
}