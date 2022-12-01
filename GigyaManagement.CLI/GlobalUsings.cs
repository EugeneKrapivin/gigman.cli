using System.Text.Json;

namespace GigyaManagement.CLI
{
    internal static class GlobalUsings
    {
        public static JsonSerializerOptions JsonSerializerOptions => new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}
