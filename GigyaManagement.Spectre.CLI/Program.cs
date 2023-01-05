using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Configurators;
using GigyaManagement.CLI.Services.GigyaApi.Models;
using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;
using GigyaManagement.Core.Exceptions;

using GigyaManagement.Spectre.CLI.Commands;
using GigyaManagement.Spectre.CLI.Commands.Abstractions;
using GigyaManagement.Spectre.CLI.Infra;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;
using Spectre.Console.Cli;

var services = new ServiceCollection();

services.Bootstrap();

var typeRegistrar = new TypeRegistrar(services);

var app = new CommandApp(typeRegistrar);

app.SetupCommandApp();

app.RunAsync(args);


file static class ServicesExtensions
{
    public static IServiceCollection Bootstrap(this IServiceCollection services)
    {
        services.AddMediator();
        services.RegisterServices();

        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
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
}

file static class CommandAppExtensions
{ 
    public static void SetupCommandApp(this CommandApp app)
    => app.Configure(conf =>
        {
            conf.SetApplicationName("gigman");
            
            conf.SetExceptionHandler(ex =>
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                return -2;
            });

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