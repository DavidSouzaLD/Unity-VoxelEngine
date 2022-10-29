using UnityEngine;
using VoxelEngine.Core;

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
                return new Vector3Int(x, y, z) + (Settings.chunkSize / 2);
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
        public Chunk(ChunkCoord _coord, VoxelWorld _world)
        {
            // Generate map
            map = new byte[Settings.chunkSize.x, Settings.chunkSize.y, Settings.chunkSize.z];

            // Creating object
            gameObject = new GameObject();
            gameObject.name = "Chunk [X:" + _coord.position.x + " / " + _coord.position.y + " / " + _coord.position.z + "]";
            gameObject.transform.parent = _world.transform;
            gameObject.transform.position = _coord.position;
            gameObject.layer = LayerMask.NameToLayer("Voxel");

            // Others
            meshCollider = gameObject.AddComponent<MeshCollider>();
            material = VoxelManager.AtlasMaterial;
            coord = _coord;
            world = _world;
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
            Vector3Int pos = _position - position;
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

            MeshContainer meshContainer = new MeshContainer();

            for (int x = 0; x < Settings.chunkSize.x; x++)
            {
                for (int y = 0; y < Settings.chunkSize.y; y++)
                {
                    for (int z = 0; z < Settings.chunkSize.z; z++)
                    {
                        Vector3Int voxelPos = new Vector3Int(x, y, z);

                        for (int f = 0; f < 6; f++)
                        {
                            Vector3Int voxelToCheck = voxelPos + MeshData.directions[f];

                            try
                            {
                                if (!VoxelManager.GetVoxelPack[map[(int)voxelToCheck.x, (int)voxelToCheck.y, (int)voxelToCheck.z]].solid)
                                {
                                    if (map[(int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z] != 0)
                                    {
                                        AddFace(meshContainer, voxelPos, f, VoxelManager.GetVoxelPack[map[x, y, z]].GetTextureID(f));
                                        _destroyChunk = false;
                                    }
                                }
                            }
                            catch (System.Exception)
                            {
                                if (map[(int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z] != 0)
                                {
                                    AddFace(meshContainer, voxelPos, f, VoxelManager.GetVoxelPack[map[x, y, z]].GetTextureID(f));
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
        private void AddFace(MeshContainer _meshContainer, Vector3 _voxelPos, int _faceIndex, int _textureID)
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
        private void AddTexture(MeshContainer _meshContainer, int _textureID)
        {
            float y = _textureID / Settings.textureAtlasSize;
            float x = _textureID - (y * Settings.textureAtlasSize);

            x *= Settings.textureNormalizedSize;
            y *= Settings.textureNormalizedSize;

            y = 1f - y - Settings.textureNormalizedSize;

            _meshContainer.uvs.Add(new Vector2(x, y));
            _meshContainer.uvs.Add(new Vector2(x, y + Settings.textureNormalizedSize));
            _meshContainer.uvs.Add(new Vector2(x + Settings.textureNormalizedSize, y));
            _meshContainer.uvs.Add(new Vector2(x + Settings.textureNormalizedSize, y + Settings.textureNormalizedSize));
        }

        // Get voxel type with position
        public byte GetVoxelType(Vector3Int _position)
        {
            try
            {
                return map[_position.x, _position.y, _position.z];
            }
            catch (System.Exception)
            {
                return world.GetVoxelType(_position);
            }
        }

        // Checks if there is a solid voxel at that position.
        public bool ExistsVoxel(Vector3Int _position)
        {
            Vector3Int pos = _position - position;

            try
            {
                return VoxelManager.GetVoxelPack[map[pos.x, pos.y, pos.z]].solid;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        // Check if a block is inside chunk
        public bool IsVoxelInChunk(Vector3Int _position)
        {
            Vector3 pos = _position - position;

            return !(
                pos.x < 0 || pos.x > Settings.chunkSize.x - 1 ||
                pos.y < 0 || pos.y > Settings.chunkSize.y - 1 ||
                pos.z < 0 || pos.z > Settings.chunkSize.z - 1
            );
        }
    }
}