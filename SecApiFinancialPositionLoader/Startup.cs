using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SecApiFinancialPositionLoader.IServices;
using SecApiFinancialPositionLoader.Services;

namespace SecApiFinancialPositionLoader
{
    public static class Startup
    {
        internal static IHostBuilder SetupHostForLambda(this IHostBuilder hostBuilder) => hostBuilder
            .AddRuntimeDependenciesBinding();

        private static IHostBuilder AddRuntimeDependenciesBinding(this IHostBuilder hostBuilder) => hostBuilder
            .ConfigureServices((context, serviceCollection) => serviceCollection
                .AddSingleton<ILambdaInvocationHandler, LambdaInvocationHandler>()
                .AddSingleton<IDeserializer, Deserializer>()
                .AddSingleton<IFinancialPositionLoader, FinancialPositionLoader>()
                .AddSingleton<ISecApiClient, SecApiClient>()
                .TryAddSingleton<IDynamoDBContext>(provider =>
                {
                    var client = new AmazonDynamoDBClient(RegionEndpoint.USWest2);
                    return new DynamoDBContext(client, new DynamoDBContextConfig()
                    {
                        ConsistentRead = false
                    });
                }));
    }
}
