using System;

namespace OpenTelemetry.Trace
{
    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddMongoDBInstrumentation(this TracerProviderBuilder builder)
            => builder.AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources");
    }
}