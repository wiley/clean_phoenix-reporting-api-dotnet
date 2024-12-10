using Reporting.Domain;
using Reporting.Domain.Pagination;
using Reporting.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;

namespace Reporting.API.Controllers
{
    [Route("api/v{version:apiVersion}/dummy")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class DummyController : ControllerBase
    {
        private readonly ILogger<DummyController> _logger;
        private readonly IDummyService _service;

        public DummyController(ILogger<DummyController> logger, IDummyService service)
        {
            this._logger = logger;
            this._service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Dummy>), 200)]
        public ActionResult<IEnumerable<Dummy>> Get()
        {
            return Ok(_service.LoadDummyData().Result);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public ActionResult Create()
        {
            _service.InsertDummyData();
            _logger.LogDebug("Created Dummy Data");
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(200)]
        public ActionResult Update()
        {
            _service.UpdateDummyData();
            _logger.LogDebug("Created Dummy Data");
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<Dummy>), 200)]
        [Route("GetDummyData")]
        public ActionResult<IEnumerable<Dummy>> Get(PageRequest request)
        {
            var filters = new StringBuilder();
            request.Filters.ForEach(filter =>
            {
                filters.AppendLine($"{filter.FieldName} = {string.Join(", ", filter.Values)}");
            });
            _logger.LogInformation($"Retrieving Dummy Data - Filters: {filters}");
            return Ok(_service.FindDummyData(request).Result);
        }
    }
}