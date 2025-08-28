using IceSyncApp.Components.Models;

namespace IceSyncApp.Components.Interfaces
{
    public interface IUniversalLoaderService
    {
        Task<string> GetTokenAsync();
        Task<List<Workflow>> GetWorkflowsAsync();
        Task<bool> RunWorkflowAsync(string workflowId);
    }
}
