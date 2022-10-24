using UnityEngine;

public static class CharacterControllerExtension
{
    public static void LerpHeight(this CharacterController _controller, float _height, float _speed)
    {
        float center = _height / 2f;
        _controller.height = Mathf.Lerp(_controller.height, _height, _speed);
        _controller.center = Vector3.Lerp(_controller.center, new Vector3(0f, center, 0f), _speed);
    }

    public static Vector3 GetTopCenterPosition(this CharacterController _controller)
    {
        Vector3 position = GetTopPosition(_controller);
        position.y -= _controller.radius;
        return position;
    }

    public static Vector3 GetBottomCenterPosition(this CharacterController _controller)
    {
        Vector3 position = GetBottomPosition(_controller);
        position.y += _controller.radius;
        return position;
    }

    public static Vector3 GetTopPosition(this CharacterController _controller)
    {
        Vector3 position = (_controller.transform.position + _controller.center) + new Vector3(0f, (_controller.height / 2f), 0f);
        return position;
    }

    public static Vector3 GetBottomPosition(this CharacterController _controller)
    {
        Vector3 position = (_controller.transform.position + _controller.center) - new Vector3(0f, (_controller.height / 2f), 0f);
        return position;
    }
}