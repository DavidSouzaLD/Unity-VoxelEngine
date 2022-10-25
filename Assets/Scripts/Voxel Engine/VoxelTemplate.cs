using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public class VoxelTemplate : MonoBehaviour
{
    public static void CreateCube(Vector3Int _position, byte _type, int _size)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();
        Chunk lastChunk = null;

        for (int x = -_size / 2; x < _size / 2; x++)
        {
            for (int y = -_size / 2; y < _size / 2; y++)
            {
                for (int z = -_size / 2; z < _size / 2; z++)
                {
                    Vector3Int pos = _position + new Vector3Int(x, y, z);

                    if (lastChunk == null)
                    {
                        // Try get chunk
                        lastChunk = VoxelWorld.Instance.GetChunk(pos);

                        if (lastChunk == null)
                        {
                            // Create new chunk
                            ChunkCoord coord = VoxelWorld.Instance.GetChunkCoord(pos);
                            lastChunk = new Chunk(coord);
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
                            lastChunk = VoxelWorld.Instance.GetChunk(pos);
                        }
                    }

                    // Create voxel
                    VoxelWorld.Instance.EditVoxel(pos, _type, false);
                }
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();

            if (!VoxelWorld.Instance.activeChunks.Contains(chunksToUpdate[i]))
            {
                VoxelWorld.Instance.activeChunks.Add(chunksToUpdate[i]);
            }
        }
    }

    public static void CreateSphere(Vector3Int _position, byte _type, int _radius)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();
        Chunk lastChunk = null;

        for (int x = -_radius / 2; x < _radius / 2; x++)
        {
            for (int y = -_radius / 2; y < _radius / 2; y++)
            {
                for (int z = -_radius / 2; z < _radius / 2; z++)
                {
                    Vector3Int pos = _position + new Vector3Int(x, y, z);
                    float distance = Vector3.Distance(_position, pos);

                    if (distance < _radius / 2)
                    {
                        if (lastChunk == null)
                        {
                            // Try get chunk
                            lastChunk = VoxelWorld.Instance.GetChunk(pos);

                            if (lastChunk == null)
                            {
                                // Create new chunk
                                ChunkCoord coord = VoxelWorld.Instance.GetChunkCoord(pos);
                                lastChunk = new Chunk(coord);
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
                                lastChunk = VoxelWorld.Instance.GetChunk(pos);
                            }
                        }

                        // Create voxel
                        VoxelWorld.Instance.EditVoxel(pos, _type, false);
                    }
                }
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();

            if (!VoxelWorld.Instance.activeChunks.Contains(chunksToUpdate[i]))
            {
                VoxelWorld.Instance.activeChunks.Add(chunksToUpdate[i]);
            }
        }
    }

    public static void CreatePyramid(Vector3Int _position, byte _type, int _maxHeight)
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
                    Vector3Int pos = _position + new Vector3Int(x, h, z);

                    if (Mathf.Abs(x) == length || Mathf.Abs(z) == length)
                    {
                        if (lastChunk == null)
                        {
                            // Try get chunk
                            lastChunk = VoxelWorld.Instance.GetChunk(pos);

                            if (lastChunk == null)
                            {
                                // Create new chunk
                                ChunkCoord coord = VoxelWorld.Instance.GetChunkCoord(pos);
                                lastChunk = new Chunk(coord);
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
                                lastChunk = VoxelWorld.Instance.GetChunk(pos);
                            }
                        }

                        // Create voxel
                        VoxelWorld.Instance.EditVoxel(pos, _type, false);
                    }
                }
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();

            if (!VoxelWorld.Instance.activeChunks.Contains(chunksToUpdate[i]))
            {
                VoxelWorld.Instance.activeChunks.Add(chunksToUpdate[i]);
            }
        }
    }

    public static void CreateTorus(Vector3Int _position, byte _type, int _size, int _innerRadius, int _thickness)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();
        Chunk lastChunk = null;

        for (int x = -_size; x < _size; x++)
        {
            for (int y = -_size; y < _size; y++)
            {
                for (int z = -_size; z < _size; z++)
                {
                    Vector3Int realPos = _position + new Vector3Int(x, y, z);
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
                            lastChunk = VoxelWorld.Instance.GetChunk(realPos);

                            if (lastChunk == null)
                            {
                                // Create new chunk
                                ChunkCoord coord = VoxelWorld.Instance.GetChunkCoord(realPos);
                                lastChunk = new Chunk(coord);
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
                                lastChunk = VoxelWorld.Instance.GetChunk(realPos);
                            }
                        }

                        // Create voxel
                        VoxelWorld.Instance.EditVoxel(realPos, _type, false);
                    }
                }
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();

            if (!VoxelWorld.Instance.activeChunks.Contains(chunksToUpdate[i]))
            {
                VoxelWorld.Instance.activeChunks.Add(chunksToUpdate[i]);
            }
        }
    }
}