using SecApiFinancialPositionLoader.Models;
using System;
using System.Threading.Tasks;

namespace SecApiFinancialPositionLoader.IServices
{
    public interface IFinancialPositionLoader
    {
        public Task Load(
            LambdaTriggerMessage triggerMessage,
            Action<string> Log);
    }
}
