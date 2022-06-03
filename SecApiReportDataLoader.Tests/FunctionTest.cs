using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using SecApiReportDataLoader;
using Amazon.Lambda.SQSEvents;
using SecApiReportDataLoader.Models;
using System.Text.Json;

namespace SecApiReportDataLoader.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestToUpperFunction()
        {
            LambdaTriggerMessage sqsMessage = new LambdaTriggerMessage()
            {
                CikNumber = "CIK0000200406",
                TickerSymbol = "JNJ",
                FinancialStatement = "CashFlowStatement",
                FinancialPosition = "NetCashProvidedByUsedInInvestingActivities"
            };

            string sqsMessageStr = JsonSerializer.Serialize(sqsMessage);

            var sqsEvent = new SQSEvent
            {
                Records = new List<SQSEvent.SQSMessage>()
                {
                    new SQSEvent.SQSMessage{ Body = sqsMessageStr}
                }
            };



            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            await function.FunctionHandler(sqsEvent, context);

            Assert.Equal("HELLO WORLD", "HELLO WORLD");
        }
    }
}
