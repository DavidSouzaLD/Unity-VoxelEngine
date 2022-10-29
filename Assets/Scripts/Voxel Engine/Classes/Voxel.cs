using UnityEngine;

namespace VoxelEngine.Classes
{
    [System.Serializable]
    public class VoxelPack
    {
        public Voxel[] Voxels;
    }

    [System.Serializable]
    public class Voxel
    {
        // Infos
        public string name = "Default";
        public bool solid = true;

        // Texture Values
        private int back;
        private int front;
        private int top;
        private int bottom;
        private int left;
        private int right;

        // Color Values
        private byte red;
        private byte green;
        private byte blue;

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
}