using UnityEngine;

namespace Game.Player
{
    public class PlayerKeys : Singleton<PlayerKeys>
    {
        [Header("Keys")]
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode placeVoxel = KeyCode.Mouse1;
        [SerializeField] private KeyCode destroyVoxel = KeyCode.Mouse0;

        // Static
        public static KeyCode Run { get { return Instance.runKey; } }
        public static KeyCode Jump { get { return Instance.jumpKey; } }
        public static KeyCode Crouch { get { return Instance.crouchKey; } }
        public static KeyCode PlaceVoxel { get { return Instance.placeVoxel; } }
        public static KeyCode DestroyVoxel { get { return Instance.destroyVoxel; } }
    }
}