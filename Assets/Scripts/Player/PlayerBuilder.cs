using UnityEngine;

namespace Game.Player
{
    public class PlayerBuilder : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float distance;
        [SerializeField] private byte type;

        [Header("Components")]
        [SerializeField] private VoxelWorld voxelWorld;
        [SerializeField] private Transform m_camera;
        [SerializeField] private Transform highlightedBlock;

        private void Update()
        {
            UpdateRaycast();
        }

        private void UpdateRaycast()
        {
            RaycastHit hit;

            if (Physics.Raycast(m_camera.position, m_camera.forward, out hit, distance, layerMask))
            {
                highlightedBlock.gameObject.SetActive(true);

                highlightedBlock.position = new Vector3(
                    Mathf.RoundToInt(hit.point.x - (hit.normal.x * 0.5f)),
                    Mathf.RoundToInt(hit.point.y - (hit.normal.y * 0.5f)),
                    Mathf.RoundToInt(hit.point.z - (hit.normal.z * 0.5f))
                );

                Vector3 placePosition = new Vector3(
                    Mathf.RoundToInt(hit.point.x + (hit.normal.x * 0.5f)),
                    Mathf.RoundToInt(hit.point.y + (hit.normal.y * 0.5f)),
                    Mathf.RoundToInt(hit.point.z + (hit.normal.z * 0.5f))
                );

                if (Input.GetKeyDown(PlayerKeys.PlaceVoxel))
                {
                    voxelWorld.EditVoxel(placePosition.ToVector3Int(), type);
                }

                if (Input.GetKeyDown(PlayerKeys.DestroyVoxel))
                {
                    voxelWorld.EditVoxel(highlightedBlock.position.ToVector3Int(), 0);
                }
            }
            else
            {
                highlightedBlock.gameObject.SetActive(false);
            }
        }
    }
}