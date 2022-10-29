using UnityEngine;
using VoxelEngine.Extras;

public class StartIsland : MonoBehaviour
{
    bool isSpawned = false;

    private void Start()
    {
        if (!isSpawned)
        {
            VoxelTemplate.CreatePlane(new Vector3Int(0, 0, 0), 1, 100);
            VoxelTemplate.CreatePlane(new Vector3Int(0, -1, 0), 1, 8);
            VoxelTemplate.CreatePlane(new Vector3Int(0, -2, 0), 1, 6);
            VoxelTemplate.CreatePlane(new Vector3Int(0, -3, 0), 1, 4);
            VoxelTemplate.CreatePlane(new Vector3Int(0, -4, 0), 1, 2);
            isSpawned = true;
        }
    }
}