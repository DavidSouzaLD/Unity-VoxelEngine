using System.Collections.Generic;
using UnityEngine;

public class VoxelWorld : StaticInstance<VoxelWorld>
{
    public Transform test;
    [HideInInspector] public ChunkCoord[,,] coordMap = new ChunkCoord[VoxelSettings.worldSize.x, VoxelSettings.worldSize.y, VoxelSettings.worldSize.z];
    [HideInInspector] public List<Chunk> activeChunks = new List<Chunk>();

    public bool showGizmos;

    private void Start()
    {
        float startTime = Time.realtimeSinceStartup;

        // Setting and creating all ChunkCoords.
        for (int x = 0; x < VoxelSettings.worldSize.x; x++)
        {
            for (int y = 0; y < VoxelSettings.worldSize.y; y++)
            {
                for (int z = 0; z < VoxelSettings.worldSize.z; z++)
                {
                    Vector3Int fix = ((VoxelSettings.worldSize * VoxelSettings.chunkSize) / 2);
                    Vector3Int pos = new Vector3Int(x, y, z) * VoxelSettings.chunkSize - fix;

                    coordMap[x, y, z] = new ChunkCoord(pos.x, pos.y, pos.z, null);
                }
            }
        }
        Debug.Log(((startTime = Time.realtimeSinceStartup) * 1000f) + " ms");

        EditVoxel(Vector3Int.zero, 1);
    }

    private void Update()
    {
        for (int i = 0; i < activeChunks.Count; i++)
        {
            // Update mesh renderers on gpu.
            activeChunks[i].Render();

            // Checks if the chunk was destroyed in that frame.
            if (activeChunks[i].isDestroyed)
            {
                activeChunks.Remove(activeChunks[i]);
            }
        }
    }

    public void EditVoxel(Vector3Int _position, byte _type, bool _updateChunk = true)
    {
        Chunk chunk = GetChunk(_position);

        if (chunk == null)
        {
            ChunkCoord coord = GetChunkCoord(_position);
            chunk = new Chunk(coord);
            coord.chunk = chunk;
            activeChunks.Add(chunk);
        }

        // Adding or removing voxel
        chunk.EditMap(_position, _type, _updateChunk);
    }

    public bool ExistsVoxel(Vector3 _position)
    {
        try
        {
            return GetChunk(_position.ToVector3Int()).ExistsVoxel(_position);
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    // Returns the chunk based on position.
    public Chunk GetChunk(Vector3 _position)
    {
        return GetChunkCoord(_position).chunk;
    }

    // Returns the ChunkCoord based on position.
    public ChunkCoord GetChunkCoord(Vector3 _position)
    {
        Vector3 fix = ((VoxelSettings.worldSize * VoxelSettings.chunkSize) / 2) - (VoxelSettings.chunkSize / 2);
        Vector3 pos = _position + fix;

        int x = Mathf.RoundToInt(pos.x / VoxelSettings.chunkSize.x);
        int y = Mathf.RoundToInt(pos.y / VoxelSettings.chunkSize.y);
        int z = Mathf.RoundToInt(pos.z / VoxelSettings.chunkSize.z);

        return coordMap[x, y, z];
    }

    // Draw the positions of the chunks.
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.green;

            if (Application.isPlaying)
            {
                for (int x = 0; x < VoxelSettings.worldSize.x; x++)
                {
                    for (int y = 0; y < VoxelSettings.worldSize.y; y++)
                    {
                        for (int z = 0; z < VoxelSettings.worldSize.z; z++)
                        {
                            Gizmos.DrawWireCube(coordMap[x, y, z].position + (VoxelSettings.chunkSize / 2), VoxelSettings.chunkSize);
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < VoxelSettings.worldSize.x; x++)
                {
                    for (int y = 0; y < VoxelSettings.worldSize.y; y++)
                    {
                        for (int z = 0; z < VoxelSettings.worldSize.z; z++)
                        {
                            Vector3Int fix = ((VoxelSettings.worldSize * VoxelSettings.chunkSize) / 2);
                            Vector3Int pos = new Vector3Int(x, y, z) * VoxelSettings.chunkSize - fix;

                            Gizmos.DrawWireCube(pos + (VoxelSettings.chunkSize / 2), VoxelSettings.chunkSize);
                        }
                    }
                }
            }
        }
    }
#endif
}

public class ChunkCoord
{
    public int x, y, z;
    public Chunk chunk;

    public ChunkCoord(int _x, int _y, int _z, Chunk _chunk)
    {
        x = _x;
        y = _y;
        z = _z;
        chunk = _chunk;
    }

    public Vector3Int position
    {
        get
        {
            return new Vector3Int(x, y, z);
        }
    }

    public Vector3Int worldPosition
    {
        get
        {
            return new Vector3Int(x, y, z) + (VoxelSettings.chunkSize / 2);
        }
    }
}