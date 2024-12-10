using Reporting.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Reporting.Domain
{
    [BsonCollection("dummy")]
    [BsonIgnoreExtraElements]
    public class Dummy : GenericEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}