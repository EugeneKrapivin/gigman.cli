using GigyaConfigCLI.Factories;
using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Configurators;
using GigyaManagement.CLI.Services.GigyaApi.Models;
using GigyaManagement.CLI.Services.Template;

using Mediator;

using Microsoft.Extensions.DependencyInjection;

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Reflection;

var sp = Bootstrap();

var root = RootCommandFactory.CreateRootCommand(sp, PrintHeaderAsync);

if (args.Length == 0)
{
    args = new[] { "-h" };
   
}

await root.InvokeAsync(args);

//Log.CloseAndFlush();

static ServiceProvider Bootstrap()
{
    var services = new ServiceCollection();
    
    services.AddMediator();

    services.AddSingleton<IGigyaService, GigyaService>();
    services.AddSingleton<IGigyaResourceConfigurator<SiteConfig, string>, GigyaSiteConfigConfigurator>();
    services.AddSingleton<IGigyaResourceConfigurator<AccountsSchema, string>, GigyaSchemaConfigurator>();
    services.AddSingleton<IGigyaResourceConfigurator<ScreenSetsConfig, string>, ScreenSetsConfigurator>();
    
    services.AddSingleton<IContextService, ContextService>();

    services.AddSingleton<ProjectManagerOptions>();
    services.AddSingleton<IProjectManager, ProjectManager>();

    services.AddSingleton(sp => sp.GetService<IContextService>()!.GetCurrentContext());

    services.AddSingleton<ICommandFactory, SiteCommandFactory>();
    services.AddSingleton<ICommandFactory, ContextCommandFactory>();
    services.AddSingleton<ICommandFactory, TemplateCommandFactory>();
    services.AddSingleton<ICommandFactory, CreateCommandFactory>();

    var sp = services.BuildServiceProvider();
    return sp;
}

static async Task PrintHeaderAsync(IConsole console)
{
    var _toolName = "Gigya site management CLI";

    var currentVersion = Assembly
        .GetExecutingAssembly()!
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
        .InformationalVersion;

    console.WriteLine($"{_toolName} (version {currentVersion})");
    try
    {
        var latestVersion = await GetLatestVersionAsync();

        if (latestVersion != currentVersion)
        {
            console.WriteLine($"You are using version {currentVersion}, a newer version exists {latestVersion}");
        }
    }
    catch
    {
        console.WriteLine("failed to fetch latest version from gist");
    }
    console.WriteLine("");
}

static async Task<string> GetLatestVersionAsync()
{
    var client = new HttpClient();
    var version = await client.GetStringAsync("https://gist.githubusercontent.com/EugeneKrapivin/8e81121089e73f8ca122e6abbaf252a5/raw/56e0d52abeaabb4b91bb219037718320fdcba476/version");

    return version;
}
