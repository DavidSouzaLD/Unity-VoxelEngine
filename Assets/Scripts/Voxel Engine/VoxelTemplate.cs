using System.Collections.Generic;
using UnityEngine;

public class VoxelTemplate : MonoBehaviour
{
    public static void CreatePlane(VoxelWorld _world, Vector3Int _position, byte _type, Vector2Int _size)
    {
        List<Chunk> chunksToUpdate = new List<Chunk>();

        for (int x = -(int)_size.x / 2; x < _size.x / 2; x++)
        {
            for (int z = -(int)_size.y / 2; z < _size.y / 2; z++)
            {
                Vector3Int pos = _position + new Vector3Int(x, 0, z);
                _world.EditVoxel(pos, _type, false);

                Chunk chunk = _world.GetChunk(pos);

                if (chunk != null)
                {
                    chunksToUpdate.Add(chunk);
                }
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++)
        {
            chunksToUpdate[i].Update();
        }
    }

    public static void CreateCube(VoxelWorld _world, Vector3Int _position, byte _type, Vector3Int _size)
    {
        int count = (int)VoxelSettings.chunkSize.magnitude;
        int c = 0;
        Debug.Log(count);

        for (int x = -_size.x / 2; x < _size.x / 2; x++)
        {
            for (int y = -_size.y / 2; y < _size.y / 2; y++)
            {
                for (int z = -_size.z / 2; z < _size.z / 2; z++)
                {
                    c++;

                    Vector3Int pos = _position + new Vector3Int(x, y, z);

                    _world.EditVoxel(pos, _type, false);

                    if (c > count)
                    {
                        _world.GetChunk(pos).Update();
                        c = 0;
                    }
                }
            }
        }
    }
}