using CVSiteService.Models;
using Microsoft.Extensions.Options;
using Octokit;

namespace CVSiteService
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly GitHubIntegrationOptions _options;

        public GitHubService(IOptions<GitHubIntegrationOptions> options)
        {
            _options = options.Value;
            _client = new GitHubClient(new ProductHeaderValue("My_GitHub_App"))
            {
                Credentials = new Credentials(_options.Token)
            };
        }

        public async Task<List<PortFolio>> GetPortFolioAsync()
        {
            var repositories = await _client.Repository.GetAllForUser(_options.UserName);
            var portfolios = new List<PortFolio>();

            foreach (var repo in repositories)
            {
                if (repo.Size > 0)
                {
                    var languages = await _client.Repository.GetAllLanguages(repo.Id);
                    var commits = await _client.Repository.Commit.GetAll(repo.Id);
                    var lastCommit = commits.FirstOrDefault()?.Commit.Author.Date;
                    var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Id);


                    portfolios.Add(new PortFolio
                    {
                        Name = repo.Name,
                        Url = repo.HtmlUrl,
                        Languages = languages.Select(lan => lan.Name).ToList(),
                        LastCommitDate = lastCommit,
                        Stars = repo.StargazersCount,
                        PullRequests = pullRequests.Count
                    });
                }
            }
            return portfolios;
        }

        public async Task<List<Repository>> SearchRepositoriesAsync(string? name = null, string? language = null, string? user = null)
        {
            var queryParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(name))
                queryParts.Add($"{name} in:name");

            if (!string.IsNullOrWhiteSpace(language))
                queryParts.Add($"language:{language}");

            if (!string.IsNullOrWhiteSpace(user))
                queryParts.Add($"user:{user}");

            if (queryParts.Count == 0)
                queryParts.Add("is:public");

            var query = string.Join(" ", queryParts);

            var request = new SearchRepositoriesRequest(query)
            {
                SortField = RepoSearchSort.Updated,
                Order = SortDirection.Descending
            };

            var result = await _client.Search.SearchRepo(request);
            return result.Items.ToList();
        }

        public async Task<DateTimeOffset?> GetLastUserActivityDateAsync()
        {
            var events = await _client.Activity.Events.GetAllUserPerformed(_options.UserName);

            return events.FirstOrDefault()?.CreatedAt;
        }
    }
}