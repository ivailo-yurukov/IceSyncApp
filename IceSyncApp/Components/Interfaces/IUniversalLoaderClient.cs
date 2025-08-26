using IceSyncApp.Models;

namespace IceSyncApp.Components.Interfaces
{
    public interface IUniversalLoaderClient
    {
        Task<string> GetTokenAsync();
        Task<List<Workflow>> GetWorkflowsAsync();
        Task<bool> RunWorkflowAsync(string workflowId);
    }
}
