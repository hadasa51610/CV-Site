using CVSiteService.Models;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVSiteService
{
    public interface IGitHubService
    {
        Task<List<PortFolio>> GetPortFolioAsync();
        Task<List<Repository>> SearchRepositoriesAsync(string? name = null, string? language = null, string? user = null);
        Task<DateTimeOffset?> GetLastUserActivityDateAsync();
    }
}
