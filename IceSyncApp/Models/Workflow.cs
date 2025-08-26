using System.Text.Json.Serialization;

namespace IceSyncApp.Models
{
    public class Workflow
    {
        [JsonPropertyName("id")]
        public int WorkflowId { get; set; } 

        [JsonPropertyName("name")]
        public string? WorkflowName { get; set; }

        public bool IsActive { get; set; }

        public string? MultiExecBehavior { get; set; }
    }
}