using Reporting.Infrastructure.Interface.Mongo;

namespace Reporting.Infrastructure.Mongo
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}