using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Configurators;
using GigyaManagement.CLI.Services.GigyaApi.Models;
using GigyaManagement.Core.Exceptions;
using GigyaManagement.Spectre.CLI.Commands;
using GigyaManagement.Spectre.CLI.Infra;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Cli;
using Spectre.Console;

using System;

var services = new ServiceCollection();

var sp = Bootstrap(services);

var typeRegistrar = new TypeRegistrar(sp);

var app = new CommandApp(typeRegistrar);

CreateRootCommand(typeRegistrar.ServiceProvider, app);

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
 
static void CreateRootCommand(IServiceProvider sp, CommandApp app)
{
    var commandFactories = sp.GetServices<IRegisterCommands>();
    
    app.Configure(conf =>
    {
        foreach (var factory in commandFactories)
        {
            factory.RegisterCommand(conf);
        }
    });
}
