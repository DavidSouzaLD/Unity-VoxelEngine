using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine.Core
{
    public static class VoxelSerializer
    {
        [System.Serializable]
        public class VoxelData
        {
            public int x;
            public int y;
            public int z;
            public byte type;

            public VoxelData(Vector3Int _position, byte _type)
            {
                x = _position.x;
                y = _position.y;
                z = _position.z;

                type = _type;
            }

            public Vector3Int position
            {
                get
                {
                    return new Vector3Int(x, y, z);
                }
            }
        }

        [System.Serializable]
        public class Build
        {
            public List<VoxelData> datas = new List<VoxelData>();
        }

        public static Build GetBuild(string _name)
        {
            string path = (Settings.buildPath + _name);
            string test = File.ReadAllText(path);
            return JsonUtility.FromJson<Build>(test);
        }
    }
}