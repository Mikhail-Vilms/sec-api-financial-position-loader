using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SecApiReportDataLoader.Services
{
    public class SecApiClientService
    {
        private readonly HttpClient _httpClient;

        public SecApiClientService()
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

        public async Task<string> RetrieveCashFlowValuesByEndDate(string cik, string financialPosition)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://data.sec.gov/api/xbrl/companyconcept/{cik}/us-gaap/{financialPosition}.json");

            var response = await _httpClient.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }
    }
}
