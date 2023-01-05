using GigyaManagement.Core;

using Spectre.Console;
using Spectre.Console.Json;

using System.Text.Json;

namespace GigyaManagement.Spectre.CLI.Commands;

public static class Extensions
{
    public static Panel AsJsonPanel<T>(this T source, string header)
        => new Panel(source.AsJsonText())
                    .Header(header)
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Yellow);

    public static JsonText AsJsonText<T>(this T source) 
        => new JsonText(JsonSerializer.Serialize(source, GlobalUsings.JsonSerializerOptions))
            .BracesColor(Color.Red)
            .BracketColor(Color.Green)
            .ColonColor(Color.Blue)
            .CommaColor(Color.Red)
            .StringColor(Color.Green)
            .NumberColor(Color.Blue)
            .BooleanColor(Color.Red)
            .NullColor(Color.Green);
}
