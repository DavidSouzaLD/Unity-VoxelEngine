using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VoxelEngine.Core.Classes;
using VoxelEngine.Utilities;

namespace VoxelEngine.Core
{
    public class VoxelWorld : MonoBehaviour
    {
        [Header("VoxelSystem")]
        public Transform target;
        public float scale;
        public float threshold;

        [Header("Render View")]
        public bool useRenderView;
        public float timeToRenderView = 10;

        [Header("Events")]
        public UnityEvent OnWorldChanged;

        [Header("Gizmos")]
        public bool showGizmos;
        public bool showRenderViewGizmos;

        [HideInInspector] public ChunkCoord[,,] coordMap;
        [HideInInspector] public List<Chunk> activeChunks = new List<Chunk>();
        private Vector3 lastTargetPos;

        private void Awake()
        {
            // Generate map
            coordMap = new ChunkCoord[VoxelSystem.GetWorldSize.x, VoxelSystem.GetWorldSize.y, VoxelSystem.GetWorldSize.z];

            // Setting and creating all ChunkCoords.
            for (int x = 0; x < VoxelSystem.GetWorldSize.x; x++)
            {
                for (int y = 0; y < VoxelSystem.GetWorldSize.y; y++)
                {
                    for (int z = 0; z < VoxelSystem.GetWorldSize.z; z++)
                    {
                        Vector3Int fix = ((VoxelSystem.GetWorldSize * VoxelSystem.GetChunkSize) / 2);
                        Vector3Int pos = new Vector3Int(x, y, z) * VoxelSystem.GetChunkSize - fix;

                        coordMap[x, y, z] = new ChunkCoord(pos.x, pos.y, pos.z, null);
                    }
                }
            }

            // VoxelSystem render view
            if (useRenderView)
            {
                InvokeRepeating("UpdateRenderView", 0f, timeToRenderView);
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

        // Ativa all chunks that are in the target's view.
        public void UpdateRenderView()
        {
            float distance = Vector3.Distance(target.position, lastTargetPos);

            if (distance > VoxelSystem.GetMaxRenderView)
            {
                for (int i = 0; i < activeChunks.Count; i++)
                {
                    activeChunks[i].isActived = false;
                }

                activeChunks.Clear();

                int distanceView = VoxelSystem.GetMaxRenderView / 2;
                int height = VoxelSystem.GetWorldSize.y / 2;

                for (int x = -distanceView; x < distanceView; x++)
                {
                    for (int y = -height; y < height + 1; y++)
                    {
                        for (int z = -distanceView; z < distanceView; z++)
                        {
                            Vector3Int chunksPos = target.position.ToVector3Int() + new Vector3Int(x, y, z) * VoxelSystem.GetChunkSize + (VoxelSystem.GetChunkSize / 2);
                            chunksPos.y -= (VoxelSystem.GetChunkSize.y / 2);

                            ChunkCoord coord = GetChunkCoord(chunksPos);

                            if (coord != null && coord.chunk != null)
                            {
                                activeChunks.Add(coord.chunk);
                            }
                            else if (coord != null && coord.chunk == null)
                            {
                                coord.chunk = CreateChunk(coord.worldPosition);
                                activeChunks.Add(coord.chunk);
                            }
                        }
                    }
                }

                for (int i = 0; i < activeChunks.Count; i++)
                {
                    activeChunks[i].isActived = true;
                    activeChunks[i].Update();
                }

                lastTargetPos = target.position;
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

                if (_updateChunk)
                {
                    OnWorldChanged.Invoke();
                }
            }
            else
            {
                Debug.Log("Sem chunk");
            }
        }

        // Get block type based on past position.
        public byte GetVoxelType(Vector3Int _position)
        {
            Chunk chunk = GetChunk(_position);

            if (chunk != null)
            {
                return chunk.GetVoxelType(_position);
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
            Vector3 fix = ((VoxelSystem.GetWorldSize * VoxelSystem.GetChunkSize) / 2) - (VoxelSystem.GetChunkSize / 2);
            Vector3 pos = _position + fix;

            int x = Mathf.RoundToInt(pos.x / VoxelSystem.GetChunkSize.x);
            int y = Mathf.RoundToInt(pos.y / VoxelSystem.GetChunkSize.y);
            int z = Mathf.RoundToInt(pos.z / VoxelSystem.GetChunkSize.z);

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
                    for (int x = 0; x < VoxelSystem.GetWorldSize.x; x++)
                    {
                        for (int y = 0; y < VoxelSystem.GetWorldSize.y; y++)
                        {
                            for (int z = 0; z < VoxelSystem.GetWorldSize.z; z++)
                            {
                                Gizmos.DrawWireCube(coordMap[x, y, z].position + (VoxelSystem.GetChunkSize / 2), VoxelSystem.GetChunkSize);
                            }
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < VoxelSystem.GetWorldSize.x; x++)
                    {
                        for (int y = 0; y < VoxelSystem.GetWorldSize.y; y++)
                        {
                            for (int z = 0; z < VoxelSystem.GetWorldSize.z; z++)
                            {
                                Vector3Int fix = ((VoxelSystem.GetWorldSize * VoxelSystem.GetChunkSize) / 2);
                                Vector3Int pos = new Vector3Int(x, y, z) * VoxelSystem.GetChunkSize - fix;

                                Gizmos.DrawWireCube(pos + (VoxelSystem.GetChunkSize / 2), VoxelSystem.GetChunkSize);
                            }
                        }
                    }
                }

                if (showRenderViewGizmos)
                {
                    Gizmos.color = Color.red;

                    int distanceView = VoxelSystem.GetMaxRenderView / 2;
                    int height = VoxelSystem.GetWorldSize.y / 2;

                    for (int x = -distanceView; x < distanceView; x++)
                    {
                        for (int y = -height; y < height + 1; y++)
                        {
                            for (int z = -distanceView; z < distanceView; z++)
                            {
                                Vector3Int chunksPos = target.position.ToVector3Int() + new Vector3Int(x, y, z) * VoxelSystem.GetChunkSize + (VoxelSystem.GetChunkSize / 2);
                                chunksPos.y -= (VoxelSystem.GetChunkSize.y / 2);

                                ChunkCoord coord = GetChunkCoord(chunksPos);

                                if (coord != null)
                                {
                                    Gizmos.color = Color.blue;
                                    Gizmos.DrawWireCube(coord.worldPosition, VoxelSystem.GetChunkSize);
                                }
                            }
                        }
                    }
                }
            }
        }
#endif
    }
}