using SecApiFinancialStatementDataLoader.Models;
using SecApiFinancialStatementDataLoader.Repositories;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiFinancialStatementDataLoader.Services
{
    public class Loader
    {
        private readonly SecApiClientService _secApiClientService;
        private readonly DynamoRepository _dynamoRepository;

        public Loader()
        {
            _secApiClientService = new SecApiClientService();
            _dynamoRepository = new DynamoRepository();
        }

        public async Task Load(LambdaTriggerMessage triggerMsg, Action<string> logger)
        {
            // Send HTTP Request to SEC API
            string resultJson = await _secApiClientService.RetrieveCashFlowValuesByEndDate(
                triggerMsg.CikNumber,
                triggerMsg.TickerSymbol,
                triggerMsg.FinancialPosition,
                logger);


            // If no result has been returned - skip current tag
            if (resultJson.StartsWith("<"))
            {
                logger("Invalid response that can't be parsed has been received; Skipping current financial position.");
                return;
            }

            // Deserialize SEC response
            CompanyConceptDto companyConceptDto = JsonSerializer
                .Deserialize<CompanyConceptDto>(
                    resultJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            // Transform response to the list of items
            List<CompanyConceptUnitDto> concepts = companyConceptDto.Units["USD"];

            // Extract 10-K numbers
            Dictionary<string, string> valuesFrom10KsByEndDate = Filter10kNumbers(concepts);
            logger($"{valuesFrom10KsByEndDate.Count} numbers have been fetched for the {triggerMsg.FinancialPosition} position for the {triggerMsg.TickerSymbol}/{triggerMsg.CikNumber}");


            // Save to Dynamo
            await _dynamoRepository.SaveFinancialStatementNumbersByDate(
                triggerMsg,
                companyConceptDto,
                valuesFrom10KsByEndDate,
                logger);
        }

        private Dictionary<string, string> Filter10kNumbers(List<CompanyConceptUnitDto> concepts)
        {
            Dictionary<string, string> valuesByEndDate = new Dictionary<string, string>();

            foreach (CompanyConceptUnitDto concept in concepts)
            {
                if (concept.Form != "10-K")
                {
                    continue;
                }

                TimeSpan period = ConvertToNormalDate(concept.End) - ConvertToNormalDate(concept.Start);

                if (period.TotalDays < 330)
                {
                    continue;
                }

                if (valuesByEndDate.ContainsKey(concept.End))
                {
                    continue;
                }

                valuesByEndDate.Add(concept.End, concept.Val.ToString());
            }

            return valuesByEndDate;
        }

        private DateTime ConvertToNormalDate(string cikDate)
        {
            string[] aDate = cikDate.Split("-");
            return new DateTime(Int32.Parse(aDate[0]), Int32.Parse(aDate[1]), Int32.Parse(aDate[2]));
        }
    }
}
