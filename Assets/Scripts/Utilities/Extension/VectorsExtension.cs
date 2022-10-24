using UnityEngine;

public static class VectorsExtension
{
    public static Vector3 InverseTransformPoint(Vector3 transformPos, Quaternion transformRotation, Vector3 transformScale, Vector3 pos)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(transformPos, transformRotation, transformScale);
        Matrix4x4 inverse = matrix.inverse;
        return inverse.MultiplyPoint3x4(pos);
    }

    public static Vector3Int ToVector3Int(this Vector3 _vector3)
    {
        return new Vector3Int(
            Mathf.RoundToInt(_vector3.x),
            Mathf.RoundToInt(_vector3.y),
            Mathf.RoundToInt(_vector3.z)
        );
    }

    public static Vector3 RoundToInt(this Vector3 _vector3)
    {
        return new Vector3(
            Mathf.RoundToInt(_vector3.x),
            Mathf.RoundToInt(_vector3.y),
            Mathf.RoundToInt(_vector3.z)
        );
    }

    public static Vector3 FloorToInt(this Vector3 _vector3)
    {
        return new Vector3(
            Mathf.FloorToInt(_vector3.x),
            Mathf.FloorToInt(_vector3.y),
            Mathf.FloorToInt(_vector3.z)
        );
    }

    public static Vector3 Multiply(this Vector3 _vector3, Vector3 _target)
    {
        return new Vector3(
           _vector3.x * _target.x,
           _vector3.y * _target.y,
           _vector3.z * _target.z
        );
    }

    public static Vector3Int Multiply(this Vector3Int _vector3, Vector3Int _target)
    {
        return new Vector3Int(
           _vector3.x * _target.x,
           _vector3.y * _target.y,
           _vector3.z * _target.z
        );
    }

    public static Vector3Int Divide(this Vector3Int _vector3, Vector3Int _target)
    {
        return new Vector3Int(
           _vector3.x / _target.x,
           _vector3.y / _target.y,
           _vector3.z / _target.z
        );
    }
}