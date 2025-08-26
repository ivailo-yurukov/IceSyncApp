using IceSyncApp.Components.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IceSyncApp.Components.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversalLoaderTestController : ControllerBase
    {
        private readonly IUniversalLoaderClient _client;

        public UniversalLoaderTestController(IUniversalLoaderClient client)
        {
            _client = client;
        }

        // GET: api/universalloader/test/token
        [HttpGet("token")]
        public async Task<IActionResult> GetToken()
        {
            var token = await _client.GetTokenAsync();
            return Ok(new { token });
        }

        // GET: api/universalloader/test/workflows
        [HttpGet("workflows")]
        public async Task<IActionResult> GetWorkflows()
        {
            var workflows = await _client.GetWorkflowsAsync();
            return Ok(workflows);
        }

        // POST: api/universalloader/test/workflows/{id}/run
        [HttpPost("workflows/{id}/run")]
        public async Task<IActionResult> RunWorkflow(string id)
        {
            var success = await _client.RunWorkflowAsync(id);
            if (success)
                return Ok(new { message = "Workflow started successfully." });
            else
                return BadRequest(new { message = "Failed to start workflow." });
        }
    }
}
