using UnityEngine;

public static class ColorExtension
{
    /// <summary>
    /// Returns a random color.
    /// </summary>
    public static Color GetRandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}