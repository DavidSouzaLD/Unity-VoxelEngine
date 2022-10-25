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
    public Vector3 position { get { return coord.position; } }

    // Mesh informations.
    private int vertexIndex = 0;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

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

    // Edit the voxel map to change the mesh.
    public void EditMap(Vector3Int _position, byte _type, bool _updateChunk = true)
    {
        Vector3Int pos = _position - position.ToVector3Int();
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

        ClearMesh();

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
                                    AddFace(voxelPos, f, VoxelEngine.GetVoxelPack[map[x, y, z]].GetTextureID(f));
                                    _destroyChunk = false;
                                }
                            }
                        }
                        catch (System.Exception)
                        {
                            if (map[(int)voxelPos.x, (int)voxelPos.y, (int)voxelPos.z] != 0)
                            {
                                AddFace(voxelPos, f, VoxelEngine.GetVoxelPack[map[x, y, z]].GetTextureID(f));
                                _destroyChunk = false;
                            }
                        }
                    }
                }
            }
        }

        // Create mesh
        Mesh mesh1 = new Mesh();
        mesh1.vertices = vertices.ToArray();
        mesh1.triangles = triangles.ToArray();
        mesh1.uv = uvs.ToArray();

        mesh1.RecalculateBounds();
        mesh1.RecalculateTangents();
        mesh1.RecalculateNormals();

        return mesh1;
    }

    // Clear mesh informations.
    private void ClearMesh()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    // Adds a face in the desired position and direction.
    private void AddFace(Vector3 _voxelPos, int _faceIndex, int _textureID)
    {
        // Vertices
        vertices.Add(_voxelPos - VoxelData.fixedPos + VoxelData.vertices[VoxelData.triangles[_faceIndex, 0]]);
        vertices.Add(_voxelPos - VoxelData.fixedPos + VoxelData.vertices[VoxelData.triangles[_faceIndex, 1]]);
        vertices.Add(_voxelPos - VoxelData.fixedPos + VoxelData.vertices[VoxelData.triangles[_faceIndex, 2]]);
        vertices.Add(_voxelPos - VoxelData.fixedPos + VoxelData.vertices[VoxelData.triangles[_faceIndex, 3]]);

        // UVS
        AddTexture(_textureID);

        // Triangles
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 3);
        vertexIndex += 4;
    }

    // Reworks the UV map based on the Atlas map.
    private void AddTexture(int _textureID)
    {
        float y = _textureID / VoxelSettings.textureAtlasSize;
        float x = _textureID - (y * VoxelSettings.textureAtlasSize);

        x *= VoxelSettings.textureNomalizedSize;
        y *= VoxelSettings.textureNomalizedSize;

        y = 1f - y - VoxelSettings.textureNomalizedSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelSettings.textureNomalizedSize));
        uvs.Add(new Vector2(x + VoxelSettings.textureNomalizedSize, y));
        uvs.Add(new Vector2(x + VoxelSettings.textureNomalizedSize, y + VoxelSettings.textureNomalizedSize));
    }

    // Checks if there is a solid voxel at that position.
    public bool ExistsVoxel(Vector3 _position)
    {
        Vector3Int pos = _position.ToVector3Int() - position.ToVector3Int();

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
    private bool IsVoxelInChunk(Vector3Int _position)
    {
        return !(
            _position.x < 0 || _position.x > VoxelSettings.chunkSize.x - 1 ||
            _position.y < 0 || _position.y > VoxelSettings.chunkSize.y - 1 ||
            _position.z < 0 || _position.z > VoxelSettings.chunkSize.z - 1
        );
    }
}