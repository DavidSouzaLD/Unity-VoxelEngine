using UnityEngine;
using TMPro;

namespace Game.Player
{
    public class PlayerInformations : MonoBehaviour
    {
        private TextMeshProUGUI textInfo;
        private PlayerController playerController;
        private PlayerBuilder playerBuilder;

        private void Start()
        {
            textInfo = GameObject.Find("Informations").GetComponent<TextMeshProUGUI>();
            playerController = GameObject.FindObjectOfType<PlayerController>();
            playerBuilder = GameObject.FindObjectOfType<PlayerBuilder>();
        }

        private void LateUpdate()
        {
            string playerPosition = "Position (" + playerController.transform.position.x.ToString("0") + "," + playerController.transform.position.y.ToString("0") + "," + playerController.transform.position.z.ToString("0") + ")";
            string viewPosition = "View Position (" + playerBuilder.viewPosition.x + ", " + playerBuilder.viewPosition.y + ", " + playerBuilder.viewPosition.z + ")";
            string voxelSelected = "Voxel Selected (" + playerBuilder.GetVoxelWithByte(playerBuilder.type).name + ")";

            textInfo.text = playerPosition + "\n" + viewPosition + "\n" + voxelSelected;
        }
    }
}