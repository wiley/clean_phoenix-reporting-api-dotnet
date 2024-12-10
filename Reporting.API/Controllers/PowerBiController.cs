using Reporting.API.Responses;
using Reporting.API.InputSerializer;
using Reporting.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;
using Reporting.API.Responses.NonSuccessfullResponses;
using DarwinAuthorization.Models;
using Microsoft.AspNetCore.Authorization;

namespace Reporting.API.Controllers
{
    [Route("api/v{version:apiVersion}/reports/powerbi-access")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class PowerBiController : ControllerBase
    {
        private readonly ILogger<PowerBiController> _logger;
        private readonly IMongoHealthCheckService _mongoHealth;
        private readonly IPowerBiService _service;
        private readonly DarwinAuthorizationContext _darwinAuthorizationContext;

        public PowerBiController(ILogger<PowerBiController> logger, IMongoHealthCheckService mongoHealth, IPowerBiService service,
                                DarwinAuthorizationContext darwinAuthorizationContext)
        {
            this._logger = logger;
            this._mongoHealth = mongoHealth;
            this._service = service;
            this._darwinAuthorizationContext = darwinAuthorizationContext;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(PowerBiResponse), 200)]
        public async Task<IActionResult> Create([FromBody] InputPowerBi content)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("PowerBI Create - {id}, {groupId}, {reportId}, {reportSectionId}",
                this._darwinAuthorizationContext.UserId, content.groupId, content.reportId, content.reportId);
                
                HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues bearerToken);
                string[] sentences = bearerToken.ToString().Split(' ');
                try 
                {
                    var resultFromUserApi = await _service.GetCklsInformations(_darwinAuthorizationContext.UserId, sentences[1]);
                    var azureAccessToken = await _service.GetAzureToken();
                    var resultFromPowerBi = await _service.GetEmbedUrl(azureAccessToken, content.groupId, content.reportId, content.reportSectionId);
                    var accessToken = await _service.GenerateToken(azureAccessToken, resultFromPowerBi["datasetId"], content.groupId, content.reportId, resultFromUserApi["dbName"], resultFromUserApi["adminId"]);
                    return Ok(new PowerBiResponse { access_token = accessToken, embed_url = resultFromPowerBi["embedUrl"] });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "PowerBI Create - Fail - {id}, {groupId}, {reportId}, {reportSectionId}",
                    _darwinAuthorizationContext.UserId, content.groupId, content.reportId, content.reportSectionId);
                    ModelState.AddModelError("Generic", ex.Message);
                    var badRequestMessage = NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState);
                    return BadRequest(badRequestMessage);
                }
            }
            else
            {
                _logger.LogWarning("PowerBI Create - BadRequest - {id}, {groupId}, {reportId}, {reportSectionId}",
                this._darwinAuthorizationContext.UserId, content.groupId, content.reportId, content.reportSectionId);
                var badRequestMessage = NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState);
                return BadRequest(badRequestMessage);
            }
        }
    }
}