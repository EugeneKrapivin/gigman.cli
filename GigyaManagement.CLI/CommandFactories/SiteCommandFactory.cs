using GigyaManagement.CLI.Handlers;
using GigyaManagement.CLI.Services.Context;

using Mediator;

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
        var scrapeCommand = CreatePullCommand();

        command.Add(listCommand);
        command.Add(applyCommand);
        command.Add(scrapeCommand);

        return command;
    }

    private Command CreatePullCommand()
    {
        var scrapeCommand = new Command("pull", "pull an existing site form remote to local");
        
        var apiKeyOption = new Option<string>(new[] { "--apikey", "-k" }, "target apikey") { IsRequired = true };
        var solutionNameOption = new Option<string>(new[] { "--name", "-n" }, "name under which the site would be stored in the workspace") { IsRequired = true };
        var envOption = new Option<string>(new[] { "--env", "-e" }, () => "dev", "which environment the site is deployed at");
        
        scrapeCommand.Add(apiKeyOption);
        scrapeCommand.Add(solutionNameOption);
        scrapeCommand.Add(envOption);

        scrapeCommand.SetHandler(async (string apikey, string solutionName, string env) =>
        {
            var scaffoldResult = await _mediator.Send(new ScrapeSiteRequest
            {
                ApiKey = apikey,
                IsTemplate = false,
                Environment = env,
                SolutionName = solutionName
            });
        }, apiKeyOption, solutionNameOption, envOption);
        return scrapeCommand;
    }

    private Command CreateApplyCommand()
    {
        var applyCommand = new Command("apply", "apply local site configurations to remote");
        
        var solutionNameOption = new Option<string>(new[] { "--solution", "-s" }, "solution application will take effect on") { IsRequired = true };
        var envNameOption = new Option<string>(new[] { "--env", "-e" }, "envrionment application will take effect on") { IsRequired = true };
        var siteNameOption = new Option<string>(new[] { "--target", "-t" }, "target apikey") { IsRequired = false };

        applyCommand.Add(siteNameOption);
        applyCommand.Add(envNameOption);
        applyCommand.Add(solutionNameOption);


        applyCommand.SetHandler(async (ctx) =>
        {
            var site = ctx.BindingContext.ParseResult.GetValueForOption(siteNameOption)!;
            var sol = ctx.BindingContext.ParseResult.GetValueForOption(solutionNameOption)!;
            var env = ctx.BindingContext.ParseResult.GetValueForOption(envNameOption)!;

            try { 
                await _mediator.Send(new ApplySiteChangesRequest
                {
                    Solution = sol,
                    Environment = site,
                    IsTemplate = false
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
