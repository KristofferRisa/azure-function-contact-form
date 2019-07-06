using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;

namespace Form
{
    public static class SendMail
    {
        [FunctionName("SendMail")]
        public static void Run(
            [QueueTrigger("myqueue-items", Connection = "Connection_STORAGE")]string myQueueItem
            , [SendGrid(ApiKey = "sendgrid_api_rsvp")] out SendGridMessage message
            , ILogger log)
        {   
            dynamic data = JsonConvert.DeserializeObject(myQueueItem);
            log.LogInformation($"Form Queue trigger function processed: {data}");
            message = new SendGridMessage();
            var body = $@"
<p>
{data.Name} sendt you a messeage:
<p>
<hr>
<p>
{data.Message}
</p>
<p>Respond to {data.Email}
";            
            message.AddTo(Environment.GetEnvironmentVariable("ToEmail"));          
            message.AddContent("text/html",body);
            message.SetFrom("FromEmail");
            message.SetSubject("Melding fra {data.Name}");
            
        }
    }
}
