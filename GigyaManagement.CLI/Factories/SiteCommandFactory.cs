using GigyaManagement.CLI.Handlers;
using GigyaManagement.CLI.Services.Context;

using MediatR;

using System.CommandLine;
using System.CommandLine.IO;

namespace GigyaConfigCLI.Factories;

public class SiteCommandFactory : ICommandFactory
{
    private readonly WorkspaceContext _partnerContext;
    private readonly IMediator _mediator;

    public SiteCommandFactory(WorkspaceContext partnerContext, IMediator mediator)
    {
        _partnerContext = partnerContext;
        _mediator = mediator;
    }
    public Command CreateCommand()
    {
        var command = new Command("site", "site oriented command");

        var listCommand = CreateListCommand();
        var applyCommand = CreateApplyCommand();
        var scrapeCommand = CreateScrapeCommand();

        command.Add(listCommand);
        command.Add(applyCommand);
        command.Add(scrapeCommand);

        return command;
    }

    private Command CreateScrapeCommand()
    {
        var scrapeCommand = new Command("scrape", "scrape a site form remote to local");
        var apiKeyOption = new Option<string>(new[] { "--apikey", "-k" }, "target apikey");
        scrapeCommand.Add(apiKeyOption);

        scrapeCommand.SetHandler(async (string apikey) =>
        {
            var scaffoldResult = await _mediator.Send(new ScrapeSiteRequest
            {
                ApiKey = apikey,
                IsTemplate = false
            }); ;
        }, apiKeyOption);
        return scrapeCommand;
    }

    private Command CreateApplyCommand()
    {
        var applyCommand = new Command("apply", "apply local site configurations to remote");
        var siteName = new Option<string>(new[] { "--name", "-n" }, "site name ") { IsRequired = true };
        applyCommand.Add(siteName);

        applyCommand.SetHandler(async (ctx) =>
        {
            var site = ctx.BindingContext.ParseResult.GetValueForOption(siteName);
            try { 
                await _mediator.Send(new ApplySiteChangesRequest
                {
                    Site = site
                });

                ctx.Console.WriteLine("changes applied");
            } 
            catch (Exception ex)
            {
                ctx.ExitCode = -1;
                ctx.Console.Error.WriteLine(ex.Message);
            }
        });
        return applyCommand;
    }

    private Command CreateListCommand()
    {
        var listCommand = new Command("list", "list locally scrapped sites");
        listCommand.SetHandler((ctx) =>
        {
            var sitesWorkspace = Path.Combine(_partnerContext.Workspace, "_sites");
            if (!Directory.Exists(sitesWorkspace) || !Directory.GetDirectories(sitesWorkspace).Any())
            {
                ctx.Console.Error.WriteLine("There are no site in current workspace");
                ctx.ExitCode = -1;
                return;
            }
            var sites = Directory.GetDirectories(sitesWorkspace)
                .Select(x => Path.GetFileName(x))
                .Select(x => $"* {x}");

            ctx.Console.WriteLine(string.Join("\r\n", sites));
        });
        return listCommand;
    }
}
