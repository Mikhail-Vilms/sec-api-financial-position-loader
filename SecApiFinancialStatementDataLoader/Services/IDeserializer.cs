using Amazon.Lambda.SQSEvents;
using SecApiFinancialStatementDataLoader.Models;

namespace SecApiFinancialStatementDataLoader.Services
{
    public interface IDeserializer
    {
        LambdaTriggerMessage Get(SQSEvent.SQSMessage sqsMessage);
    }
}
