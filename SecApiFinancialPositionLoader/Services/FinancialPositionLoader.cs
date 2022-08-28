using Amazon.DynamoDBv2.DataModel;
using SecApiFinancialPositionLoader.Heplers;
using SecApiFinancialPositionLoader.IServices;
using SecApiFinancialPositionLoader.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiFinancialPositionLoader.Services
{
    /// <summary>
    /// The class is responsible of retrieving data for the specific financial position and loading it into the Dynamo table.
    /// </summary>
    public class FinancialPositionLoader : IFinancialPositionLoader
    {
        private readonly ISecApiClient _secApiClient;
        private readonly IDynamoDBContext _dynamoDbContext;

        public FinancialPositionLoader(
            ISecApiClient secApiClient,
            IDynamoDBContext dynamoDbContext)
        {
            _secApiClient = secApiClient;
            _dynamoDbContext = dynamoDbContext;
        }

        /// <inheritdoc />
        public async Task Load(
            FinancialPostitionInfo financialPostitionInfo,
            Action<string> logger)
        {
            // Send HTTP request to SEC API to fetch financial data
            logger($"Fetching financial position data from SEC API: {financialPostitionInfo}");
            string secApiResponseJson = await _secApiClient
                .RetrieveValuesForFinancialPosition(
                    financialPostitionInfo.CikNumber,
                    financialPostitionInfo.TickerSymbol,
                    financialPostitionInfo.FinancialPosition,
                    logger);


            // Deserialize SEC API response
            logger($"Trying to deserialize SEC API response: {financialPostitionInfo}");
            SecApiCompanyConceptResponseDto companyConceptDto = JsonSerializer
                .Deserialize<SecApiCompanyConceptResponseDto>(
                    secApiResponseJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });


            // Mapping to Dynamo-table-item format:
            logger($"Mapping SEC API Response to Dynamo-item format: {financialPostitionInfo}");
            FinancialPositionDynamoItem dynamoItem = FinancialPositionMapper
                .Map(financialPostitionInfo, companyConceptDto);


            // Saving new item to Dynamo:
            logger($"Saving new item to Dynamo table: {financialPostitionInfo}");
            await _dynamoDbContext.SaveAsync(dynamoItem);


            logger($"{dynamoItem.Facts.Count} numbers have been saved to Dynamo: [Partition Key: {dynamoItem.PartitionKey} / Sort Key: {dynamoItem.SortKey}]");
        }
    }
}
