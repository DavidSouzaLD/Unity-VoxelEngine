using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Manages all systems search by other scripts.
    /// </summary>
    [DisallowMultipleComponent]
    public class Systems : PersistentSingleton<Systems>
    {
        private Input _Input;
        private Audio _Audio;

        public static Input Input { get { return Instance._Input; } private set { } }
        public static Audio Audio { get { return Instance._Audio; } private set { } }

        private void Start()
        {
            _Input = GetComponentInChildren<Input>();
            _Audio = GetComponentInChildren<Audio>();
        }
    }
}