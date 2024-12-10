using Reporting.Domain;
using Reporting.Domain.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Services.Interfaces
{
    public interface IDummyService
    {
        Task InsertDummyData();

        void UpdateDummyData();

        Task<IEnumerable<Dummy>> LoadDummyData();

        Task<IEnumerable<Dummy>> FindDummyData(PageRequest request);
    }
}