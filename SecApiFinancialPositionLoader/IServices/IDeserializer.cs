using Amazon.Lambda.SQSEvents;
using SecApiFinancialPositionLoader.Models;

namespace SecApiFinancialPositionLoader.IServices
{
    public interface IDeserializer
    {
        LambdaTriggerMessage Get(SQSEvent.SQSMessage sqsMessage);
    }
}
