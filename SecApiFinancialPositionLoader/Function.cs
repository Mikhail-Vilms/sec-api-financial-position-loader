using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using SecApiFinancialPositionLoader.Models;
using SecApiFinancialPositionLoader.Services;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SecApiFinancialPositionLoader
{
    public class Function
    {
        private readonly IDeserializer _deserializer;
        private readonly Loader _loader;

        public Function()
        {
            _deserializer = new Deserializer();
            _loader = new Loader();
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (var msg in evnt.Records)
            {
                await ProcessMessageAsync(msg, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage msg, ILambdaContext context)
        {
            void Log(string logMsg)
            {
                context.Logger.LogLine($"[RequestId: {context.AwsRequestId}]: {logMsg}");
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
                await _loader.Load(triggerMessage, Log);
            }
            catch (Exception ex)
            {
                Log($"Failed to process message: triggerMessage; msg: {ex}");
            }

            Log($"Finished processing. <<<<<");
        }
    }
}
