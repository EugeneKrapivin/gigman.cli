using Microsoft.Extensions.DependencyInjection;

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace GigyaConfigCLI.Factories;

public static class RootCommandFactory
{
    public static Parser CreateRootCommand(IServiceProvider sp, Func<IConsole, Task> preHandle)
    {
        var logOpt = CreateLogLevelOption();

        var commandFactories = sp.GetServices<ICommandFactory>();

        var root = new RootCommand("A CLI tool for managing gigya resources");
        var builder = new CommandLineBuilder(root);
        builder.UseDefaults()
            .AddMiddleware(async ctx =>
            {
                await preHandle(ctx.Console);
            });

        foreach (var factory in commandFactories)
        {
            root.AddCommand(factory.CreateCommand());
        }

        root.AddGlobalOption(logOpt);
        builder.AddMiddleware(ctx =>
        {
            //var lvl = ctx.ParseResult.GetValueForOption(logOpt);
            //var loggerConf = new LoggerConfiguration();

            //loggerConf = lvl switch
            //{
            //    "verbose" => loggerConf.MinimumLevel.Verbose(),
            //    "debug" => loggerConf.MinimumLevel.Debug(),
            //    "info" => loggerConf.MinimumLevel.Information(),
            //    "warn" => loggerConf.MinimumLevel.Warning(),
            //    "err" => loggerConf.MinimumLevel.Error(),
            //    "fatal" => loggerConf.MinimumLevel.Fatal(),
            //    _ => throw new NotImplementedException()
            //};

            //Log.Logger = loggerConf
            //    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            //    .CreateLogger();

        });

        return builder.Build();

        static Option<string> CreateLogLevelOption()
        {
#if RELEASE
    var defaultLogOpt = () => "fatal";
#elif DEBUG
            var defaultLogOpt = () => "verbose";
#endif

            var logOpt = new Option<string>("-v", defaultLogOpt, "set log verbosity");

            logOpt.FromAmong("verbose", "debug", "info", "warn", "err", "fatal");
            return logOpt;
        }
    }
}
