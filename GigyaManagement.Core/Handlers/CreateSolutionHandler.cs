using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.Project.ProjectModels;
using GigyaManagement.Core.Exceptions;
using GigyaManagement.Core.Services.LockService;

using Mediator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigyaManagement.Core.Handlers;

public class CreateSolutionRequest : IRequest<CreateSolutionResult>
{
    public string SolutionName { get; set; }
}

public class CreateSolutionResult
{
    public bool Created { get; set; }
    public string? SolutionFolder { get; set; }
    public string Error { get; set; }
}

public class CreateSolutionHandler : IRequestHandler<CreateSolutionRequest, CreateSolutionResult>
{
    private readonly IContextService _contextService;
    private readonly ILockService _lockService;

    public CreateSolutionHandler(IContextService contextService, ILockService lockService)
    {
        _contextService = contextService;
        _lockService = lockService;
    }
    public async ValueTask<CreateSolutionResult> Handle(CreateSolutionRequest request, CancellationToken cancellationToken)
    {
        // TODO: behavior?
        var ctx = _contextService.GetCurrentContext() ?? throw new ContextNotSetException();

        var solutionPath = Path.Combine(ctx.Workspace, request.SolutionName);

        if (Directory.Exists(solutionPath))
        {
            return new CreateSolutionResult
            {
                Created = false,
                SolutionFolder = null,
                Error = $"Solution with '{request.SolutionName}' already exists at workspace '{new Uri(ctx.Workspace)}'"
            };
        }

        var solution = GigyaSolution.New(request.SolutionName, ctx.Workspace);

        await solution.PersistToDisk();
        _lockService.LockRecursive(solutionPath);

        return new CreateSolutionResult
        {
            Created = true,
            SolutionFolder = solutionPath
        };
    }
}
