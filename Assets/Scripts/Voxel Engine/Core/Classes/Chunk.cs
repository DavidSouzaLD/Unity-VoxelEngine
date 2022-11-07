using UnityEngine;
using UnityEngine.AI;

namespace VoxelEngine.Core.Classes
{
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

        public ChunkCoord(Vector3Int _pos, Chunk _chunk)
        {
            x = _pos.x;
            y = _pos.y;
            z = _pos.z;
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
                return new Vector3Int(x, y, z) + (VoxelSystem.GetChunkSize / 2);
            }
        }
    }

    public class Chunk
    {
        // Voxel position map
        public byte[,,] map { get; private set; }

        // Chunk mesh
        public Mesh mesh { get; private set; }

        // Chunk material
        public Material material { get; private set; }

        // Chunk MeshCollider
        public MeshCollider meshCollider { get; private set; }

        // GameObject of the chunk
        public GameObject gameObject { get; private set; }

        // ChunkCoord responsible for the actual location of the chunk.
        public ChunkCoord coord { get; private set; }

        // VoxelWorld responsible for creating the chunk.
        public VoxelWorld world { get; private set; }

        // If chunk was destroyed in that frame.
        public bool isDestroyed { get; private set; }

        // Returns whether the chunk is on or off.
        public bool isActived
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        // Returns the position of the chunk.
        public Vector3Int position { get { return coord.position; } }

        // Create the chunk based on past information.
        public Chunk(ChunkCoord _coord, VoxelWorld _world, bool _usePopulate = false)
        {
            // Generate map
            map = new byte[VoxelSystem.GetChunkSize.x, VoxelSystem.GetChunkSize.y, VoxelSystem.GetChunkSize.z];

            // Creating object
            gameObject = new GameObject();
            gameObject.name = "Chunk [X:" + _coord.position.x + " / " + _coord.position.y + " / " + _coord.position.z + "]";
            gameObject.transform.parent = _world.transform;
            gameObject.transform.position = _coord.position;
            gameObject.layer = LayerMask.NameToLayer("Voxel");

            // Others
            meshCollider = gameObject.AddComponent<MeshCollider>();
            material = VoxelSystem.AtlasMaterial;
            coord = _coord;
            world = _world;

            if (_usePopulate)
            {
                PopulateMap();
            }
        }

        private void PopulateMap()
        {
            for (int x = 0; x < VoxelSystem.GetChunkSize.x; x++)
            {
                for (int y = 0; y < VoxelSystem.GetChunkSize.y; y++)
                {
                    for (int z = 0; z < VoxelSystem.GetChunkSize.z; z++)
                    {
                        Vector3Int pos = new Vector3Int(
                                x + position.x,
                                y + position.y,
                                z + position.z
                            );

                        map[x, y, z] = 0;
                    }
                }
            }
        }

        // Render the chunk every frame on the gpu.
        public void Render()
        {
            if (mesh != null)
            {
                Graphics.DrawMesh(mesh, position, Quaternion.identity, material, 0);
            }
        }

        // Updates all mesh information and stuff.
        public void Update()
        {
            bool destroyChunk = true;

            // Mesh
            Mesh newMesh = CreateMesh(out destroyChunk);
            mesh = newMesh;
            meshCollider.sharedMesh = newMesh;

            if (destroyChunk)
            {
                coord.chunk = null;
                MonoBehaviour.Destroy(mesh);
                MonoBehaviour.Destroy(meshCollider);
                MonoBehaviour.Destroy(gameObject);
                isDestroyed = true;
            }
        }

        // Not used
        private void UpdateSurroundVoxels(Vector3Int _position)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3Int voxelToCheck = _position + MeshData.directions[i];

                if (!IsVoxelInChunk(voxelToCheck))
                {
                    Chunk chunk = world.GetChunk(voxelToCheck);

                    if (chunk != null)
                    {
                        chunk.Update();
                    }
                }
            }
        }

        // Edit the voxel map to change the mesh.
        public void EditMap(Vector3Int _position, byte _type, bool _updateChunk = true)
        {
            Vector3Int pos = WorldToChunk(_position);
            map[pos.x, pos.y, pos.z] = _type;

            if (_updateChunk)
            {
                Update();
            }
        }

        // Returns current mesh based on voxel map information.
        private Mesh CreateMesh(out bool _destroyChunk)
        {
            _destroyChunk = true;

            MeshData.Container meshContainer = new MeshData.Container();

            for (int x = 0; x < VoxelSystem.GetChunkSize.x; x++)
            {
                for (int y = 0; y < VoxelSystem.GetChunkSize.y; y++)
                {
                    for (int z = 0; z < VoxelSystem.GetChunkSize.z; z++)
                    {
                        Vector3Int voxelPos = new Vector3Int(x, y, z);

                        if (VoxelSystem.GetVoxelPack[map[x, y, z]].solid)
                        {
                            for (int f = 0; f < 6; f++)
                            {
                                Vector3Int voxelToCheck = voxelPos + MeshData.directions[f];

                                if (!ExistsVoxel(voxelToCheck))
                                {
                                    AddFace(meshContainer, voxelPos, f, VoxelSystem.GetVoxelPack[map[x, y, z]].GetTextureID(f));
                                    _destroyChunk = false;
                                }
                            }
                        }
                    }
                }
            }

            // Create mesh
            Mesh mesh1 = new Mesh();
            mesh1.vertices = meshContainer.vertices.ToArray();
            mesh1.triangles = meshContainer.triangles.ToArray();
            mesh1.uv = meshContainer.uvs.ToArray();

            mesh1.RecalculateBounds();
            mesh1.RecalculateTangents();
            mesh1.RecalculateNormals();

            return mesh1;
        }

        // Adds a face in the desired position and direction.
        private void AddFace(MeshData.Container _meshContainer, Vector3 _voxelPos, int _faceIndex, int _textureID)
        {
            // Vertices
            _meshContainer.vertices.Add(_voxelPos - MeshData.fixedPos + MeshData.vertices[MeshData.triangles[_faceIndex, 0]]);
            _meshContainer.vertices.Add(_voxelPos - MeshData.fixedPos + MeshData.vertices[MeshData.triangles[_faceIndex, 1]]);
            _meshContainer.vertices.Add(_voxelPos - MeshData.fixedPos + MeshData.vertices[MeshData.triangles[_faceIndex, 2]]);
            _meshContainer.vertices.Add(_voxelPos - MeshData.fixedPos + MeshData.vertices[MeshData.triangles[_faceIndex, 3]]);

            // UVS
            AddTexture(_meshContainer, _textureID);

            // Triangles
            _meshContainer.triangles.Add(_meshContainer.vertexIndex);
            _meshContainer.triangles.Add(_meshContainer.vertexIndex + 1);
            _meshContainer.triangles.Add(_meshContainer.vertexIndex + 2);
            _meshContainer.triangles.Add(_meshContainer.vertexIndex + 2);
            _meshContainer.triangles.Add(_meshContainer.vertexIndex + 1);
            _meshContainer.triangles.Add(_meshContainer.vertexIndex + 3);
            _meshContainer.vertexIndex += 4;
        }

        // Reworks the UV map based on the Atlas map.
        private void AddTexture(MeshData.Container _meshContainer, int _textureID)
        {
            float y = _textureID / VoxelSystem.GetTextureAtlasSize;
            float x = _textureID - (y * VoxelSystem.GetTextureAtlasSize);

            x *= VoxelSystem.GetTextureNormalizedSize;
            y *= VoxelSystem.GetTextureNormalizedSize;

            y = 1f - y - VoxelSystem.GetTextureNormalizedSize;

            _meshContainer.uvs.Add(new Vector2(x, y));
            _meshContainer.uvs.Add(new Vector2(x, y + VoxelSystem.GetTextureNormalizedSize));
            _meshContainer.uvs.Add(new Vector2(x + VoxelSystem.GetTextureNormalizedSize, y));
            _meshContainer.uvs.Add(new Vector2(x + VoxelSystem.GetTextureNormalizedSize, y + VoxelSystem.GetTextureNormalizedSize));
        }

        // Get voxel type with position
        public byte GetVoxelType(Vector3Int _position)
        {
            Vector3Int pos = WorldToChunk(_position);

            if (!IsVoxelInChunk(pos))
            {
                return world.GetVoxelType(pos + position);
            }
            else
            {
                return map[pos.x, pos.y, pos.z];
            }
        }

        // Checks if there is a solid voxel at that position.
        public bool ExistsVoxel(Vector3Int _position)
        {
            if (!IsVoxelInChunk(_position))
            {
                return world.ExistsVoxel(_position + position);
            }
            else
            {
                return VoxelSystem.GetVoxelPack[map[_position.x, _position.y, _position.z]].solid;
            }
        }

        // Check if a block is inside chunk
        public bool IsVoxelInChunk(Vector3Int _position)
        {
            return !(
                _position.x < 0 || _position.x > VoxelSystem.GetChunkSize.x - 1 ||
                _position.y < 0 || _position.y > VoxelSystem.GetChunkSize.y - 1 ||
                _position.z < 0 || _position.z > VoxelSystem.GetChunkSize.z - 1
            );
        }

        // Transforms the global value into the local value of the chunk.
        public Vector3Int WorldToChunk(Vector3Int _position)
        {
            return _position - position;
        }
    }
}