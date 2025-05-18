using Microsoft.Extensions.DependencyInjection;

namespace CVSiteService
{
    public static class Extension
    {
        public static void AddGitHubIntegration(this IServiceCollection services, Action<GitHubIntegrationOptions> action)
        {
            services.Configure(action);
            services.AddScoped<IGitHubService, GitHubService>();
        }
    }
}
