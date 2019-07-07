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
            [QueueTrigger("form-items", Connection = "Connection_STORAGE")]string myQueueItem
            , [SendGrid(ApiKey = "sendgrid_api_rsvp")] out SendGridMessage message
            , ILogger log)
        {   
            dynamic data = JsonConvert.DeserializeObject(myQueueItem);
            log.LogInformation($"Form Queue trigger function processed: {data}");
            message = new SendGridMessage();
            var body = $@"
<p>
 {data.name} sendt you a messeage:
<p>
<hr>
<p>
 {data.message}
</p>
<p>Respond to {data.email}
";            
            message.AddTo(Environment.GetEnvironmentVariable("ToEmail"));          
            message.AddContent("text/html",body);
            message.SetFrom(Environment.GetEnvironmentVariable("FromEmail"));
            message.SetSubject($"Melding fra {data.name}");
            
        }
    }
}
