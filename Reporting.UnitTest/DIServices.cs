using Reporting.Services;
using Reporting.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Reporting.UnitTest
{
    public class DIServices
    {
        public ServiceProvider GenerateDependencyInjection()
        {
            var services = new ServiceCollection();
            services.AddScoped(typeof(IPaginationService<>), typeof(PaginationService<>));

            return services
                .AddLogging()
                .BuildServiceProvider();
        }
    }
}