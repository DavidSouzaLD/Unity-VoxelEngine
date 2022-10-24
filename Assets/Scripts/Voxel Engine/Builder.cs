using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class Builder : MonoBehaviour
    {
        public byte type;
        public LayerMask layerMask;
        public VoxelWorld voxelWorld;
        public Transform highlightedBlock;
        public Transform m_camera;
        public float distance;

        void Update()
        {
            RaycastHit hit;

            if (Physics.Raycast(m_camera.position, m_camera.forward, out hit, distance, layerMask))
            {
                highlightedBlock.gameObject.SetActive(true);

                highlightedBlock.position = new Vector3(
                    Mathf.RoundToInt(hit.point.x + (hit.normal.x * 0.5f)),
                    Mathf.RoundToInt(hit.point.y + (hit.normal.y * 0.5f)),
                    Mathf.RoundToInt(hit.point.z + (hit.normal.z * 0.5f))
                );

                Vector3 destroyPosition = new Vector3(
                    Mathf.RoundToInt(hit.point.x - (hit.normal.x * 0.5f)),
                    Mathf.RoundToInt(hit.point.y - (hit.normal.y * 0.5f)),
                    Mathf.RoundToInt(hit.point.z - (hit.normal.z * 0.5f))
                );

                if (Systems.Input.GetBool("PlaceVoxel"))
                {
                    voxelWorld.EditVoxel(highlightedBlock.position.ToVector3Int(), type);
                }

                if (Systems.Input.GetBool("DestroyVoxel"))
                {
                    voxelWorld.EditVoxel(destroyPosition.ToVector3Int(), 0);
                }
            }
            else
            {
                highlightedBlock.gameObject.SetActive(false);
            }

            if (Input.GetKey(KeyCode.Alpha1))
            {
                type = 1;
            }

            if (Input.GetKey(KeyCode.Alpha2))
            {
                type = 2;
            }

            if (Input.GetKey(KeyCode.Alpha3))
            {
                type = 3;
            }

            if (Input.GetKey(KeyCode.Alpha4))
            {
                type = 4;
            }

            if (Input.GetKey(KeyCode.Alpha5))
            {
                type = 5;
            }
        }
    }
}