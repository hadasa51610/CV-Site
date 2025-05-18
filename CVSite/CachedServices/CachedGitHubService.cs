using CVSiteService;
using CVSiteService.Models;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace CVSite.NewFolder
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;
        private const string UserPortFolioKey = "UserPortFolioKey";
        private const string LastEventKey = "LastEventKey";

        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }

        public async Task<List<PortFolio>> GetPortFolioAsync()
        {
            var latestEventDate = await GetLastUserActivityDateAsync();

            if (_memoryCache.TryGetValue(LastEventKey, out DateTimeOffset cachedEventDate) &&
                _memoryCache.TryGetValue(UserPortFolioKey, out List<PortFolio> cachedPortfolio))
            {
                if (latestEventDate <= cachedEventDate)
                {
                    return cachedPortfolio;
                }
            }

            cachedPortfolio = await _gitHubService.GetPortFolioAsync();

            _memoryCache.Set(UserPortFolioKey, cachedPortfolio);
            _memoryCache.Set(LastEventKey, latestEventDate ?? DateTimeOffset.UtcNow);

            return cachedPortfolio;
        }

        public Task<List<Repository>> SearchRepositoriesAsync(string? name = null, string? language = null, string? user = null)
        {
            return _gitHubService.SearchRepositoriesAsync(name, language, user);
        }

        public async Task<DateTimeOffset?> GetLastUserActivityDateAsync()
        {
            return await _gitHubService.GetLastUserActivityDateAsync();
        }
    }
}