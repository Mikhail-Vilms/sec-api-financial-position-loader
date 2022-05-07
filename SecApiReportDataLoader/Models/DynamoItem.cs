using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace SecApiReportDataLoader.Models
{
    [DynamoDBTable("sec-api-company-concepts")]
    public class DynamoDbItem
    {
        [DynamoDBHashKey("cik")]
        public string cik { get; set; }

        [DynamoDBRangeKey("tag")]
        public string tag { get; set; }

        [DynamoDBProperty("ticker")]
        public string ticker { get; set; }

        public Dictionary<string, string> ValuesByDate { get; set; }
    }
}
