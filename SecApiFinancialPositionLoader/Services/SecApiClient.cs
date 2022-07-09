using SecApiFinancialPositionLoader.IServices;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SecApiFinancialPositionLoader.Services
{
    public class SecApiClient : ISecApiClient
    {
        // https://www.sec.gov/edgar/sec-api-documentation
        // Example: https://data.sec.gov/api/xbrl/companyconcept/CIK0000055067/us-gaap/NetCashProvidedByUsedInInvestingActivities.json
        // The company-concept API returns all the XBRL disclosures from a single company (CIK) and concept (a taxonomy and tag) into a single JSON file,
        // with a separate array of facts for each units on measure that the company has chosen to disclose
        private readonly string _companyConceptUrl = "https://data.sec.gov/api/xbrl/companyconcept/{0}/us-gaap/{1}.json";
        private readonly HttpClient _httpClient;

        public SecApiClient()
        {
            _httpClient = new HttpClient();

            _httpClient
                .DefaultRequestHeaders
                .UserAgent
                .Add(new ProductInfoHeaderValue("ScraperBot", "1.0"));
            _httpClient
                .DefaultRequestHeaders
                .UserAgent
                .Add(new ProductInfoHeaderValue("(+http://www.example.com/ScraperBot.html)"));
        }

        public async Task<string> RetrieveValuesForFinancialPosition(
            string cikNumber,
            string tickerSymbol,
            string financialPosition,
            Action<string> logger)
        {
            string targetUrl = string.Format(_companyConceptUrl, cikNumber, financialPosition);
            logger($"Sending GET request to the target url to retrieve company concept(financia position) for [{tickerSymbol}/{cikNumber}/{financialPosition}]: {targetUrl}");

            var request = new HttpRequestMessage(HttpMethod.Get, targetUrl);
            var response = await _httpClient.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }
    }
}
