using Reporting.Domain;
using Reporting.Domain.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Services.Interfaces
{
    public interface IPowerBiService
    {
        Task<Dictionary<string, string>> GetCklsInformations(int userId, string accessToken);
        Task<string> GetAzureToken();
        Task<Dictionary<string, string>> GetEmbedUrl(string azureAccessToken, string groupId, string reportId, string reportSectionId);
        Task<string> GenerateToken(string azureAccessToken, string datasetId, string groupId, string reportId, string dbName, string adminId);
    }
}