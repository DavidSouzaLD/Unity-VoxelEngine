using System.IO;
using UnityEngine;
using VoxelEngine.Classes;

namespace VoxelEngine.Core
{
    public class VoxelManager : StaticInstance<VoxelManager>
    {
        [Header("Settings")]
        // Material like all blocks as texture.
        [SerializeField] private Material atlasMaterial;

        [Header("Voxels")]
        // Location where all voxel information is found.
        [SerializeField] private string voxelsPath;

        // Color reference for creating new voxels.
        [SerializeField] private Color colorReference;

        // Voxel class imported into Unity from JSON.
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
}