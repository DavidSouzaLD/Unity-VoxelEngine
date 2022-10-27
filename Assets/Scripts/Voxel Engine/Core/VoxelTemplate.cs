using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;

public class VoxelTemplate : MonoBehaviour
{
    private static VoxelWorld activeWorld;

    private void Awake()
    {
        activeWorld = GetComponent<VoxelWorld>();
    }

    [Command]
    public static void CreatePlane(Vector3Int position, byte type, int size)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();
        Chunk lastChunk = null;

        for (int x = -size / 2; x < size / 2; x++)
        {
            for (int z = -size / 2; z < size / 2; z++)
            {
                Vector3Int pos = position + new Vector3Int(x, 0, z);

                if (lastChunk == null)
                {
                    // Try get chunk
                    lastChunk = activeWorld.GetChunk(pos);

                    if (lastChunk == null)
                    {
                        // Create new chunk
                        ChunkCoord coord = activeWorld.GetChunkCoord(pos);
                        lastChunk = new Chunk(coord, activeWorld);
                        coord.chunk = lastChunk;
                    }
                }

                if (lastChunk != null)
                {
                    // Check if have a same chunk in list
                    if (!chunksToUpdate.Contains(lastChunk))
                    {
                        chunksToUpdate.Add(lastChunk);
                    }

                    // Change last chunk
                    if (!lastChunk.IsVoxelInChunk(pos))
                    {
                        lastChunk = activeWorld.GetChunk(pos);
                    }
                }

                // Create voxel
                activeWorld.EditVoxel(pos, type, false);
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();

            if (!activeWorld.activeChunks.Contains(chunksToUpdate[i]))
            {
                activeWorld.activeChunks.Add(chunksToUpdate[i]);
            }
        }
    }

    [Command]
    public static void CreateCube(Vector3Int position, byte type, int size)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();
        Chunk lastChunk = null;

        for (int x = -size / 2; x < size / 2; x++)
        {
            for (int y = -size / 2; y < size / 2; y++)
            {
                for (int z = -size / 2; z < size / 2; z++)
                {
                    Vector3Int pos = position + new Vector3Int(x, y, z);

                    if (lastChunk == null)
                    {
                        // Try get chunk
                        lastChunk = activeWorld.GetChunk(pos);

                        if (lastChunk == null)
                        {
                            // Create new chunk
                            ChunkCoord coord = activeWorld.GetChunkCoord(pos);
                            lastChunk = new Chunk(coord, activeWorld);
                            coord.chunk = lastChunk;
                        }
                    }

                    if (lastChunk != null)
                    {
                        // Check if have a same chunk in list
                        if (!chunksToUpdate.Contains(lastChunk))
                        {
                            chunksToUpdate.Add(lastChunk);
                        }

                        // Change last chunk
                        if (!lastChunk.IsVoxelInChunk(pos))
                        {
                            lastChunk = activeWorld.GetChunk(pos);
                        }
                    }

                    // Create voxel
                    activeWorld.EditVoxel(pos, type, false);
                }
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();

            if (!activeWorld.activeChunks.Contains(chunksToUpdate[i]))
            {
                activeWorld.activeChunks.Add(chunksToUpdate[i]);
            }
        }
    }

    [Command]
    public static void CreateSphere(Vector3Int position, byte type, int _radius)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();
        Chunk lastChunk = null;

        for (int x = -_radius / 2; x < _radius / 2; x++)
        {
            for (int y = -_radius / 2; y < _radius / 2; y++)
            {
                for (int z = -_radius / 2; z < _radius / 2; z++)
                {
                    Vector3Int pos = position + new Vector3Int(x, y, z);
                    float distance = Vector3.Distance(position, pos);

                    if (distance < _radius / 2)
                    {
                        if (lastChunk == null)
                        {
                            // Try get chunk
                            lastChunk = activeWorld.GetChunk(pos);

                            if (lastChunk == null)
                            {
                                // Create new chunk
                                ChunkCoord coord = activeWorld.GetChunkCoord(pos);
                                lastChunk = new Chunk(coord, activeWorld);
                                coord.chunk = lastChunk;
                            }
                        }

                        if (lastChunk != null)
                        {
                            // Check if have a same chunk in list
                            if (!chunksToUpdate.Contains(lastChunk))
                            {
                                chunksToUpdate.Add(lastChunk);
                            }

                            // Change last chunk
                            if (!lastChunk.IsVoxelInChunk(pos))
                            {
                                lastChunk = activeWorld.GetChunk(pos);
                            }
                        }

                        // Create voxel
                        activeWorld.EditVoxel(pos, type, false);
                    }
                }
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();

            if (!activeWorld.activeChunks.Contains(chunksToUpdate[i]))
            {
                activeWorld.activeChunks.Add(chunksToUpdate[i]);
            }
        }
    }

    [Command]
    public static void CreatePyramid(Vector3Int position, byte type, int _maxHeight)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();
        Chunk lastChunk = null;

        for (int h = 0; h < _maxHeight; h++)
        {
            int length = _maxHeight - h;

            for (int x = -length; x <= length; x++)
            {
                for (int z = -length; z <= length; z++)
                {
                    Vector3Int pos = position + new Vector3Int(x, h, z);

                    if (Mathf.Abs(x) == length || Mathf.Abs(z) == length)
                    {
                        if (lastChunk == null)
                        {
                            // Try get chunk
                            lastChunk = activeWorld.GetChunk(pos);

                            if (lastChunk == null)
                            {
                                // Create new chunk
                                ChunkCoord coord = activeWorld.GetChunkCoord(pos);
                                lastChunk = new Chunk(coord, activeWorld);
                                coord.chunk = lastChunk;
                            }
                        }

                        if (lastChunk != null)
                        {
                            // Check if have a same chunk in list
                            if (!chunksToUpdate.Contains(lastChunk))
                            {
                                chunksToUpdate.Add(lastChunk);
                            }

                            // Change last chunk
                            if (!lastChunk.IsVoxelInChunk(pos))
                            {
                                lastChunk = activeWorld.GetChunk(pos);
                            }
                        }

                        // Create voxel
                        activeWorld.EditVoxel(pos, type, false);
                    }
                }
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();

            if (!activeWorld.activeChunks.Contains(chunksToUpdate[i]))
            {
                activeWorld.activeChunks.Add(chunksToUpdate[i]);
            }
        }
    }

    [Command]
    public static void CreateTorus(Vector3Int position, byte type, int size, int _innerRadius, int _thickness)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();
        Chunk lastChunk = null;

        for (int x = -size; x < size; x++)
        {
            for (int y = -size; y < size; y++)
            {
                for (int z = -size; z < size; z++)
                {
                    Vector3Int realPos = position + new Vector3Int(x, y, z);
                    Vector3Int pos = new Vector3Int(x, y, z);
                    Vector3 direction = new Vector3(pos.x, 0, pos.z);

                    direction.Normalize();

                    Vector3 donutCenter = direction * (_innerRadius + _thickness);

                    if (x == 0 && z == 0)
                        continue;

                    if (Vector3.Distance(pos, donutCenter) < _thickness)
                    {
                        if (lastChunk == null)
                        {
                            // Try get chunk
                            lastChunk = activeWorld.GetChunk(realPos);

                            if (lastChunk == null)
                            {
                                // Create new chunk
                                ChunkCoord coord = activeWorld.GetChunkCoord(realPos);
                                lastChunk = new Chunk(coord, activeWorld);
                                coord.chunk = lastChunk;
                            }
                        }

                        if (lastChunk != null)
                        {
                            // Check if have a same chunk in list
                            if (!chunksToUpdate.Contains(lastChunk))
                            {
                                chunksToUpdate.Add(lastChunk);
                            }

                            // Change last chunk
                            if (!lastChunk.IsVoxelInChunk(realPos))
                            {
                                lastChunk = activeWorld.GetChunk(realPos);
                            }
                        }

                        // Create voxel
                        activeWorld.EditVoxel(realPos, type, false);
                    }
                }
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();

            if (!activeWorld.activeChunks.Contains(chunksToUpdate[i]))
            {
                activeWorld.activeChunks.Add(chunksToUpdate[i]);
            }
        }
    }
}