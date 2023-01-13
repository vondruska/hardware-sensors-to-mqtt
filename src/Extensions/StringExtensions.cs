using System.Text.RegularExpressions;

namespace HardwareSensorsToMQTT;

public static class StringExtensions
{
    private static readonly Regex FilterInvalidCharacterIdRegex = new Regex("[^a-zA-Z0-9_-]");
    public static string NormalizeIdForMqtt(this string input) =>
        FilterInvalidCharacterIdRegex
            .Replace(input.Trim('/').Replace('/', '_'), String.Empty);
}