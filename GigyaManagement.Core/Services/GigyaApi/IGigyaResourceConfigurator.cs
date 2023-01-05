namespace GigyaManagement.CLI.Services.GigyaApi;

public interface IGigyaResourceConfigurator<TResource, TContext>
{
    Task<TResource> Extract(TContext context);
    Task Apply(string apikey, TResource resource);
}
