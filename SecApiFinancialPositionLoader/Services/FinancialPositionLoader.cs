using Amazon.DynamoDBv2.DataModel;
using SecApiFinancialPositionLoader.Heplers;
using SecApiFinancialPositionLoader.IServices;
using SecApiFinancialPositionLoader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiFinancialPositionLoader.Services
{
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

        public async Task Load(
            LambdaTriggerMessage triggerMsg,
            Action<string> logger)
        {
            // Send HTTP request to SEC API
            string secResponseJson = string.Empty;
            try
            {
                secResponseJson = await _secApiClient
                  .RetrieveValuesForFinancialPosition(
                      triggerMsg.CikNumber,
                      triggerMsg.TickerSymbol,
                      triggerMsg.FinancialPosition,
                      logger);
            }
            catch (Exception ex)
            {
                logger($"Error occured while fetching financial position data from the SEC API: [{triggerMsg.TickerSymbol}/{triggerMsg.CikNumber}/{triggerMsg.FinancialPosition}]. Exception message: {ex}");
                return;
            }

            // If no result has been returned - skip current tag
            if (string.IsNullOrEmpty(secResponseJson) || secResponseJson.StartsWith("<"))
            {
                logger($"Invalid SEC response that can't be parsed; Skipping current financial position: [{triggerMsg.TickerSymbol}/{triggerMsg.CikNumber}/{triggerMsg.FinancialPosition}].");
                return;
            }

            // Deserialize SEC response
            CompanyConceptDto companyConceptDto = null;
            try
            {
                companyConceptDto = JsonSerializer
                .Deserialize<CompanyConceptDto>(
                    secResponseJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (Exception ex)
            {
                logger($"Error occured while deserializing response from the SEC API: [{triggerMsg.TickerSymbol}/{triggerMsg.CikNumber}/{triggerMsg.FinancialPosition}]. Exception message: {ex}");
                return;
            }

            // Extract financial numbers from the response
            IList<SecApiCompanyFact> facts = null;
            try
            {
                List<CompanyConceptUnitDto> concepts = companyConceptDto.Units["USD"];
                facts = CompanyConceptsHelper.FilterBalanceSheetNumbers(concepts);
                logger($"{facts.Count} numbers(financial facts) have been fetched for the financial position: [{triggerMsg.TickerSymbol}/{triggerMsg.CikNumber}/{triggerMsg.FinancialPosition}]");
            }
            catch (Exception ex)
            {
                logger($"Error occured while extracting financial numbers: [{triggerMsg.TickerSymbol}/{triggerMsg.CikNumber}/{triggerMsg.FinancialPosition}]. Exception message: {ex}");
                return;
            }


            // Saving to Dynamo
            try
            {
                await _dynamoDbContext.SaveAsync(new FinancialPositionDynamoItem()
                {
                    PartitionKey = triggerMsg.CikNumber,
                    SortKey = $"{triggerMsg.FinancialStatement}_{triggerMsg.FinancialPosition}",
                    TickerSymbol = triggerMsg.TickerSymbol,
                    CompanyName = companyConceptDto.EntityName,
                    DisplayName = companyConceptDto.Label,
                    Description = companyConceptDto.Description,
                    Taxanomy = companyConceptDto.Taxonomy,
                    Facts = facts
                        .Select(fact => new SecFact
                            {
                                Form = fact.Form,
                                StartDate = fact.StartDate,
                                EndDate = fact.EndDate,
                                Frame = fact.Frame,
                                Value = fact.Value
                            })
                        .OrderByDescending(secFact => secFact.Frame)
                        .ToList()
                });

                logger($"{facts.Count} numbers have been saved to Dynamo: [Partition Key: {triggerMsg.CikNumber} / Sort Key: {triggerMsg.FinancialStatement}_{triggerMsg.FinancialPosition}]");
            }
            catch (Exception ex)
            {
                logger($"Error occured while saving financial values to Dynamo: [{triggerMsg.TickerSymbol}/{triggerMsg.CikNumber}/{triggerMsg.FinancialPosition}]. Exception message: {ex}");
                return;
            }
        }
    }
}
