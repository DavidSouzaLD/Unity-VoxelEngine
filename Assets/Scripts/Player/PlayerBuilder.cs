using UnityEngine;
using UnityEngine.Events;
using Game.Player.Others;
using VoxelEngine.Core;
using VoxelEngine.Core.Classes;

namespace Game.Player
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerConsole))]
    public class PlayerBuilder : MonoBehaviour
    {

        [Header("Settings")]
        public Highlight highlightPrefab;
        public LayerMask layerMask;
        public float distance;

        [Header("Destroy")]
        public GameObject destroyPrefab;
        public ParticleSystem destroyEffect;
        public Material destroyMaterial;
        public Sprite[] destroySprites;

        [Header("Events")]
        public UnityEvent onPlaceVoxel;
        public UnityEvent onDestroyVoxel;

        public byte type { get; private set; }
        public Vector3 viewPosition { get; private set; }

        // Privates
        private float destroyTimer;
        private Vector3Int currentDestroyPos;

        // Components
        private Transform m_camera;
        private GameObject destroy;
        private Highlight highlight;
        private VoxelWorld voxelWorld;
        private PlayerConsole playerConsole;
        private PlayerController playerController;

        private void Start()
        {
            // Get components
            m_camera = GetComponentInChildren<Camera>().transform;
            voxelWorld = GameObject.FindObjectOfType<VoxelWorld>();
            playerController = GetComponent<PlayerController>();
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

                    Vector3Int destroyPosition = new Vector3Int(
                        Mathf.RoundToInt(hit.point.x - (hit.normal.x * 0.5f)),
                        Mathf.RoundToInt(hit.point.y - (hit.normal.y * 0.5f)),
                        Mathf.RoundToInt(hit.point.z - (hit.normal.z * 0.5f))
                    );

                    Vector3Int placePosition = new Vector3Int(
                        Mathf.RoundToInt(hit.point.x + (hit.normal.x * 0.5f)),
                        Mathf.RoundToInt(hit.point.y + (hit.normal.y * 0.5f)),
                        Mathf.RoundToInt(hit.point.z + (hit.normal.z * 0.5f))
                    );

                    highlight.transform.position = destroyPosition;

                    // Setting view position
                    viewPosition = destroyPosition;

                    // Checking directions
                    CheckDirections(hit.normal.ToVector3Int());

                    if (Input.GetKeyDown(PlayerKeys.PlaceVoxel))
                    {
                        // Setting voxel
                        voxelWorld.EditVoxel(placePosition, type);

                        // Apply event
                        onPlaceVoxel.Invoke();
                    }

                    if (playerController.creativeMode ?
                        Input.GetKeyDown(PlayerKeys.DestroyVoxel) :
                        Input.GetKey(PlayerKeys.DestroyVoxel))
                    {
                        if (playerController.creativeMode)
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
                        else
                        {
                            if (currentDestroyPos == destroyPosition)
                            {
                                if (destroy == null)
                                {
                                    destroy = Instantiate(destroyPrefab, Vector3.zero, Quaternion.identity);
                                }

                                // Get voxel
                                byte voxelType = voxelWorld.GetVoxelType(destroyPosition);
                                Voxel currentVoxel = VoxelSystem.GetVoxelPack[voxelType];

                                // Material
                                float indexTimer = destroyTimer / currentVoxel.timeToDestroy;
                                int matIndex = Mathf.RoundToInt(indexTimer * destroySprites.Length);

                                if (matIndex < 0)
                                {
                                    matIndex = 0;
                                }

                                if (matIndex > destroySprites.Length - 1)
                                {
                                    matIndex = destroySprites.Length - 1;
                                }

                                destroyMaterial.mainTexture = destroySprites[matIndex].texture;

                                // Position
                                destroy.transform.position = destroyPosition;

                                // Time
                                destroyTimer += Time.deltaTime;

                                // Destroy voxel
                                if (destroyTimer >= currentVoxel.timeToDestroy)
                                {
                                    // Destroy effet
                                    byte type = voxelWorld.GetVoxelType(highlight.transform.position.ToVector3Int());
                                    ParticleSystem effect = Instantiate(destroyEffect, highlight.transform.position.ToVector3Int(), Quaternion.identity);
                                    ParticleSystem.MainModule ps = effect.main;
                                    ps.startColor = VoxelSystem.GetVoxelPack[type].GetColor();

                                    // Setting the voxel like air
                                    voxelWorld.EditVoxel(destroyPosition, 0);

                                    // Apply event
                                    onDestroyVoxel.Invoke();

                                    // Reset
                                    ResetDestroyValues();
                                }
                            }
                            else
                            {
                                ResetDestroyValues();
                            }
                            currentDestroyPos = destroyPosition;
                        }
                    }
                    else
                    {
                        ResetDestroyValues();
                    }
                }
                else
                {
                    if (highlight != null)
                    {
                        Destroy(highlight.gameObject);
                        ResetDestroyValues();
                    }
                }
            }
        }

        private void ResetDestroyValues()
        {
            destroyTimer = 0;

            if (destroy != null)
            {
                Destroy(destroy);
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

        public Voxel GetVoxelWithByte(byte type)
        {
            return VoxelSystem.GetVoxelPack[type];
        }
    }
}