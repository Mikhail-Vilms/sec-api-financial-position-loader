using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System.Threading.Tasks;

namespace SecApiFinancialPositionLoader.IServices
{
    public interface ILambdaInvocationHandler
    {
        public Task FunctionHandler(SQSEvent sqsEvnt, ILambdaContext context);
    }
}
