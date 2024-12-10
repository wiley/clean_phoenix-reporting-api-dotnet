using Reporting.Domain;
using Reporting.Domain.Pagination;
using Reporting.Services;
using Reporting.Services.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.UnitTest.Services
{
    public class PaginationServiceTest
    {
        private readonly IPaginationService<Dummy> _service;

        public PaginationServiceTest()
        {
            _service = new PaginationService<Dummy>();
        }

        [Test]
        public async Task PaginationTestAsync()
        {
            string[] stringArray = new string[] { "Teste" };
            var filter = new Filter() { FieldName = "Name", Values = stringArray };
            var listFilter = new List<Filter>();
            listFilter.Add(filter);

            var pageRequest = new PageRequest()
            {
                PageOffset = 0,
                PageSize = 1,
                SortField = "Description",
                SortOrder = EnumSortOrder.Ascending,
                Filters = listFilter
            };

            var result = await _service.ApplyPaginationAsync(MockData.ForPaginationMockData.GetDummy().AsQueryable(), pageRequest);

            Assert.IsNotNull(result);
            Assert.AreEqual("Description", result.FirstOrDefault().Description);
        }

        [Test]
        public async Task PaginationNoFilterTestAsync()
        {
            var listFilter = new List<Filter>();
            var pageRequest = new PageRequest()
            {
                PageOffset = 0,
                PageSize = 1,
                SortField = "Description",
                SortOrder = EnumSortOrder.Ascending,
                Filters = listFilter
            };

            var result = await _service.ApplyPaginationAsync(MockData.ForPaginationMockData.GetDummy().AsQueryable(), pageRequest);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task PaginationSortDescendingTestAsync()
        {
            string[] stringArray = new string[] { "Teste" };
            var filter = new Filter() { FieldName = "Name", Values = stringArray };
            var listFilter = new List<Filter>();
            listFilter.Add(filter);

            var pageRequest = new PageRequest()
            {
                PageOffset = 0,
                PageSize = 1,
                SortField = "Description",
                SortOrder = EnumSortOrder.Descending,
                Filters = listFilter
            };

            var result = await _service.ApplyPaginationAsync(MockData.ForPaginationMockData.GetDummy().AsQueryable(), pageRequest);

            Assert.IsNotNull(result);

            Assert.AreEqual("Description2", result.FirstOrDefault().Description);
        }

        [Test]
        public async Task PaginationOtherSortTestAsync()
        {
            string[] stringArray = new string[] { "Teste" };
            var filter = new Filter() { FieldName = "Name", Values = stringArray };
            var listFilter = new List<Filter>();
            listFilter.Add(filter);

            var pageRequest = new PageRequest()
            {
                PageOffset = 0,
                PageSize = 1,
                SortField = "Description",
                SortOrder = (EnumSortOrder)2,
                Filters = listFilter
            };

            var result = await _service.ApplyPaginationAsync(MockData.ForPaginationMockData.GetDummy().AsQueryable(), pageRequest);

            Assert.IsNotNull(result);
        }
    }
}