using UnityEngine;
using VoxelEngine.Extras;

public class StartIsland : MonoBehaviour
{
    bool isSpawned = false;

    private void Start()
    {
        VoxelTemplate.CreateBuild(Vector3Int.zero, "Island");
        Destroy(this);
    }
}