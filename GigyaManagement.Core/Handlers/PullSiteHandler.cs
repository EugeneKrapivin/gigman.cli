using System;

using GigyaManagement.CLI.Services.Context;

using GigyaManagement.CLI.Services.GigyaApi;
using GigyaManagement.CLI.Services.GigyaApi.Models;

using GigyaManagement.CLI.Services.Project.ProjectModels;

using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;
using GigyaManagement.Core.Exceptions;

using Mediator;

namespace GigyaManagement.CLI.Handlers;

public class PullSiteGroupRequest : IRequest<PullSiteResult>
{
    public string ApiKey { get; set; }

    public bool IsTemplate { get; set; } = false;

    public bool ChildSite { get; set; }

    public string Environment { get; set; }

    public string SiteName { get; set; }

    public bool IncludeGroup { get; set; } = false;
    public string SolutionName { get; set; }
}

public class PullSiteRequest : IRequest<PullSiteResult>
{
    public string ApiKey { get; set; }

    public bool IsTemplate { get; set; } = false;

    public bool ChildSite { get; set; }

    public string Environment { get; set; }

    public string SiteName { get; set; }

    public bool IncludeGroup { get; set; } = false;
    public string SolutionName { get; set; }
}

public class PullSiteResult
{
    public string ProjectPath { get; internal set; }
    public SiteProject SiteProject { get; internal set; }
    public IEnumerable<SiteProject> ChildSites { get; internal set; }
}

public class PullSiteHandler 
    : IRequestHandler<PullSiteRequest, PullSiteResult>
    , IRequestHandler<PullSiteGroupRequest, PullSiteResult>
{
    private readonly IGigyaResourceConfigurator<SiteConfig, string> _siteConfigConfigurator;
    private readonly IGigyaResourceConfigurator<AccountsSchema, string> _accountsSchemaConfigurator;
    private readonly IGigyaResourceConfigurator<ScreenSetsConfig, string> _screenSetsConfigurator;
    private readonly IContextService _contextService;

    public PullSiteHandler(
        IGigyaResourceConfigurator<SiteConfig, string> siteConfigConfigurator,
        IGigyaResourceConfigurator<AccountsSchema, string> accountsSchemaConfigurator,
        IGigyaResourceConfigurator<ScreenSetsConfig, string> screenSetsConfigurator,
        IContextService contextService)
    {
        _siteConfigConfigurator = siteConfigConfigurator;
        _accountsSchemaConfigurator = accountsSchemaConfigurator;
        _screenSetsConfigurator = screenSetsConfigurator;
        _contextService = contextService;
    }

    public async ValueTask<PullSiteResult> Handle(PullSiteRequest request, CancellationToken cancellationToken)
    {
        var gigProject = await GetSiteProject(request.ApiKey);

        if (_contextService.GetCurrentContext() == null)
        {
            throw new Exception("Context not set");
        }

        var ctx = _contextService.GetCurrentContext() ?? throw new ContextNotSetException();

        var solutionPath = Path.Combine(ctx.Workspace, request.SolutionName);

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

        solution.Add(gigProject);
        await solution.PersistToDisk();

        return new PullSiteResult()
        {
            ProjectPath = solutionPath,
            SiteProject = gigProject
        };
    }

    public async ValueTask<PullSiteResult> Handle(PullSiteGroupRequest request, CancellationToken cancellationToken)
    {
        if (_contextService.GetCurrentContext() == null)
        {
            throw new Exception("Context not set");
        }
        var ctx = _contextService.GetCurrentContext() ?? throw new ContextNotSetException();

        var gigProject = await GetSiteProject(request.ApiKey);

        var solutionPath = Path.Combine(ctx.Workspace, request.SolutionName);

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

        solution.Add(gigProject);

        foreach(var member in gigProject.SiteConfigResource?.Resource?.SiteGroupSettings?.Members ?? Enumerable.Empty<string>())
        {
            var site = await GetSiteProject(member);
            site.ChildSite = true;
            site.ParentSite = gigProject;

            gigProject.ChildSites.Add(site);
        }

        await solution.PersistToDisk();

        return new PullSiteResult()
        {
            ProjectPath = solutionPath,
            SiteProject = gigProject,
            ChildSites = gigProject.ChildSites
        };
    }

    private async Task<SiteProject> GetSiteProject(string apikey)
    {
        return new SiteProject
        {
            Apikey = apikey,
            SiteConfigResource = new SiteConfigResource { Resource = await _siteConfigConfigurator.Extract(apikey) },
            AccountsSchemaResource = new AccountsSchemaResource { Resource = await _accountsSchemaConfigurator.Extract(apikey) },
            ScreenSetsResource = new ScreenSetsResource { Resource = await _screenSetsConfigurator.Extract(apikey) },
            DataFlowsResource = new(),
            EmailTemplatesResource = new(),
            ExtensionsResource = new(),
            FlowsResource = new(),
            HostedPagesResource = new(),
            IdentitySecurityResource = new(),
            SmsTemplatesResource = new()
        };
    }
}