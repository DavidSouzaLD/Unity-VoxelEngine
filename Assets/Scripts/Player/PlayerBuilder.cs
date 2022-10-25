using UnityEngine;

namespace Game.Player
{
    public class PlayerBuilder : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float distance;
        [SerializeField] private byte type;
        [SerializeField] private Highlight highlightPrefab;

        private Highlight highlight;
        private VoxelWorld voxelWorld;
        private Transform m_camera;

        private void Start()
        {
            voxelWorld = GameObject.FindObjectOfType<VoxelWorld>();
            m_camera = GetComponentInChildren<Camera>().transform;

            if (voxelWorld == null)
            {
                Debug.Log("(PlayerBuilder) VoxelWorld not finded!");
                this.enabled = false;
            }

            if (m_camera == null)
            {
                Debug.Log("(PlayerBuilder) Camera not finded!");
                this.enabled = false;
            }
        }

        private void Update()
        {
            UpdateRaycast();
        }

        private void UpdateRaycast()
        {
            RaycastHit hit;

            if (Physics.Raycast(m_camera.position, m_camera.forward, out hit, distance, layerMask))
            {
                if (highlight == null)
                {
                    highlight = Instantiate(highlightPrefab, Vector3.zero, Quaternion.identity).GetComponent<Highlight>();
                }

                highlight.transform.position = new Vector3(
                    Mathf.RoundToInt(hit.point.x - (hit.normal.x * 0.5f)),
                    Mathf.RoundToInt(hit.point.y - (hit.normal.y * 0.5f)),
                    Mathf.RoundToInt(hit.point.z - (hit.normal.z * 0.5f))
                );

                Vector3 placePosition = new Vector3(
                    Mathf.RoundToInt(hit.point.x + (hit.normal.x * 0.5f)),
                    Mathf.RoundToInt(hit.point.y + (hit.normal.y * 0.5f)),
                    Mathf.RoundToInt(hit.point.z + (hit.normal.z * 0.5f))
                );

                CheckDirections(hit.normal.ToVector3Int());

                if (Input.GetKeyDown(PlayerKeys.PlaceVoxel))
                {
                    voxelWorld.EditVoxel(placePosition.ToVector3Int(), type);
                }

                if (Input.GetKeyDown(PlayerKeys.DestroyVoxel))
                {
                    voxelWorld.EditVoxel(highlight.transform.position.ToVector3Int(), 0);
                }
            }
            else
            {
                if (highlight != null)
                {
                    Destroy(highlight.gameObject);
                }
            }
        }

        private void CheckDirections(Vector3Int _direction)
        {
            if (_direction.y > 0)
            {
                highlight.ApplyDirection(Highlight.Directions.Top);
            }
            else if (_direction.y < 0)
            {
                highlight.ApplyDirection(Highlight.Directions.Bottom);
            }
            else if (_direction.x > 0)
            {
                highlight.ApplyDirection(Highlight.Directions.Right);
            }
            else if (_direction.x < 0)
            {
                highlight.ApplyDirection(Highlight.Directions.Left);
            }
            else if (_direction.z > 0)
            {
                highlight.ApplyDirection(Highlight.Directions.Front);
            }
            else if (_direction.z < 0)
            {
                highlight.ApplyDirection(Highlight.Directions.Back);
            }
        }
    }
}