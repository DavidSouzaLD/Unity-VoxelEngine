using System.IO;
using UnityEngine;

public class VoxelEngine : StaticInstance<VoxelEngine>
{
    [Header("Settings")]
    [SerializeField] private Material atlasMaterial;
    [SerializeField] private string voxelsPath;
    [SerializeField] private VoxelPack voxelPack;

    public static Material AtlasMaterial
    {
        get
        {
            return Instance.atlasMaterial;
        }
    }

    public static Voxel[] GetVoxelPack
    {
        get
        {
            return Instance.voxelPack.Voxels;
        }
    }

    public override void Awake()
    {
        base.Awake();

        // Get VoxelPack
        string test = File.ReadAllText(Instance.voxelsPath + "/VoxelPack.cfg");
        Instance.voxelPack = JsonUtility.FromJson<VoxelPack>(test);
    }
}

[System.Serializable]
public class VoxelPack
{
    public Voxel[] Voxels;
}

[System.Serializable]
public class Voxel
{
    public string name = "Default";
    public bool isSolid = true;

    [Header("Texture Values")]
    [SerializeField] private int back;
    [SerializeField] private int front;
    [SerializeField] private int top;
    [SerializeField] private int bottom;
    [SerializeField] private int left;
    [SerializeField] private int right;

    public int GetTextureID(int _faceID)
    {
        switch (_faceID)
        {
            case 0: return back;
            case 1: return front;
            case 2: return top;
            case 3: return bottom;
            case 4: return left;
            case 5: return right;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }
}