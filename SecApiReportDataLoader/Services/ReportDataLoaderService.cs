using SecApiReportDataLoader.Models;
using SecApiReportDataLoader.Repositories;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecApiReportDataLoader.Services
{
    public class ReportDataLoaderService
    {
        private readonly SecApiClientService _secApiClientService;
        private readonly DynamoRepository _dynamoRepository;

        public ReportDataLoaderService()
        {
            _secApiClientService = new SecApiClientService();
            _dynamoRepository = new DynamoRepository();
        }

        public async Task LoadReportData(string cikNumber, string financialPosition, string ticker)
        {
            // Send HTTP Request to SEC API
            string resultJson = await _secApiClientService.RetrieveCashFlowValuesByEndDate(cikNumber, financialPosition);

            // If no result has been returned - skip current tag
            if (resultJson.StartsWith("<"))
            {
                return;
            }

            // Deserialize
            CompanyConceptDto companyConceptDto = JsonSerializer
                .Deserialize<CompanyConceptDto>(
                    resultJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            // Transform to list of items
            List<CompanyConceptUnitDto> concepts = companyConceptDto.Units["USD"];

            // Extract 10-K numbers
            Dictionary<string, string> valuesFrom10KsByEndDate = Filter10kNumbers(concepts);

            // Save to Dynamo
            await _dynamoRepository.SaveReportNumbersByDate(
                cikNumber,
                financialPosition,
                ticker,
                valuesFrom10KsByEndDate);
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
