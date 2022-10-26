using System.Collections.Generic;
using UnityEngine;

public class VoxelWorld : MonoBehaviour
{
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

        VoxelTemplate.CreateCube(this, Vector3Int.zero, 1, 5);
        VoxelTemplate.CreateCube(this, new Vector3Int(20, 0, 0), 1, 20);
        VoxelTemplate.CreateCube(this, new Vector3Int(-50, 0, 0), 1, 50);
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

    // Checks if the chunk exists, if it doesn't exist it will create a new one
    public Chunk CreateChunk(Vector3Int _position)
    {
        // Checks if a chunk already exists at that location.
        Chunk chunk = GetChunk(_position);

        // If it does not exist, it creates one and placed it in the coord.
        if (chunk == null)
        {
            ChunkCoord coord = GetChunkCoord(_position);
            chunk = new Chunk(coord, this);
            coord.chunk = chunk;
            return chunk;
        }
        else
        {
            // If it exists it just returns.
            return chunk;
        }
    }

    // Edit the world globally by reacting on all chunks.
    public void EditVoxel(Vector3Int _position, byte _type, bool _updateChunk = true)
    {
        Chunk chunk = CreateChunk(_position);
        chunk.EditMap(_position, _type, _updateChunk);
    }

    public byte GetVoxelType(Vector3Int _position)
    {
        Chunk chunk = GetChunk(_position);
        Vector3Int pos = _position - chunk.position;

        if (chunk != null)
        {
            return chunk.GetVoxelType(pos);
        }
        else
        {
            return 0;
        }
    }

    // Checks if there is a voxel in global position.
    public bool ExistsVoxel(Vector3Int _position)
    {
        try
        {
            return GetChunk(_position).ExistsVoxel(_position);
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    // Returns the chunk based on position.
    public Chunk GetChunk(Vector3Int _position)
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

#if UNITY_EDITOR
    // Draw the positions of the chunks.
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