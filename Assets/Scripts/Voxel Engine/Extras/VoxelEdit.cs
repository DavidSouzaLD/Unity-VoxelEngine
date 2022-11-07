using System.IO;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine.Core;

namespace VoxelEngine.Extras
{
    public class VoxelEdit : MonoBehaviour
    {
        [Header("VoxelSystem")]
        public GameObject prefab;
        public string nameObject = "Default";

        [Header("Serialize")]
        public bool save;
        public bool load;

        VoxelSerializer.Build build = new VoxelSerializer.Build();
        List<GameObject> voxelObjects = new List<GameObject>();

        private void Start()
        {
            AddVoxel(Vector3Int.zero, 1);
        }

        private void Update()
        {
            if (save)
            {
                string value = JsonUtility.ToJson(build);
                System.IO.File.WriteAllText(VoxelSystem.GetBuildsPath + nameObject + ".build", value);

                save = false;
            }

            if (load)
            {
                if (build != null)
                {
                    for (int i = 0; i < build.datas.Count; i++)
                    {
                        Destroy(voxelObjects[i]);
                        voxelObjects.RemoveAt(i);
                        build.datas.RemoveAt(i);
                    }
                }

                string test = File.ReadAllText(VoxelSystem.GetBuildsPath + nameObject + ".build");
                build = JsonUtility.FromJson<VoxelSerializer.Build>(test);

                if (build != null)
                {
                    for (int i = 0; i < build.datas.Count; i++)
                    {
                        AddVoxel(build.datas[i].position, build.datas[i].type);
                    }
                }

                load = false;
            }
        }

        public void AddVoxel(Vector3Int _pos, byte _type)
        {
            // Create data
            GameObject obj = Instantiate(prefab, _pos, Quaternion.identity);
            obj.transform.parent = transform;

            VoxelSerializer.VoxelData data = new VoxelSerializer.VoxelData(_pos, _type);

            // Adding data to list
            if (!build.datas.Contains(data))
            {
                build.datas.Add(data);
                voxelObjects.Add(obj);
            }
        }

        public void RemoveVoxel(Vector3Int _pos)
        {
            for (int i = 0; i < build.datas.Count; i++)
            {
                if (build.datas[i].position == _pos)
                {
                    Destroy(voxelObjects[i]);
                    voxelObjects.RemoveAt(i);
                    build.datas.RemoveAt(i);
                }
            }
        }
    }
}