using Reporting.Domain;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.UnitTest.MockData
{
    public class ForPaginationMockData
    {
        private static readonly Dummy dummyItem = new()
        {
            Id = Guid.NewGuid(),
            Name = "Teste",
            Description = "Description"
        };

        private static readonly Dummy dummyItem1 = new()
        {
            Id = Guid.NewGuid(),
            Name = "Teste",
            Description = "Description1"
        };

        private static readonly Dummy dummyItem2 = new()
        {
            Id = Guid.NewGuid(),
            Name = "Teste",
            Description = "Description2"
        };

        public static MongoDB.Driver.Linq.IMongoQueryable<Dummy> GetDummy()
        {
            var profile = new List<Dummy>();
            profile.Add(dummyItem);
            profile.Add(dummyItem1);
            profile.Add(dummyItem2);

            return new MongoQueryable<Dummy>() { MockData = profile };
        }
    }
}