using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace salesforce_platform_events_dotnetcore
{
    public interface ISalesforceEventService
    {
        Task<string> PublishEventAsync();
    }


    public class SalesforceAuthentificationResponse
    {
        public string access_token { get; set; }
        public string instance_url { get; set; }
        public string id { get; set; }
        public string token_type { get; set; }
        public string issued_at { get; set; }
        public string signature { get; set; }
    }

    public class SalesforceEventService : ISalesforceEventService
    {
        public SalesforceEventService(IConfiguration configuration)
        {
            Username = configuration.GetSection("Salesforce")["Username"];
            Password = configuration.GetSection("Salesforce")["Password"];
            Token = configuration.GetSection("Salesforce")["Token"];
            ClientSecret = configuration.GetSection("Salesforce")["ClientSecret"];
            ClientId = configuration.GetSection("Salesforce")["ClientId"];
            LoginEndpoint = configuration.GetSection("Salesforce")["LoginEndpoint"];
            ApiEndpoint = configuration.GetSection("Salesforce")["ApiEndpoint"];
        }

        private string Username { get; }
        private string Password { get; }

        private string Token { get; }

        private string ClientId { get; }
        private string ClientSecret { get; }


        private string LoginEndpoint { get; }
        private string ApiEndpoint { get; }


        public async Task<string> PublishEventAsync()
        {
            //Login
            var salesforceAuthentificationResponse = await Login();

            using var client = new HttpClient();

            //Build dynamic Customer Interaction Object 
            dynamic customerInteraction = new ExpandoObject();

            //Quick note for .NET Salesforce beginners: Fields in Salesforce have the extensions __c. If your name your field "Message" the corresponding API name will be Message__c
            customerInteraction.Message__c = "Hello World";
            var json = JsonConvert.SerializeObject(customerInteraction);


            //Arrage 
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, $"{salesforceAuthentificationResponse.instance_url}{ApiEndpoint}sobjects/CustomerInteraction__e");
            request.Headers.Add("Authorization", "Bearer " + salesforceAuthentificationResponse.access_token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = content;

            //Send request with Auth Token 
            var response = await client.SendAsync(request);

            //Get results back. SF will response with a HTTP Status Code and a message
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Login to Salesforce and return the login object that contains the access token and instance url
        /// </summary>
        /// <returns></returns>
        public async Task<SalesforceAuthentificationResponse> Login()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("username", Username),
                new KeyValuePair<string, string>("password", Password + Token)
            });
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(LoginEndpoint),
                Content = content
            };
            var responseMessage = await httpClient.SendAsync(request);
            var response = await responseMessage.Content.ReadAsStringAsync();
            var salesforceAuthentificationResponse = JsonConvert.DeserializeObject<SalesforceAuthentificationResponse>(response);

            return salesforceAuthentificationResponse;
        }
    }
}