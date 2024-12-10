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
using System.Threading.Tasks;

namespace Reporting.UnitTest.Services
{
    public class DummyServiceTest
    {
        private readonly IDummyService _service;
        private readonly ServiceProvider _services;
        private readonly Mock<IMongoRepository<Dummy>> _repository;
        private readonly Mock<ILogger<DummyService>> _logDummyService;
        private readonly IPaginationService<Dummy> _paginationDummy;

        public DummyServiceTest()
        {
            _services = new DIServices().GenerateDependencyInjection();
            _repository = new Mock<IMongoRepository<Dummy>>();
            _logDummyService = new Mock<ILogger<DummyService>>();
            _paginationDummy = _services.GetService<IPaginationService<Dummy>>();
            _service = new DummyService(_repository.Object, _logDummyService.Object, _paginationDummy);
        }

        [SetUp]
        public void Setup()
        {
            _repository.Setup(m => m.AsQueryable()).Returns(new MockData.DummyMockData().GetDummyData().AsQueryable());
        }

        [Test]
        public async Task LoadDummyData_DummyShouldExist()
        {
            var dummies = await _service.LoadDummyData();
            Assert.IsNotNull(dummies);
            Assert.IsTrue(dummies.Count() > 0);
        }

        [Test]
        public async Task LoadDummyData_DummyShouldNotExist()
        {
            _repository.Setup(m => m.AsQueryable()).Returns(new List<Dummy>().AsQueryable());
            var dummies = await _service.LoadDummyData();
            Assert.IsNotNull(dummies);
            Assert.IsTrue(dummies.Count() == 0);
        }

        [Test]
        public async Task FindDummysFilterName_ShouldReturnData()
        {
            _repository.Setup(m => m.AsQueryable()).Returns(new DummyMockData().GetDummyData().AsQueryable());
            var filters = new List<Filter>();
            filters.Add(new Filter
            {
                FieldName = "Name",
                Values = new string[] { "Test 1" }
            });
            var request = new PageRequest()
            {
                PageOffset = 0,
                PageSize = 15,
                Filters = filters
            };
            var dummies = await _service.FindDummyData(request);
            Assert.IsNotNull(dummies);
            Assert.IsTrue(dummies.Count() >= 1);
        }

        [Test]
        public async Task FindDummysFilterName_ShouldReturnNoData()
        {
            var filters = new List<Filter>();
            filters.Add(new Filter
            {
                FieldName = "Name",
                Values = new string[] { "ShouldNotExist" }
            });
            var request = new PageRequest()
            {
                PageOffset = 1,
                PageSize = 15,
                Filters = filters
            };
            var dummies = await _service.FindDummyData(request);
            Assert.IsNotNull(dummies);
            Assert.IsTrue(dummies.Count() == 0);
        }

        [Test]
        public void FindDummyById_Exception_NullQueryable()
        {
            _repository.Setup(m => m.AsQueryable()).Throws(new System.Exception());
            Assert.ThrowsAsync<System.Exception>(async () => await _service.LoadDummyData());
        }
    }
}