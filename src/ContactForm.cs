using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Form
{
    public static class ContactForm
    {
        [FunctionName("ContactForm")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("form-items", Connection = "Connection_STORAGE")]ICollector<string> outputQueueItem,
            ILogger log)
        {
            //This function process the request and forward the request on the queue 
            log.LogInformation("Form HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            log.LogInformation($"Reading data from Form: {data.name}. Sendt from {data.email}.");

            // outputQueueItem.Add($"Name passed to the function: {data.name}. Sendt from {data.email}.");
            outputQueueItem.Add(data.ToString());
            
            return data != null
                ? (ActionResult)new OkObjectResult($"Hello, {data.name}. Sendt from {data.email}.")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
