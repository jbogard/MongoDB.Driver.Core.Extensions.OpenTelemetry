# MongoDB.Driver.Core.Extensions.OpenTelemetry

![CI](https://github.com/jbogard/MongoDB.Driver.Core.Extensions.OpenTelemetry/workflows/CI/badge.svg)
[![NuGet](https://img.shields.io/nuget/dt/MongoDB.Driver.Core.Extensions.OpenTelemetry.svg)](https://www.nuget.org/packages/MongoDB.Driver.Core.Extensions.OpenTelemetry) 
[![NuGet](https://img.shields.io/nuget/vpre/MongoDB.Driver.Core.Extensions.OpenTelemetry.svg)](https://www.nuget.org/packages/MongoDB.Driver.Core.Extensions.OpenTelemetry)
[![MyGet (dev)](https://img.shields.io/myget/jbogard-ci/v/MongoDB.Driver.Core.Extensions.OpenTelemetry.svg)](https://myget.org/gallery/jbogard-ci)

## Usage

This repo include the package:

 - [MongoDB.Driver.Core.Extensions.OpenTelemetry](https://www.nuget.org/packages/MongoDB.Driver.Core.Extensions.OpenTelemetry/)
 
The `MongoDB.Driver.Core.Extensions.OpenTelemetry` package provides adapters to [OpenTelemetry](https://opentelemetry.io/).

To use `MongoDB.Driver.Core.Extensions.OpenTelemetry`, you need to configure your `MongoClientSettings` to add this MongoDB event subscriber through the [MongoDB.Driver.Core.Extensions.DiagnosticSources](https://www.nuget.org/packages/MongoDB.Driver.Core.Extensions.DiagnosticSources/) package:

```csharp
var clientSettings = MongoClientSettings.FromUrl(mongoUrl);
clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
var mongoClient = new MongoClient(clientSettings);
```

That event subscriber exposes Activity events via a DiagnosticListener under the root activity name, `MongoDB.Driver.Core.Events.Command`. To subscribe, you may use the `DiagnosticListener.AllListeners` observable.

## OpenTelemetry usage

Once you've configured your MongoDB client to expose diagnostics events as above, you can configure OpenTelemetry (typically through the [OpenTelemetry.Extensions.Hosting](https://www.nuget.org/packages/OpenTelemetry.Extensions.Hosting/0.2.0-alpha.275) package).

```csharp
services.AddOpenTelemetry(builder => {
    builder
        // Configure exporters
        .UseZipkin()
        // Configure adapters
        .UseRequestAdapter()
        .UseDependencyAdapter()
        .AddMongoDBAdapter(); // Adds MongoDB OTel support
});
```

By default, the command text is not logged. To change this, configure the options:

```csharp
services.AddOpenTelemetry(builder => {
    builder
        // Configure exporters
        .UseZipkin()
        // Configure adapters
        .UseRequestAdapter()
        .UseDependencyAdapter()
        .AddMongoDBAdapter(opt => opt.CaptureCommandText = true); // Adds MongoDB OTel support
});
```

This package supports the latest released OpenTelemetry beta package on NuGet.
