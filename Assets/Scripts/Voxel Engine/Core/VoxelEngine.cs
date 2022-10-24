using UnityEngine;

public class VoxelEngine : StaticInstance<VoxelEngine>
{
    public Material atlasMaterial;
    public VoxelType[] types;

    public static Material AtlasMaterial
    {
        get
        {
            return Instance.atlasMaterial;
        }
    }

    public static VoxelType[] Types
    {
        get
        {
            return Instance.types;
        }
    }
}

[System.Serializable]
public class VoxelType
{
    public string voxelName = "Default";
    public bool isSolid = true;

    [Header("Texture Values")]
    [SerializeField] private int Back;
    [SerializeField] private int Front;
    [SerializeField] private int Top;
    [SerializeField] private int Bottom;
    [SerializeField] private int Left;
    [SerializeField] private int Right;

    public int GetTextureID(int _faceID)
    {
        switch (_faceID)
        {
            case 0: return Back;
            case 1: return Front;
            case 2: return Top;
            case 3: return Bottom;
            case 4: return Left;
            case 5: return Right;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }
}