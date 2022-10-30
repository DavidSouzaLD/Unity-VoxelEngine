using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Core;
using VoxelEngine.Core.Classes;
using QFSW.QC;

namespace VoxelEngine.Extras
{
    public class VoxelTemplate
    {
        [Command("Voxel.CreateBuild")]
        public static void CreateBuild(Vector3Int _position, string _name)
        {
            VoxelWorld activeWorld = GameObject.FindObjectOfType<VoxelWorld>();

            if (activeWorld != null)
            {
                VoxelSerializer.Build build = VoxelSerializer.GetBuild(_name + ".build");

                List<Chunk> chunksToUpdate = new List<Chunk>();
                Chunk lastChunk = null;

                for (int i = 0; i < build.datas.Count; i++)
                {
                    if (lastChunk == null)
                    {
                        // Try get chunk
                        lastChunk = activeWorld.GetChunk(build.datas[i].position + _position);

                        if (lastChunk == null)
                        {
                            // Create new chunk
                            ChunkCoord coord = activeWorld.GetChunkCoord(build.datas[i].position + _position);
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
                        if (!lastChunk.IsVoxelInChunk(build.datas[i].position + _position))
                        {
                            lastChunk = activeWorld.GetChunk(build.datas[i].position + _position);
                        }
                    }

                    // Create voxel
                    activeWorld.EditVoxel(build.datas[i].position + _position, build.datas[i].type, false);
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

        [Command("Voxel.CreatePlane")]
        public static void CreatePlane(Vector3Int position, byte type, int size)
        {
            VoxelWorld activeWorld = GameObject.FindObjectOfType<VoxelWorld>();

            if (activeWorld != null)
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
        }

        [Command("Voxel.CreateCube")]
        public static void CreateCube(Vector3Int position, byte type, int size)
        {
            VoxelWorld activeWorld = GameObject.FindObjectOfType<VoxelWorld>();

            if (activeWorld != null)
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
        }

        [Command("Voxel.CreateSphere")]
        public static void CreateSphere(Vector3Int position, byte type, int radius)
        {
            VoxelWorld activeWorld = GameObject.FindObjectOfType<VoxelWorld>();

            if (activeWorld != null)
            {
                List<Chunk> chunksToUpdate = new List<Chunk>();
                Chunk lastChunk = null;

                for (int x = -radius / 2; x < radius / 2; x++)
                {
                    for (int y = -radius / 2; y < radius / 2; y++)
                    {
                        for (int z = -radius / 2; z < radius / 2; z++)
                        {
                            Vector3Int pos = position + new Vector3Int(x, y, z);
                            float distance = Vector3.Distance(position, pos);

                            if (distance < radius / 2)
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
        }

        [Command("Voxel.CreatePyramid")]
        public static void CreatePyramid(Vector3Int position, byte type, int maxHeight)
        {
            VoxelWorld activeWorld = GameObject.FindObjectOfType<VoxelWorld>();

            if (activeWorld != null)
            {
                List<Chunk> chunksToUpdate = new List<Chunk>();
                Chunk lastChunk = null;

                for (int h = 0; h < maxHeight; h++)
                {
                    int length = maxHeight - h;

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
        }

        [Command("Voxel.CreateTorus")]
        public static void CreateTorus(Vector3Int position, byte type, int size, int innerRadius, int thickness)
        {
            VoxelWorld activeWorld = GameObject.FindObjectOfType<VoxelWorld>();

            if (activeWorld != null)
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

                            Vector3 donutCenter = direction * (innerRadius + thickness);

                            if (x == 0 && z == 0)
                                continue;

                            if (Vector3.Distance(pos, donutCenter) < thickness)
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
    }
}