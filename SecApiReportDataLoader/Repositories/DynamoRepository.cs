using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using SecApiReportDataLoader.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiReportDataLoader.Repositories
{
    public class DynamoRepository
    {
        private readonly string _tableName = "Sec-Api-Data";

        public async Task SaveFinancialStatementNumbersByDate(
            LambdaTriggerMessage triggerMsg,
            CompanyConceptDto companyConceptDto,
            Dictionary<string, string> valuesByDate,
            Action<string> logger)
        {
            using var ddbClient = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            var dynamoTable = Table.LoadTable(ddbClient, _tableName, true);

            logger($"Saving to Dynamo: PartitionKey: {triggerMsg.CikNumber}; SortKey: {triggerMsg.FinancialStatement}_{triggerMsg.FinancialPosition}");
            var newItem = new
            {
                PartitionKey = triggerMsg.CikNumber,
                SortKey = $"{triggerMsg.FinancialStatement}_{triggerMsg.FinancialPosition}",
                DisplayName = companyConceptDto.Label,
                Description = companyConceptDto.Description,
                Taxanomy = companyConceptDto.Taxonomy,
                TickerSymbol = triggerMsg.TickerSymbol,
                CompanyName = companyConceptDto.EntityName,
                ValuesByDate = valuesByDate
            };

            var documentJson = JsonSerializer.Serialize(newItem);
            var document = Document.FromJson(documentJson);

            await dynamoTable.PutItemAsync(document);
        }
    }
}
