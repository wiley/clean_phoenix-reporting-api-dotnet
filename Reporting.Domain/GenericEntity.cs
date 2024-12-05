using Reporting.Domain.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace Reporting.Domain
{

    public abstract class GenericEntity : IDocument
    {

        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        [BsonRequired]
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}