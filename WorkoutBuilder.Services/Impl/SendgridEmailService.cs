using HttpTracer;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace WorkoutBuilder.Services.Impl
{
    public class SendgridEmailService : IEmailService
    {
        public IConfiguration Configuration { init; protected get; } = null!;

        public void Send(string toAddress, string subject, string body)
        {
            var options = new RestClientOptions("https://api.sendgrid.com");
            var client = new RestClient(options);
            client.AddDefaultHeader("Authorization", $"Bearer {Configuration["SendgridApiKey"]}");
            var restBody = new
            {
                personalizations = new[]
                {
                    new
                    {
                        to = new[]
                        {
                            new { email = toAddress }
                        }
                    }
                },
                from = new
                {
                    email = Configuration["SendgridFromAddress"],
                    name = Configuration["SendgridFromName"]
                },
                subject,
                content = new[]
                {
                    new { type = "text/html", value = body}
                }
            };

            var request = new RestRequest("v3/mail/send", Method.Post)
                .AddJsonBody(restBody);

            var response = client.ExecutePost(request);
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Sendgrid reported an error while sending message: {response.Content}");
        }

    }
}
