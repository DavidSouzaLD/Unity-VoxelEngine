using System.IO;
using UnityEngine;

public class VoxelEngine : StaticInstance<VoxelEngine>
{
    [Header("Settings")]
    [SerializeField] private Material atlasMaterial; // Material like all blocks as texture.

    [Header("Voxels")]
    [SerializeField] private string voxelsPath; // Location where all voxel information is found.
    [SerializeField] private Color colorReference; // Color reference for creating new voxels.
    [SerializeField] private VoxelPack voxelPack; // Voxel class imported into Unity from JSON.

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

    [Header("Color")]
    [SerializeField] private byte red;
    [SerializeField] private byte green;
    [SerializeField] private byte blue;

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

    public Color GetColor()
    {
        return new Color32(red, green, blue, 1);
    }
}