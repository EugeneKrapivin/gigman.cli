using System;

using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Models;
using GigyaManagement.CLI.Services.Project.ProjectModels;
using GigyaManagement.CLI.Services.Template;
using GigyaManagement.CLI.Services.Template.ProjectModels;
using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

using Mediator;

namespace GigyaManagement.CLI.Handlers;

public class ScrapeSiteRequest : IRequest<ScrapeSiteResult>
{
    public string ApiKey { get; set; }

    public bool IsTemplate { get; set; } = false;

    public string Environment { get; set; }

    public string SolutionName { get; set; }
}

public class ScrapeSiteResult
{
    public string ProjectPath { get; internal set; }
}

public class ScrapeSiteHandler : IRequestHandler<ScrapeSiteRequest, ScrapeSiteResult>
{
    private readonly IGigyaResourceConfigurator<SiteConfig, string> _siteConfigConfigurator;
    private readonly IGigyaResourceConfigurator<AccountsSchema, string> _accountsSchemaConfigurator;
    private readonly IGigyaResourceConfigurator<ScreenSetsConfig, string> _screenSetsConfigurator;
    private readonly IProjectManager _projectManager;
    private readonly IContextService _contextService;

    public ScrapeSiteHandler(
        IGigyaResourceConfigurator<SiteConfig, string> siteConfigConfigurator,
        IGigyaResourceConfigurator<AccountsSchema, string> accountsSchemaConfigurator,
        IGigyaResourceConfigurator<ScreenSetsConfig, string> screenSetsConfigurator,
        IProjectManager projectManager,
        IContextService contextService)
    {
        _siteConfigConfigurator = siteConfigConfigurator;
        _accountsSchemaConfigurator = accountsSchemaConfigurator;
        _screenSetsConfigurator = screenSetsConfigurator;
        _projectManager = projectManager;
        _contextService = contextService;
    }

    public async ValueTask<ScrapeSiteResult> Handle(ScrapeSiteRequest request, CancellationToken cancellationToken)
    {
        var gigProject = new SiteProject
        {
            Environment = request.Environment,
            Apikey = request.ApiKey,
            SiteConfigResource = new SiteConfigResource { Resource = await _siteConfigConfigurator.Extract(request.ApiKey) },
            AccountsSchemaResource = new AccountsSchemaResource { Resource = await _accountsSchemaConfigurator.Extract(request.ApiKey) },
            ScreenSetsResource = new ScreenSetsResource { Resource = await _screenSetsConfigurator.Extract(request.ApiKey) },
            DataFlowsResource = new(),
            EmailTemplatesResource = new(),
            ExtensionsResource = new(),
            FlowsResource = new(),
            HostedPagesResource = new(),
            IdentitySecurityResource = new(),
            SmsTemplatesResource = new()
        };

        if (_contextService.GetCurrentContext() == null)
        {
            throw new Exception("Context not set");
        }

        var collectionFolder = request.IsTemplate ? "_template" : "_sites";

        var sitesPath = Path.Combine(_contextService.GetCurrentContext().Workspace, collectionFolder);

        var solutionPath = Path.Combine(sitesPath, request.SolutionName);
        GigyaSolution solution;
        if (Directory.Exists(solutionPath))
        {
            solution = await GigyaSolution.Load(solutionPath);
        }
        else
        {
            Directory.CreateDirectory(solutionPath);
            solution = GigyaSolution.New(request.SolutionName, solutionPath);
            await solution.PersistToDisk(); // ensure folder structure is created
        }

        try 
        {
            solution.Add(gigProject);
            await solution.PersistToDisk();
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
        }
        return new ScrapeSiteResult();

    }
}