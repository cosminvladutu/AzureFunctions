using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace Orchestrations
{
    public class TerminateOrchestration
    {
        private readonly ILogger<TerminateOrchestration> _logger;

        public TerminateOrchestration(ILogger<TerminateOrchestration> logger)
        {
            _logger = logger;
        }

        [Function("TerminateOrchestration")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "start/{instanceId}")] HttpRequest req,
            DurableTaskClient orchestrationStarter,
            string instanceId)
        {
            await orchestrationStarter.TerminateInstanceAsync(instanceId, "Manual Termination");
            _logger.LogInformation($"{instanceId} stopped");
            
            return new OkResult();

        }
    }
}