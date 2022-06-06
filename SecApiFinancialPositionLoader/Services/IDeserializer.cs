using Amazon.Lambda.SQSEvents;
using SecApiFinancialPositionLoader.Models;

namespace SecApiFinancialPositionLoader.Services
{
    public interface IDeserializer
    {
        LambdaTriggerMessage Get(SQSEvent.SQSMessage sqsMessage);
    }
}
