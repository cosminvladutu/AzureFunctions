using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Orchestrations
{
    public class StartOrchestration
    {
        private readonly ILogger<StartOrchestration> _logger;

        public StartOrchestration(ILogger<StartOrchestration> logger)
        {
            _logger = logger;
        }

        [Function("StartOrchestration")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Admin, "post", Route = "start/{orchestrationName}")] HttpRequest req,
            DurableTaskClient orchestrationStarter,
            string orchestrationName)
        {
            if (req.Body == null || req.Body.Length == 0)
            {
                throw new ValidationException("requestBody cannot be empty");
            }
            
            var requestBody = new StreamReader(req.Body).ReadToEnd();
           
            var input = req.ContentLength > 0
                  ? JsonConvert.DeserializeObject<object>(requestBody)
                  : null;

                await orchestrationStarter.ScheduleNewOrchestrationInstanceAsync(new Microsoft.DurableTask.TaskName(orchestrationName), input);

            _logger.LogInformation($"{orchestrationName} started");
            return new OkResult();

        }
    }
}