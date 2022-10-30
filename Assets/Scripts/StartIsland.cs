using UnityEngine;
using VoxelEngine.Extras;

public class StartIsland : MonoBehaviour
{
    bool isSpawned = false;

    private void Start()
    {
        VoxelTemplate.CreateBuild(Vector3Int.zero, "Island");
        VoxelTemplate.CreateBuild(new Vector3Int(7, 0, 8), "Island");
        VoxelTemplate.CreateBuild(new Vector3Int(-7, 0, -8), "Island2");
        Destroy(this);
    }
}