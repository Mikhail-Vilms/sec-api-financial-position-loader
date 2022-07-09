using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using SecApiFinancialPositionLoader.IServices;
using SecApiFinancialPositionLoader.Models;
using System;
using System.Threading.Tasks;

namespace SecApiFinancialPositionLoader.Services
{
    public class LambdaInvocationHandler : ILambdaInvocationHandler
    {
        private readonly IDeserializer _deserializer;
        private readonly IFinancialPositionLoader _financialPositionLoader;

        public LambdaInvocationHandler(
            IDeserializer deserializer,
            IFinancialPositionLoader financialPositionLoader)
        {
            _deserializer = deserializer;
            _financialPositionLoader = financialPositionLoader;
        }

        public async Task FunctionHandler(SQSEvent sqsEvnt, ILambdaContext context)
        {
            foreach (var msg in sqsEvnt.Records)
            {
                await ProcessMessageAsync(msg, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage msg, ILambdaContext context)
        {
            void Log(string logMessage)
            {
                context.Logger.LogLine($"[RequestId: {context.AwsRequestId}]: {logMessage}");
            }

            Log($">>>>> Processing message {msg.Body}");

            LambdaTriggerMessage triggerMessage = null;
            try
            {
                triggerMessage = _deserializer.Get(msg);
            }
            catch (Exception ex)
            {
                Log($"Failed to deserialize lambda's trigger message: {ex}");
                return;
            }

            try
            {
                await _financialPositionLoader.Load(triggerMessage, Log);
            }
            catch (Exception ex)
            {
                Log($"Failed to process message: triggerMessage; msg: {ex}");
            }

            Log($"Finished processing. <<<<<");
        }
    }
}
