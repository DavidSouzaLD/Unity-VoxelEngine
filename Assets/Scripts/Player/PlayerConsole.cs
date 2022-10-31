using UnityEngine;

namespace Game.Player
{
    public class PlayerConsole : MonoBehaviour
    {
        public GameObject console;
        public bool consoleEnabled { get; private set; }

        private void Start()
        {
            console.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(PlayerKeys.OpenDebug))
            {
                consoleEnabled = !consoleEnabled;

                if (consoleEnabled)
                {
                    console.SetActive(true);
                }
                else
                {
                    console.SetActive(false);
                }
            }

        }
    }
}