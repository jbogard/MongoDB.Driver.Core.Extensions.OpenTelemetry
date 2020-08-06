using System;
using OpenTelemetry.Trace;

namespace MongoDB.Driver.Core.Extensions.OpenTelemetry
{
    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddMongoDBAdapter(this TracerProviderBuilder builder)
            => builder.AddMongoDBAdapter(null);

        public static TracerProviderBuilder AddMongoDBAdapter(this TracerProviderBuilder builder, Action<MongoDBInstrumentationOptions> configureInstrumentationOptions)
        {
            configureInstrumentationOptions ??= opt => { };

            var options = new MongoDBInstrumentationOptions();

            configureInstrumentationOptions(options);

            return builder.AddInstrumentation(t => new MongoDBCommandAdapter(t, options));
        }
    }
}