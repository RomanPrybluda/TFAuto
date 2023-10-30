namespace TFAuto.Domain.ExtensionMethods;

public static class StringExtensions
{
    public static string FirstLetterToLower(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        char[] characters = input.ToCharArray();
        characters[0] = char.ToLower(characters[0]);

        return new string(characters);
    }
}
