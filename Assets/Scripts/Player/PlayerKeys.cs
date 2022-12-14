using UnityEngine;

namespace Game.Player
{
    public class PlayerKeys : Singleton<PlayerKeys>
    {
        [Header("Keys")]
        [SerializeField] private KeyCode downKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode placeVoxel = KeyCode.Mouse1;
        [SerializeField] private KeyCode destroyVoxel = KeyCode.Mouse0;
        [SerializeField] private KeyCode openDebug = KeyCode.BackQuote;

        // Static
        public static KeyCode Down { get { return Instance.downKey; } }
        public static KeyCode Run { get { return Instance.runKey; } }
        public static KeyCode Jump { get { return Instance.jumpKey; } }
        public static KeyCode Crouch { get { return Instance.crouchKey; } }
        public static KeyCode PlaceVoxel { get { return Instance.placeVoxel; } }
        public static KeyCode DestroyVoxel { get { return Instance.destroyVoxel; } }
        public static KeyCode OpenDebug { get { return Instance.openDebug; } }
    }
}