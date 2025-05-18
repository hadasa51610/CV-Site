using CVSiteService;
using CVSiteService.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CVSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;
        public GitHubController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        // GET: api/<GitHubController>
        [HttpGet("portFolio")]
        public async Task<ActionResult<List<PortFolio>>> Get()
        {
            return await _gitHubService.GetPortFolioAsync();
        }

        [HttpGet("searchRepositories")]
        public async Task<ActionResult<List<PortFolio>>> Get([FromQuery] string? name, [FromQuery] string? language, [FromQuery] string? user)
        {
            var result = await _gitHubService.SearchRepositoriesAsync(name, language, user);
            return result != null ? Ok(result) : NotFound();
        }
    }
}