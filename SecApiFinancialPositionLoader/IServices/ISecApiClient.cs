using System;
using System.Threading.Tasks;

namespace SecApiFinancialPositionLoader.IServices
{
    public interface ISecApiClient
    {
        public Task<string> RetrieveValuesForFinancialPosition(
            string cikNumber,
            string tickerSymbol,
            string financialPosition,
            Action<string> logger);
    }
}
