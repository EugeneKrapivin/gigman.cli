using System.Text.Json;

namespace GigyaManagement.Core
{
    public static class GlobalUsings
    {
        public static JsonSerializerOptions JsonSerializerOptions => new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}
