using IceSyncApp.Components.Data;
using IceSyncApp.Components.Interfaces;
using IceSyncApp.Models;
using Microsoft.EntityFrameworkCore;

namespace IceSyncApp.Components.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUniversalLoaderClient _apiClient;
        private readonly ILogger<WorkflowService> _logger;

        public WorkflowService(ApplicationDbContext dbContext, IUniversalLoaderClient apiClient, ILogger<WorkflowService> logger)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _logger = logger;
        }

        /// <summary>
        /// Returns workflows from the database (for UI display).
        /// </summary>
        public async Task<List<Workflow>> GetWorkflowsAsync()
        {
            return await _dbContext.Workflows.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Runs a workflow using the Universal Loader API.
        /// </summary>
        public async Task<bool> RunWorkflowAsync(string workflowId)
        {
            try
            {
                return await _apiClient.RunWorkflowAsync(workflowId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running workflow {WorkflowId}", workflowId);
                return false;
            }
        }

        /// <summary>
        /// Synchronizes workflows between API and local database.
        /// </summary>
        public async Task SyncWorkflowsAsync()
        {
            try
            {
                var apiWorkflows = await _apiClient.GetWorkflowsAsync();
                var dbWorkflows = await _dbContext.Workflows.ToListAsync();

                var apiMap = apiWorkflows.ToDictionary(w => w.WorkflowId);
                var dbMap = dbWorkflows.ToDictionary(w => w.WorkflowId);

                // Insert new workflows
                foreach (var apiWorkflow in apiWorkflows)
                {
                    if (!dbMap.ContainsKey(apiWorkflow.WorkflowId))
                    {
                        _dbContext.Workflows.Add(apiWorkflow);
                        _logger.LogInformation("Inserted new workflow {WorkflowId}", apiWorkflow.WorkflowId);
                    }
                }

                // Update existing workflows
                foreach (var apiWorkflow in apiWorkflows)
                {
                    if (dbMap.TryGetValue(apiWorkflow.WorkflowId, out var dbWorkflow))
                    {
                        if (dbWorkflow.WorkflowName != apiWorkflow.WorkflowName ||
                            dbWorkflow.IsActive != apiWorkflow.IsActive ||
                            dbWorkflow.MultiExecBehavior != apiWorkflow.MultiExecBehavior)
                        {
                            dbWorkflow.WorkflowName = apiWorkflow.WorkflowName;
                            dbWorkflow.IsActive = apiWorkflow.IsActive;
                            dbWorkflow.MultiExecBehavior = apiWorkflow.MultiExecBehavior;

                            _dbContext.Workflows.Update(dbWorkflow);
                            _logger.LogInformation("Updated workflow {WorkflowId}", apiWorkflow.WorkflowId);
                        }
                    }
                }

                // Delete workflows not in API
                foreach (var dbWorkflow in dbWorkflows)
                {
                    if (!apiMap.ContainsKey(dbWorkflow.WorkflowId))
                    {
                        _dbContext.Workflows.Remove(dbWorkflow);
                        _logger.LogInformation("Deleted workflow {WorkflowId}", dbWorkflow.WorkflowId);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synchronizing workflows.");
            }
        }
    }
}
