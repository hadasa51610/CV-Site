using GitHubService;
using Microsoft.Extensions.Options;
using Octokit;

namespace GitHubService
{
    public class GitHubService:IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly GitHubIntegrationOptions _options;

        public GitHubService(IOptions<GitHubIntegrationOptions> options)
        {
            _client = new GitHubClient(new ProductHeaderValue("my_github_app"));
            _options = options.Value;
        }

    }
}
