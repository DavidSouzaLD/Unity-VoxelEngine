public static class StringExtension
{
    /// <summary>
    /// Returns the first letter of the string.
    /// </summary>
    public static string GetFirstWord(this string text)
    {
        return text.Substring(0, 1);
    }
}