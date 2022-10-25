using UnityEngine;

namespace Game.Player.Others
{
    public class Highlight : MonoBehaviour
    {
        public enum Directions { Top, Bottom, Right, Left, Front, Back }

        [Tooltip("0 - Top, 1 - Bottom, 2 - Right, 3 - Left, 4 - Front, 5 - Back ")]
        public GameObject[] faces = new GameObject[6];
        public Directions directions;

        public void ApplyDirection(Directions _direction)
        {
            directions = _direction;

            for (int i = 0; i < faces.Length; i++)
            {
                faces[i].SetActive(false);
            }

            switch (directions)
            {
                case Directions.Top: faces[0].SetActive(true); break;
                case Directions.Bottom: faces[1].SetActive(true); break;
                case Directions.Right: faces[2].SetActive(true); break;
                case Directions.Left: faces[3].SetActive(true); break;
                case Directions.Front: faces[4].SetActive(true); break;
                case Directions.Back: faces[5].SetActive(true); break;
            }
        }
    }
}