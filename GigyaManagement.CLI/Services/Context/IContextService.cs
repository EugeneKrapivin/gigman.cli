namespace GigyaManagement.CLI.Services.Context;

public interface IContextService
{
    WorkspaceContext SetContext(string context);

    WorkspaceContext? GetCurrentContext();

    ContextModel GetAllContexts();

    WorkspaceContext CreateNewContext(WorkspaceContext context);
}
