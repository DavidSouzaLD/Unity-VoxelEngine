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

        for (int x = -_radius; x < _radius; x++)
        {
            for (int y = -_radius; y < _radius; y++)
            {
                for (int z = -_radius; z < _radius; z++)
                {
                    Vector3Int pos = _position + new Vector3Int(x, y, z);
                    float distance = Vector3.Distance(_position, pos);

                    if (distance < _radius)
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
}