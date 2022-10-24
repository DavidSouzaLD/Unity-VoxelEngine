using UnityEngine;

public static class RotationExtension
{
    public enum Axis { X, Y, Z }

    /// <summary>
    /// Clamp the axis of fate.
    /// </summary>
    public static Quaternion ClampRotation(this Quaternion q, float min, float max, Axis Axis)
    {
        switch (Axis)
        {
            case Axis.X:

                q.x /= q.w;
                q.y /= q.w;
                q.z /= q.w;
                q.w = 1.0f;

                float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

                angleX = Mathf.Clamp(angleX, min, max);

                q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

                return q;

            case Axis.Y:

                q.x /= q.w;
                q.y /= q.w;
                q.z /= q.w;
                q.w = 1.0f;

                float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

                angleY = Mathf.Clamp(angleY, min, max);

                q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

                return q;

            case Axis.Z:

                q.x /= q.w;
                q.y /= q.w;
                q.z /= q.w;
                q.w = 1.0f;

                float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);

                angleZ = Mathf.Clamp(angleZ, min, max);

                q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

                return q;
        }

        return Quaternion.Euler(0f, 0f, 0f);
    }

    public static Vector3 QuaternionToVector3(this Quaternion _quaternion)
    {
        return new Vector3(_quaternion.x, _quaternion.y, _quaternion.z);
    }

    public static Quaternion Vector3ToQuaternion(this Vector3 _vector3)
    {
        return Quaternion.Euler(_vector3);
    }
}