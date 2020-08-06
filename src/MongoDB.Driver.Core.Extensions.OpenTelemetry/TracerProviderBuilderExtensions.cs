using System;
using OpenTelemetry.Trace;

namespace MongoDB.Driver.Core.Extensions.OpenTelemetry
{
    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddMongoDBInstrumentation(this TracerProviderBuilder builder)
            => builder.AddMongoDBInstrumentation(null);

        public static TracerProviderBuilder AddMongoDBInstrumentation(this TracerProviderBuilder builder, Action<MongoDBInstrumentationOptions> configureInstrumentationOptions)
        {
            configureInstrumentationOptions ??= opt => { };

            var options = new MongoDBInstrumentationOptions();

            configureInstrumentationOptions(options);

            return builder.AddInstrumentation(t => new MongoDBCommandAdapter(t, options));
        }
    }
}