using IceSyncApp.Components.Interfaces;

namespace IceSyncApp.Components.Services
{
    public class WorkflowSyncHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WorkflowSyncHostedService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

        public WorkflowSyncHostedService(IServiceProvider serviceProvider, ILogger<WorkflowSyncHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Workflow sync background service started.");

            // Run loop until cancellation
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var workflowService = scope.ServiceProvider.GetRequiredService<IWorkflowService>();
                    try
                    {
                        _logger.LogInformation("Starting workflow synchronization...");

                        await workflowService.SyncWorkflowsAsync();

                        _logger.LogInformation("Workflow synchronization completed.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during workflow synchronization.");
                    }
                }
                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    
                }
            }

            _logger.LogInformation("Workflow sync background service stopping.");
        }
    }
}