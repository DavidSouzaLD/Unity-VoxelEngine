using UnityEngine;
using Game.Player.Others;
using VoxelEngine.Extras;

namespace Game.Player
{
    public class PlayerEdit : MonoBehaviour
    {
        [Header("Move Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float sensitivity = 3f;

        [Header("Ray")]
        [SerializeField] private VoxelEdit voxelEdit;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float distance;
        [SerializeField] private Highlight highlightPrefab;

        Vector2 _mouseRotation = Vector2.zero;
        float rotTransformY;
        float rotCameraX;
        private Highlight highlight;

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
            //UpdateScroll();
        }

        private void UpdateRaycast()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, distance, layerMask))
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
                    voxelEdit.AddVoxel(placePosition.ToVector3Int(), 1);
                }

                if (Input.GetKeyDown(PlayerKeys.DestroyVoxel))
                {
                    voxelEdit.RemoveVoxel(highlight.transform.position.ToVector3Int());
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