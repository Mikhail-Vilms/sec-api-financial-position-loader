using SecApiFinancialPositionLoader.Models;
using System;
using System.Threading.Tasks;

namespace SecApiFinancialPositionLoader.IServices
{
    /// <summary>
    /// Contract for the operation of saving data for the specific financial position into the Dynamo table.
    /// </summary>
    public interface IFinancialPositionLoader
    {
        /// <summary>
        /// Retrieves data for the specific financial position from the SEC API and loads it into the Dynamo table.
        /// </summary>
        public Task Load(
            FinancialPostitionInfo financialPostitionInfo,
            Action<string> Log);
    }
}
