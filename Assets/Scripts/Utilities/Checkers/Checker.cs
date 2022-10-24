using UnityEngine;

[ExecuteInEditMode]
public class Checker : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask layersToCheck;
    public Vector3 offset;
    public Collider[] colliders;

    [Header("Gizmos")]
    public bool gizmosSelected = true;
    public Color entryColor = Color.green;
    public Color nullColor = Color.red;

    public virtual bool HaveCollider
    {
        get
        {
            return colliders.Length > 0;
        }
    }
}
