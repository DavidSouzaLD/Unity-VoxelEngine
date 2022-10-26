using UnityEngine;
using TMPro;

public class DebugInfos : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public Transform player;
    public Vector3 viewPosition;

    private void LateUpdate()
    {
        string playerPos = "Position in World (" + player.position.x.ToString("0") + ", " + player.position.y.ToString("0") + ", " + player.position.z.ToString("0") + ")\n";
        string viewPos = "View position (" + viewPosition.x + ", " + viewPosition.y + ", " + viewPosition.z + ")";
        textMesh.text = playerPos + viewPos;
    }
}
