using Reporting.Domain;
using Reporting.Domain.Pagination;
using Reporting.Infrastructure.Interface.Mongo;
using Reporting.Services;
using Reporting.Services.Interfaces;
using Reporting.UnitTest.MockData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace Reporting.UnitTest.Services
{
    public class PowerBiServiceTest
    {
        private readonly Mock<ILogger<PowerBiService>> _logPowerBiService;

        public PowerBiServiceTest()
        {
            _logPowerBiService = new Mock<ILogger<PowerBiService>>();
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetAzureTokenTest()
        {
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When(Environment.GetEnvironmentVariable("AZURE_URL"))
                    .Respond("application/json", "{'access_token' : 'This is a test token'}"); // Respond with JSON
            var httpClient = mockHttp.ToHttpClient();
            PowerBiService service = new PowerBiService(_logDummyService.Object, httpClient);
            var result = await service.GetAzureToken();
            Assert.AreEqual("This is a test token", result);
        }

        [Test]
        public async Task GetEmbedUrlTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            string groupId = "123";
            string reportId = "456";
            string reportSectionId = "789";
            string dummyAzureToken = "DummyToken";

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp
                    .When(Environment.GetEnvironmentVariable("POWERBI_API_URL") + groupId + "/reports/" + reportId)
                    .Respond("application/json", "{'embedUrl' : 'http://embedurl.com'}"); // Respond with JSON
            var httpClient = mockHttp.ToHttpClient();
            PowerBiService service = new PowerBiService(_logDummyService.Object, httpClient);
            var result = await service.GetEmbedUrl(dummyAzureToken, groupId, reportId, reportSectionId);
            Assert.AreEqual("http://embedurl.com:80/?pageName=ReportSection" + reportSectionId, result);
        }

        [Test]
        public async Task GetAccessTokenTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            string groupId = "123";
            string reportId = "456";
            string dummyAzureToken = "DummyToken";

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp
                    .When(Environment.GetEnvironmentVariable("POWERBI_API_URL") + groupId + "/reports/" + reportId + "/GenerateToken")
                    .Respond("application/json", "{'token' : 'This is a test token'}"); // Respond with JSON
            var httpClient = mockHttp.ToHttpClient();
            PowerBiService service = new PowerBiService(_logDummyService.Object, httpClient);
            var result = await service.GenerateToken(dummyAzureToken, groupId, reportId);
            Assert.AreEqual("This is a test token", result);
        }
    }
}