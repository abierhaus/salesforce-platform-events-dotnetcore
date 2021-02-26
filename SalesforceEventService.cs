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

    public static class Constants
    {
        public static string USERNAME = "";
        public static string PASSWORD = "";
        public static string TOKEN = "";
        public static string CONSUMER_KEY = "";
        public static string CONSUMER_SECRET = "";
        public static string TOKEN_REQUEST_ENDPOINTURL = "https://login.salesforce.com/services/oauth2/token";
        public static string TOKEN_REQUEST_QUERYURL = "/services/data/v43.0/query?q=select+Id+,name+from+account+limit+10";

    }

    public class SalesforceEventService : ISalesforceEventService
    {
        private const string LOGIN_ENDPOINT = "https://login.salesforce.com/services/oauth2/token";
        private const string API_ENDPOINT = "/services/data/v49.0/";
        private readonly IConfigurationRoot ConfigRoot;


        public SalesforceEventService(IConfiguration configRoot)
        {
            ConfigRoot = (IConfigurationRoot) configRoot;

            // SF requires TLS 1.1 or 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            Username = ConfigRoot.GetSection("Salesforce")["Username"];
            Password = ConfigRoot.GetSection("Salesforce")["Password"];
            Token = ConfigRoot.GetSection("Salesforce")["Token"];

            ClientSecret = ConfigRoot.GetSection("Salesforce")["ClientSecret"];
            ClientId = ConfigRoot.GetSection("Salesforce")["ClientId"];
        }

        private  string Username { get; set; }
        private string Password { get; set; }

        private string Token { get; set; }

        private string ClientId { get; set; }
        private string ClientSecret { get; set; }
        private string AuthToken { get; set; }
        private string InstanceUrl { get; set; }



        public async Task Login()
        {
            //string jsonResponse;
            //using (var client = new HttpClient())
            //{
            //    HttpContent request = new FormUrlEncodedContent(new Dictionary<string, string>
            //    {
            //        {"grant_type", "password"},
            //        {"client_id", ClientId},
            //        {"client_secret", ClientSecret},
            //        {"username", Username},
            //        {"password", Password}
            //    });

            //    request.Headers.Add("X-PrettyPrint", "1");
            //    var response = client.PostAsync(LOGIN_ENDPOINT, request).Result;
            //    jsonResponse = response.Content.ReadAsStringAsync().Result;
            //}

            //Console.WriteLine($"Response: {jsonResponse}");
            //var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
            //AuthToken = values["access_token"];
            //InstanceUrl = values["instance_url"];

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
                RequestUri = new Uri(Constants.TOKEN_REQUEST_ENDPOINTURL),
                Content = content
            };
            var responseMessage = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);


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