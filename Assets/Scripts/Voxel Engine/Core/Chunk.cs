using System.Collections.Generic;
using UnityEngine;

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
        map = new byte[VoxelSettings.chunkSize.x, VoxelSettings.chunkSize.y, VoxelSettings.chunkSize.z];

        // Creating object
        gameObject = new GameObject();
        gameObject.name = "Chunk [X:" + _coord.position.x + " / " + _coord.position.y + " / " + _coord.position.z + "]";
        gameObject.transform.parent = _world.transform;
        gameObject.transform.position = _coord.position;
        gameObject.layer = LayerMask.NameToLayer("Voxel");

        // Others
        meshCollider = gameObject.AddComponent<MeshCollider>();
        material = VoxelEngine.AtlasMaterial;
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
            Vector3Int voxelToCheck = _position + VoxelData.directions[i];

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

        VoxelData.MeshData meshData = new VoxelData.MeshData();

        for (int x = 0; x < VoxelSettings.chunkSize.x; x++)
        {
            for (int y = 0; y < VoxelSettings.chunkSize.y; y++)
            {
                for (int z = 0; z < VoxelSettings.chunkSize.z; z++)
                {
                    Vector3 voxelPos = new Vector3(x, y, z);

                    for (int f = 0; f < 6; f++)
                    {
                        Vector3 voxelToCheck = voxelPos + VoxelData.directions[f];

                        try
                        {
                            if (!VoxelEngine.GetVoxelPack[map[(int)voxelToCheck.x, (int)voxelToCheck.y, (int)voxelToCheck.z]].isSolid)
                            {
                                if (map[(int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z] != 0)
                                {
                                    AddFace(meshData, voxelPos, f, VoxelEngine.GetVoxelPack[map[x, y, z]].GetTextureID(f));
                                    _destroyChunk = false;
                                }
                            }
                        }
                        catch (System.Exception)
                        {
                            if (map[(int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z] != 0)
                            {
                                AddFace(meshData, voxelPos, f, VoxelEngine.GetVoxelPack[map[x, y, z]].GetTextureID(f));
                                _destroyChunk = false;
                            }
                        }
                    }
                }
            }
        }

        // Create mesh
        Mesh mesh1 = new Mesh();
        mesh1.vertices = meshData.vertices.ToArray();
        mesh1.triangles = meshData.triangles.ToArray();
        mesh1.uv = meshData.uvs.ToArray();

        mesh1.RecalculateBounds();
        mesh1.RecalculateTangents();
        mesh1.RecalculateNormals();

        return mesh1;
    }

    // Adds a face in the desired position and direction.
    private void AddFace(VoxelData.MeshData _meshData, Vector3 _voxelPos, int _faceIndex, int _textureID)
    {
        // Vertices
        _meshData.vertices.Add(_voxelPos - VoxelData.fixedPos + VoxelData.vertices[VoxelData.triangles[_faceIndex, 0]]);
        _meshData.vertices.Add(_voxelPos - VoxelData.fixedPos + VoxelData.vertices[VoxelData.triangles[_faceIndex, 1]]);
        _meshData.vertices.Add(_voxelPos - VoxelData.fixedPos + VoxelData.vertices[VoxelData.triangles[_faceIndex, 2]]);
        _meshData.vertices.Add(_voxelPos - VoxelData.fixedPos + VoxelData.vertices[VoxelData.triangles[_faceIndex, 3]]);

        // UVS
        AddTexture(_meshData, _textureID);

        // Triangles
        _meshData.triangles.Add(_meshData.vertexIndex);
        _meshData.triangles.Add(_meshData.vertexIndex + 1);
        _meshData.triangles.Add(_meshData.vertexIndex + 2);
        _meshData.triangles.Add(_meshData.vertexIndex + 2);
        _meshData.triangles.Add(_meshData.vertexIndex + 1);
        _meshData.triangles.Add(_meshData.vertexIndex + 3);
        _meshData.vertexIndex += 4;
    }

    // Reworks the UV map based on the Atlas map.
    private void AddTexture(VoxelData.MeshData _meshData, int _textureID)
    {
        float y = _textureID / VoxelSettings.textureAtlasSize;
        float x = _textureID - (y * VoxelSettings.textureAtlasSize);

        x *= VoxelSettings.textureNormalizedSize;
        y *= VoxelSettings.textureNormalizedSize;

        y = 1f - y - VoxelSettings.textureNormalizedSize;

        _meshData.uvs.Add(new Vector2(x, y));
        _meshData.uvs.Add(new Vector2(x, y + VoxelSettings.textureNormalizedSize));
        _meshData.uvs.Add(new Vector2(x + VoxelSettings.textureNormalizedSize, y));
        _meshData.uvs.Add(new Vector2(x + VoxelSettings.textureNormalizedSize, y + VoxelSettings.textureNormalizedSize));
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
            return VoxelEngine.GetVoxelPack[map[pos.x, pos.y, pos.z]].isSolid;
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
            pos.x < 0 || pos.x > VoxelSettings.chunkSize.x - 1 ||
            pos.y < 0 || pos.y > VoxelSettings.chunkSize.y - 1 ||
            pos.z < 0 || pos.z > VoxelSettings.chunkSize.z - 1
        );
    }
}