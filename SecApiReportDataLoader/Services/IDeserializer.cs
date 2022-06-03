using Amazon.Lambda.SQSEvents;
using SecApiReportDataLoader.Models;

namespace SecApiReportDataLoader.Services
{
    public interface IDeserializer
    {
        LambdaTriggerMessage Get(SQSEvent.SQSMessage sqsMessage);
    }
}
