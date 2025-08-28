namespace IceSyncApp.Components.Models
{
    public class WorkflowDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public bool IsActive { get; set; }

        public string? MultiExecBehavior { get; set; }
    }
}
