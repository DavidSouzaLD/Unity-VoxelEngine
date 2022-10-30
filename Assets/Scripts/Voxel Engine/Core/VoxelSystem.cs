using System.IO;
using UnityEngine;
using VoxelEngine.Core.Classes;

namespace VoxelEngine.Core
{
    public class VoxelSystem : StaticInstance<VoxelSystem>
    {
        [Header("Settings")]
        [SerializeField] private Material atlasMaterial; // Material like all blocks as texture.

        // Voxel class imported into Unity from JSON.
        VoxelPack voxelPack;

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
            string test = File.ReadAllText(Settings.voxelPackPath);
            Instance.voxelPack = JsonUtility.FromJson<VoxelPack>(test);
        }
    }
}