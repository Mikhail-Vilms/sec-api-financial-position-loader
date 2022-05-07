using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using SecApiReportDataLoader.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiReportDataLoader.Repositories
{
    public class DynamoRepository
    {
        private string _tableName = "sec-api-company-concepts";

        public async Task SaveReportNumbersByDate(
            string cik,
            string tag,
            string ticker,
            Dictionary<string, string> valuesByDate)
        {
            using var ddbClient = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            var dynamoTable = Table.LoadTable(ddbClient, _tableName, true);

            var newItem = new DynamoDbItem
            {
                cik = cik,
                tag = tag,
                ticker = ticker,
                ValuesByDate = valuesByDate
            };

            var documentJson = JsonSerializer.Serialize(newItem);
            var document = Document.FromJson(documentJson);

            await dynamoTable.PutItemAsync(document);
        }
    }
}
