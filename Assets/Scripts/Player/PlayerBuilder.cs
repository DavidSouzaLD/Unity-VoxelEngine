using UnityEngine;
using UnityEngine.Events;
using Game.Player.Others;
using VoxelEngine.Core;

namespace Game.Player
{
    [RequireComponent(typeof(PlayerConsole))]
    public class PlayerBuilder : MonoBehaviour
    {
        [Header("Settings")]
        public Highlight highlightPrefab;
        public LayerMask layerMask;
        public float distance;

        [Header("Effects")]
        public ParticleSystem destroyEffect;

        [Header("Events")]
        public UnityEvent onPlaceVoxel;
        public UnityEvent onDestroyVoxel;

        // Privates
        private byte type;
        private Transform m_camera;

        // Components
        private Highlight highlight;
        private VoxelWorld voxelWorld;
        private PlayerConsole playerConsole;

        private void Start()
        {
            // Get components
            voxelWorld = GameObject.FindObjectOfType<VoxelWorld>();
            m_camera = GetComponentInChildren<Camera>().transform;
            playerConsole = GetComponent<PlayerConsole>();
        }

        private void Update()
        {
            UpdateRaycast();
            UpdateScroll();
        }

        private void UpdateRaycast()
        {
            bool buildConditions = !playerConsole.consoleEnabled;

            if (buildConditions)
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
                        ps.startColor = VoxelSystem.GetVoxelPack[type].GetColor();

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

            if (Input.mouseScrollDelta.y > 0 && type < VoxelSystem.GetVoxelPack.Length - 1)
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