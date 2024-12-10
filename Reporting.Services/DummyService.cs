using Reporting.Domain;
using Reporting.Domain.Pagination;
using Reporting.Infrastructure.Interface.Mongo;
using Reporting.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Services
{
    public class DummyService : IDummyService
    {
        private readonly IMongoRepository<Dummy> _dummyRepository;
        private readonly IPaginationService<Dummy> _paginationService;
        private readonly ILogger<DummyService> _logger;

        public DummyService(IMongoRepository<Dummy> dummyRepository, ILogger<DummyService> logger
            , IPaginationService<Dummy> paginationService)
        {
            this._dummyRepository = dummyRepository;
            this._logger = logger;
            this._paginationService = paginationService;
        }

        public async Task InsertDummyData()
        {
            var id = Guid.NewGuid();
            await _dummyRepository.InsertOneAsync(new Dummy
            {
                Name = $"Test - {id}"
            });
            _logger.LogInformation($"Inserted Dummy Data - {id}");
        }

        public async Task<IEnumerable<Dummy>> LoadDummyData()
        {
            return await Task.Run<IEnumerable<Dummy>>(() => _dummyRepository.AsQueryable().Take(10).ToList());
        }

        public void UpdateDummyData()
        {
            var firstRecord = _dummyRepository.AsQueryable().FirstOrDefault();
            _logger.LogInformation($"Updating ID: {firstRecord.Id}, Name: {firstRecord.Name}");
            if (firstRecord != null)
            {
                firstRecord.Name = "Test Updated";
                _dummyRepository.ReplaceOne(firstRecord);
            }
        }

        public async Task<IEnumerable<Dummy>> FindDummyData(PageRequest request)
        {
            if (string.IsNullOrEmpty(request.SortField))
            {
                request.SortField = "Name";
            }
            return await Task.Run(() =>
                _paginationService.ApplyPaginationAsync(_dummyRepository.AsQueryable(), request)
            );
        }
    }
}