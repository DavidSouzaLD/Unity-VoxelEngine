using UnityEngine;
using UnityEngine.Events;
using Game.Player.Others;

namespace Game.Player
{
    public class PlayerBuilder : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float distance;
        [SerializeField] private Highlight highlightPrefab;

        [Header("Effects")]
        [SerializeField] private ParticleSystem destroyEffect;

        [Header("Events")]
        [SerializeField] private UnityEvent onPlaceVoxel;
        [SerializeField] private UnityEvent onDestroyVoxel;

        private byte type;
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
            UpdateScroll();
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

                DebugSystem.AddInfo("ViewPosition", "View Position (" + highlight.transform.position.x + ", " + highlight.transform.position.y + ", " + highlight.transform.position.z + ")");
                CheckDirections(hit.normal.ToVector3Int());

                if (Input.GetKeyDown(PlayerKeys.PlaceVoxel))
                {
                    // Setting voxel
                    voxelWorld.EditVoxel(placePosition.ToVector3Int(), type);

                    // Apply event
                    onPlaceVoxel.Invoke();
                }

                if (Input.GetKeyDown(PlayerKeys.DestroyVoxel))
                {
                    // Destroy effet
                    byte type = voxelWorld.GetVoxelType(highlight.transform.position.ToVector3Int());
                    ParticleSystem effect = Instantiate(destroyEffect, highlight.transform.position.ToVector3Int(), Quaternion.identity);
                    ParticleSystem.MainModule ps = effect.main;
                    ps.startColor = VoxelEngine.GetVoxelPack[type].GetColor();

                    // Setting the voxel like air
                    voxelWorld.EditVoxel(highlight.transform.position.ToVector3Int(), 0);

                    // Apply event
                    onDestroyVoxel.Invoke();
                }
            }
            else
            {
                if (highlight != null)
                {
                    DebugSystem.RemoveInfo("View Position");
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

        private void UpdateScroll()
        {
            if (type < 1)
            {
                type = 1;
            }

            if (Input.mouseScrollDelta.y > 0 && type < VoxelEngine.GetVoxelPack.Length - 1)
            {
                type++;
            }
            else if (Input.mouseScrollDelta.y < 0 && type > 1)
            {
                type--;
            }
        }
    }
}