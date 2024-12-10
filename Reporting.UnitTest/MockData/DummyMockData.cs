using Reporting.Domain;
using System.Collections.Generic;

namespace Reporting.UnitTest.MockData
{
    public class DummyMockData
    {
        public IEnumerable<Dummy> GetDummyData()
        {
            var dummyData = new List<Dummy>();
            dummyData.Add(new Dummy
            {
                Name = "Test 1"
            });

            dummyData.Add(new Dummy
            {
                Name = "Test 2"
            });

            dummyData.Add(new Dummy
            {
                Name = "Test 3"
            });

            return dummyData;
        }
    }
}