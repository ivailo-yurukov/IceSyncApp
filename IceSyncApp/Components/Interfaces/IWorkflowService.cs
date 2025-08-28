using IceSyncApp.Components.Models;

namespace IceSyncApp.Components.Interfaces
{
    public interface IWorkflowService
    {
        Task<List<Workflow>> GetWorkflowsAsync();       
        Task<bool> RunWorkflowAsync(string workflowId);  
        Task SyncWorkflowsAsync();
    }
}
