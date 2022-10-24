using UnityEngine;

public class SphereChecker : Checker
{
    [Header("Sphere")]
    public float radius;

    private void LateUpdate()
    {
        colliders = Physics.OverlapSphere(transform.position + offset, radius, layersToCheck);
    }

    private void OnDrawGizmos()
    {
        if (!gizmosSelected)
        {
            Gizmos.color = HaveCollider ? entryColor : nullColor;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (gizmosSelected)
        {
            Gizmos.color = HaveCollider ? entryColor : nullColor;
            Gizmos.DrawWireSphere(transform.position + offset, radius);
        }
    }
}