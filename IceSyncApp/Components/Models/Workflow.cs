using System.Text.Json.Serialization;

namespace IceSyncApp.Components.Models
{
    public class Workflow
    {
        public string? WorkflowId { get; set; } 

        public string? WorkflowName { get; set; }

        public bool IsActive { get; set; }

        public string? MultiExecBehavior { get; set; }
    }
}