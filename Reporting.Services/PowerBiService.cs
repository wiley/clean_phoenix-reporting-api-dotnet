using Reporting.Domain;
using Reporting.Infrastructure.Interface.Mongo;
using Reporting.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Reporting.Services
{
    public class PowerBiService : IPowerBiService
    {
        private readonly HttpClient _client;
        private readonly ILogger<PowerBiService> _logger;

        public PowerBiService(ILogger<PowerBiService> logger, HttpClient client)
        {
            this._logger = logger;
            this._client = client;
        }

        public async Task<Dictionary<string, string>> GetCklsInformations(int userId, string accessToken)
        {
            using (var requestMessage =
                      new HttpRequestMessage(HttpMethod.Get, Environment.GetEnvironmentVariable("USER_API_URL") + "users/" + userId + "/user-mappings?platformName=ck"))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await this._client.SendAsync(requestMessage);
                
                if ((int)response.StatusCode != 200) {
                    throw new Exception("Not able to retrieve CKLS informations");
                }
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
                dynamic platformData = JsonConvert.DeserializeObject<dynamic>(responseData.items[0]["platformData"].ToString());

                Dictionary<string, string> returnValue = new Dictionary<string, string>() {
                    {"adminId", platformData["platformDataUserId"].ToString()},
                    {"dbName", responseData.items[0]["platformCustomer"].ToString()}
                };
                return returnValue;
            }
        }

        public async Task<string> GetAzureToken()
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", Environment.GetEnvironmentVariable("GRANT_TYPE") },
                { "client_id", Environment.GetEnvironmentVariable("CLIENT_ID") },
                { "client_secret", Environment.GetEnvironmentVariable("CLIENT_SECRET") },
                { "resource", Environment.GetEnvironmentVariable("RESOURCE") }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await this._client.PostAsync(Environment.GetEnvironmentVariable("AZURE_URL"), content);

            var responseString = await response.Content.ReadAsStringAsync();
            if ((int)response.StatusCode != 200) {
                throw new Exception("Not able to retrieve azure token.");
            }
            dynamic responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
            return responseData.access_token;
        }

        public async Task<Dictionary<string, string>> GetEmbedUrl(string azureAccessToken, string groupId, string reportId, string reportSectionId)
        {
            using (var requestMessage =
                      new HttpRequestMessage(HttpMethod.Get, Environment.GetEnvironmentVariable("POWERBI_API_URL") + groupId + "/reports/" + reportId))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", azureAccessToken);
                
                var response = await this._client.SendAsync(requestMessage);
                if ((int)response.StatusCode != 200) {
                    throw new Exception("The groupdId or reportId value are incorrect.");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic responseData = JsonConvert.DeserializeObject<dynamic>(responseString);

                var uriBuilder = new UriBuilder((string)responseData.embedUrl);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["pageName"] = "ReportSection" + reportSectionId;
                uriBuilder.Query = query.ToString();
                Dictionary<string, string> returnValue = new Dictionary<string, string>() {
                    {"embedUrl", uriBuilder.ToString()},
                    {"datasetId", responseData.datasetId.ToString()}
                };
                return returnValue;
            }
        }

        public async Task<string> GenerateToken(string azureAccessToken, string datasetId, string groupId, string reportId, string dbName, string adminId)
        {
            using (var requestMessage =
            new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("POWERBI_API_URL") + groupId + "/reports/" + reportId + "/GenerateToken"))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", azureAccessToken);
                // We need to change the username with the authorization to retrieve dbName and adminId
                var data = @"{
                    ""datasets"" : [
                        {
                        ""id"" : """ + datasetId + @"""
                    }],
                    ""accessLevel"" : ""View"",
                    ""identities"" : [{
                        ""username"" : """ + dbName + @":" + adminId + @""",
                        ""roles"" : [""CKLS:ADMIN""],
                        ""datasets"" : [""" + datasetId + @"""]
                    }]
                }";
                
                requestMessage.Content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await this._client.SendAsync(requestMessage);
                if ((int)response.StatusCode != 200) {
                    throw new Exception("The groupdId or reportId value are incorrect.");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
                return responseData.token;
            }
        }
    }
}