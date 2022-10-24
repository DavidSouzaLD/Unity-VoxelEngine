using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// Manages and applies audios across scenes in 2D and 3D ways.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class Audio : MonoBehaviour
    {
        private static AudioSource Source;

        private void Awake()
        {
            Source = GetComponent<AudioSource>();
        }

        public static void PlaySound(AudioClip _clip, float _volume = 1f)
        {
            if (_clip != null)
            {
                Source.PlayOneShot(_clip, _volume);
            }
        }

        public static void Play3DSound(AudioClip _clip, Vector3 _position, float _minDistance = 1f, float _maxDistance = 500f, float _volume = 1f, float _destroyTime = 5f)
        {
            if (_clip != null)
            {
                AudioSource source3D = new GameObject().AddComponent<AudioSource>();
                source3D.playOnAwake = false;
                source3D.clip = _clip;
                source3D.spatialBlend = 1f;
                source3D.volume = _volume;
                source3D.minDistance = _minDistance;
                source3D.maxDistance = _maxDistance;
                source3D.Play();
                Destroy(source3D.gameObject, _destroyTime);
            }
        }
    }
}