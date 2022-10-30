using System.IO;
using UnityEngine;
using Game.Player.Others;
using VoxelEngine.Core;
using VoxelEngine.Core.Classes;
using VoxelEngine.Extras;
using TMPro;

namespace Game.Player
{
    public class PlayerEdit : MonoBehaviour
    {
        [Header("Move Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float sensitivity = 3f;

        [Header("Ray")]
        [SerializeField] private VoxelEdit voxelEdit;
        [SerializeField] private byte type;
        [SerializeField] private float distance;
        [SerializeField] private Highlight highlightPrefab;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI textUI;

        Vector2 _mouseRotation = Vector2.zero;
        float rotTransformY;
        float rotCameraX;
        private Highlight highlight;

        // Voxel class imported into Unity from JSON.
        VoxelPack voxelPack;

        private void Awake()
        {
            // Get VoxelPack
            string test = File.ReadAllText(Settings.voxelPackPath);
            voxelPack = JsonUtility.FromJson<VoxelPack>(test);
        }

        private void Start()
        {
            // Setting values
            _mouseRotation.y = rotTransformY;
            _mouseRotation.x = rotCameraX;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // Mouse Look
            Vector2 _cameraAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); // Easy to implement new input system

            _mouseRotation.x += _cameraAxis.x * sensitivity;
            _mouseRotation.y += -_cameraAxis.y * sensitivity;
            _mouseRotation.y = Mathf.Clamp(_mouseRotation.y, -90, 90); // Limit vertical angle

            // Rotate body to horizontal
            transform.localRotation = Quaternion.Euler(
                _mouseRotation.y,
                _mouseRotation.x,
                transform.localRotation.z);

            Vector2 axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            Vector3 direction = transform.forward * axis.y + transform.right * axis.x;

            transform.position += direction * moveSpeed * Time.deltaTime;

            UpdateRaycast();
            UpdateScroll();
        }

        private void UpdateRaycast()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
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

                Debug.Log(highlight.transform.position);


                Vector3 placePosition = new Vector3(
                    Mathf.RoundToInt(hit.point.x + (hit.normal.x * 0.5f)),
                    Mathf.RoundToInt(hit.point.y + (hit.normal.y * 0.5f)),
                    Mathf.RoundToInt(hit.point.z + (hit.normal.z * 0.5f))
                );

                CheckDirections(hit.normal.ToVector3Int());

                if (Input.GetKeyDown(PlayerKeys.PlaceVoxel))
                {
                    voxelEdit.AddVoxel(placePosition.ToVector3Int(), type);
                }

                if (Input.GetKeyDown(PlayerKeys.DestroyVoxel))
                {
                    voxelEdit.RemoveVoxel(highlight.transform.position.ToVector3Int());
                }

                textUI.text = "CurrentBlock: " + voxelPack.Voxels[type].name;
            }
            else
            {
                if (highlight != null)
                {
                    Destroy(highlight.gameObject);
                }
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