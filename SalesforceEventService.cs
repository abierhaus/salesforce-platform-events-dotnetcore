using System;
using System.Collections.Generic;
using System.Net;
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
        Task PublishEventAsync();
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
  
        private readonly IConfigurationRoot _configRoot;


        public SalesforceEventService(IConfiguration configRoot)
        {
            _configRoot = (IConfigurationRoot) configRoot;

            //// SF requires TLS 1.1 or 1.2
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            Username = _configRoot.GetSection("Salesforce")["Username"];
            Password = _configRoot.GetSection("Salesforce")["Password"];
            Token = _configRoot.GetSection("Salesforce")["Token"];
            ClientSecret = _configRoot.GetSection("Salesforce")["ClientSecret"];
            ClientId = _configRoot.GetSection("Salesforce")["ClientId"];
            LoginEndpoint = _configRoot.GetSection("Salesforce")["LoginEndpoint"];
            ApiEndpoint = _configRoot.GetSection("Salesforce")["ApiEndpoint"];

        }

        private  string Username { get; set; }
        private string Password { get; set; }

        private string Token { get; set; }

        private string ClientId { get; set; }
        private string ClientSecret { get; set; }


        private  string LoginEndpoint { get; set; }
        private  string ApiEndpoint { get; set; }





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
            HttpClient _httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(LoginEndpoint),
                Content = content
            };
            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            SalesforceAuthentificationResponse responseDyn = JsonConvert.DeserializeObject<SalesforceAuthentificationResponse>(response);

            return responseDyn;
        }




        //public string getProducts()
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var restRequest = InstanceUrl + "/services/data/v49.0/sobjects/product__c";


        //        var request = new HttpRequestMessage(HttpMethod.Get, restRequest);
        //        request.Headers.Add("Authorization", "Bearer " + AuthToken);
        //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var response = client.SendAsync(request).Result;
        //        var result = response.Content.ReadAsStringAsync().Result;
        //        return result;
        //    }
        //}

        //public string addMessage(string id, int quantity, string name, string custId)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var restRequest = InstanceUrl + "/services/data/v49.0/sobjects/ProductEvent__e";

        //        //Arrange
        //        var pd = new Models.PlatformEvent();
        //        pd.Quantity__c = quantity.ToString();
        //        pd.ProductID__c = id;
        //        pd.ProductName__c = name;
        //        pd.CustomerId__c = "XXXX";

        //        var json = JsonConvert.SerializeObject(pd);
        //        //construct content to send
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        var request = new HttpRequestMessage(HttpMethod.Post, restRequest);
        //        request.Headers.Add("Authorization", "Bearer " + AuthToken);
        //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        request.Content = content;

        //        var response = client.SendAsync(request).Result;
        //        var result = response.Content.ReadAsStringAsync().Result;
        //        return result;
        //    }
        //}


        public async Task PublishEventAsync()
        {


            await Login();

        }
    }
}