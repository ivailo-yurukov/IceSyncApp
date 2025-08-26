using IceSyncApp.Components.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IceSyncApp.Components.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowsController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;

        public WorkflowsController(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        // GET: api/workflows
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var workflows = await _workflowService.GetWorkflowsAsync();
            return Ok(workflows);
        }

        // POST: api/workflows/{id}/run
        [HttpPost("{id}/run")]
        public async Task<IActionResult> Run(string id)
        {
            var success = await _workflowService.RunWorkflowAsync(id);
            if (success)
                return Ok(new { message = "Workflow started successfully." });
            else
                return BadRequest(new { message = "Failed to start workflow." });
        }

        // POST: api/workflows/sync
        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            await _workflowService.SyncWorkflowsAsync();
            return Ok(new { message = "Synchronization completed." });
        }
    }
}
