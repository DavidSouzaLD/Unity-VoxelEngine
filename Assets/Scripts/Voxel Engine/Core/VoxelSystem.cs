using System.IO;
using UnityEngine;
using VoxelEngine.Core.Classes;

namespace VoxelEngine.Core
{
    public class VoxelSystem : MonoBehaviour
    {
        public static VoxelSystem Instance;

        [Header("World Settings")]
        [SerializeField] private Vector3Int chunkSize = new Vector3Int(15, 55, 15);
        [SerializeField] private Vector3Int worldSize = new Vector3Int(24, 5, 24);
        [SerializeField] private int textureAtlasSize = 8;

        [Header("Materials")]
        [SerializeField] private Material atlasMaterial;

        [Header("Render View")]
        [SerializeField] private int maxRenderView;

        [Header("Paths")]
        [SerializeField] private string buildsPath;
        [SerializeField] private string voxelsPath;
        private VoxelPack voxelPack;

        public static Material AtlasMaterial
        { get { return Instance.atlasMaterial; } }

        public static Voxel[] GetVoxelPack
        { get { return Instance.voxelPack.Voxels; } }

        public static string GetBuildsPath
        { get { return Instance.buildsPath; } }

        public static string GetVoxelPackPath
        { get { return Instance.voxelsPath; } }

        // Value of blocks by textures
        public static int GetMaxRenderView
        { get { return Instance.maxRenderView; } }

        // Value of blocks by textures
        public static int GetTextureAtlasSize
        { get { return 8; } }

        // Normalized texture value
        public static float GetTextureNormalizedSize
        { get { return 1f / Instance.textureAtlasSize; } }

        // Number of chunks to be generated in the world from the central point
        public static Vector3Int GetWorldSize
        { get { return Instance.worldSize; } }

        // Full chunk size
        public static Vector3Int GetChunkSize
        { get { return Instance.chunkSize; } }

        private void OnValidate()
        {
            // Get Instance
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Awake()
        {
            // Get Instance
            if (Instance == null)
            {
                Instance = this;
            }

            // Get VoxelPack
            string test = File.ReadAllText(GetVoxelPackPath);
            Instance.voxelPack = JsonUtility.FromJson<VoxelPack>(test);
        }
    }
}