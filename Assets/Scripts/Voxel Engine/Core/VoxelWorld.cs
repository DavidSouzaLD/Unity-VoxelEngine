using System.Collections.Generic;
using UnityEngine;

public class VoxelWorld : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform player;

    [Header("Distace View")]
    [SerializeField] private bool useDistanceView;
    [SerializeField] private float timeToCheckDistanceView = 10;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos;
    [SerializeField] private bool showDistanceViewGizmos;

    private ChunkCoord[,,] coordMap = new ChunkCoord[VoxelSettings.worldSize.x, VoxelSettings.worldSize.y, VoxelSettings.worldSize.z];
    public List<Chunk> activeChunks = new List<Chunk>();
    Vector3 lastPlayerPosition;

    private void Start()
    {
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

        VoxelTemplate.CreatePlane(this, Vector3Int.zero, 2, 10);

        if (useDistanceView)
        {
            InvokeRepeating("UpdateViewDistance", 0f, timeToCheckDistanceView);
        }
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

    // Ativa all chunks that are in the player's view.
    public void UpdateViewDistance()
    {
        float distance = Vector3.Distance(player.position, lastPlayerPosition);

        if (distance > VoxelSettings.maxDistanceView)
        {
            for (int i = 0; i < activeChunks.Count; i++)
            {
                activeChunks[i].isActived = false;
            }

            activeChunks.Clear();

            int distanceView = VoxelSettings.maxDistanceView / 2;
            int height = VoxelSettings.worldSize.y / 2;

            for (int x = -distanceView; x < distanceView; x++)
            {
                for (int y = -height; y < height + 1; y++)
                {
                    for (int z = -distanceView; z < distanceView; z++)
                    {
                        Vector3Int chunksPos = player.position.ToVector3Int() + new Vector3Int(x, y, z) * VoxelSettings.chunkSize + (VoxelSettings.chunkSize / 2);
                        chunksPos.y -= (VoxelSettings.chunkSize.y / 2);

                        ChunkCoord coord = GetChunkCoord(chunksPos);

                        if (coord != null && coord.chunk != null)
                        {
                            activeChunks.Add(coord.chunk);
                        }
                    }
                }
            }

            for (int i = 0; i < activeChunks.Count; i++)
            {
                activeChunks[i].isActived = true;
            }

            lastPlayerPosition = player.position;
        }
    }

    // Checks if the chunk exists, if it doesn't exist it will create a new one.
    public Chunk CreateChunk(Vector3Int _position)
    {
        // Checks if a chunk already exists at that location.
        Chunk chunk = GetChunk(_position);

        // If it does not exist, it creates one and placed it in the coord.
        if (chunk == null)
        {
            // Creating chunk.
            ChunkCoord coord = GetChunkCoord(_position);

            if (coord != null)
            {
                chunk = new Chunk(coord, this);
                coord.chunk = chunk;

                // Sending to list and then rendering your mesh on GPU.
                activeChunks.Add(chunk);
            }

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

        if (chunk != null)
        {
            chunk.EditMap(_position, _type, _updateChunk);
        }
        else
        {
            Debug.Log("sem chunk");
        }
    }

    // Get block type based on past position.
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
        try
        {
            return GetChunkCoord(_position).chunk;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    // Returns the ChunkCoord based on position.
    public ChunkCoord GetChunkCoord(Vector3 _position)
    {
        Vector3 fix = ((VoxelSettings.worldSize * VoxelSettings.chunkSize) / 2) - (VoxelSettings.chunkSize / 2);
        Vector3 pos = _position + fix;

        int x = Mathf.RoundToInt(pos.x / VoxelSettings.chunkSize.x);
        int y = Mathf.RoundToInt(pos.y / VoxelSettings.chunkSize.y);
        int z = Mathf.RoundToInt(pos.z / VoxelSettings.chunkSize.z);

        try
        {
            return coordMap[x, y, z];
        }
        catch (System.Exception)
        {
            return null;
        }
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

            if (showDistanceViewGizmos)
            {
                Gizmos.color = Color.red;

                int distanceView = VoxelSettings.maxDistanceView / 2;
                int height = VoxelSettings.worldSize.y / 2;

                for (int x = -distanceView; x < distanceView; x++)
                {
                    for (int y = -height; y < height + 1; y++)
                    {
                        for (int z = -distanceView; z < distanceView; z++)
                        {
                            Vector3Int chunksPos = player.position.ToVector3Int() + new Vector3Int(x, y, z) * VoxelSettings.chunkSize + (VoxelSettings.chunkSize / 2);
                            chunksPos.y -= (VoxelSettings.chunkSize.y / 2);

                            ChunkCoord coord = GetChunkCoord(chunksPos);

                            if (coord != null)
                            {
                                Gizmos.color = Color.blue;
                                Gizmos.DrawWireCube(coord.worldPosition, VoxelSettings.chunkSize);
                            }
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