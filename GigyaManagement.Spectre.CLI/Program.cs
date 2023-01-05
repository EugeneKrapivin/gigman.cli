using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Configurators;
using GigyaManagement.CLI.Services.GigyaApi.Models;
using GigyaManagement.Core.Exceptions;

using GigyaManagement.Spectre.CLI;
using GigyaManagement.Spectre.CLI.Commands;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

var services = new ServiceCollection();

var sp = Bootstrap(services);

var typeRegistrar = new TypeRegistrar(sp);

var app = new CommandApp(typeRegistrar);

CreateRootCommand(app);

app.RunAsync(args);

static IServiceCollection Bootstrap(IServiceCollection services)
{
    services.AddMediator();
    services.AddSingleton<IRegisterCommands, SiteCommandRegistrar>();
    services.AddSingleton<IRegisterCommands, ContextCommandRegistrar>();
    
    services.AddSingleton<IGigyaService, GigyaService>();
    
    services.AddSingleton<IGigyaResourceConfigurator<SiteConfig, string>, GigyaSiteConfigConfigurator>();
    services.AddSingleton<IGigyaResourceConfigurator<AccountsSchema, string>, GigyaSchemaConfigurator>();
    services.AddSingleton<IGigyaResourceConfigurator<ScreenSetsConfig, string>, ScreenSetsConfigurator>();

    services.AddSingleton<IContextService, ContextService>();
    services.AddSingleton(sp => sp.GetService<IContextService>()?.GetCurrentContext() switch
    {
        null => throw new ContextNotSetException(),
        { } ctx => ctx,
    });

    return services;
}
 
static void CreateRootCommand(CommandApp app)
{   
    app.Configure(conf =>
    {
        conf.SetApplicationName("gigman");

        var commandFactories = new IRegisterCommands[] 
        { 
            new ContextCommandRegistrar(), 
            new SiteCommandRegistrar() 
        };

        foreach (var factory in commandFactories)
        {
            factory.RegisterCommand(conf);
        }
    });
}
