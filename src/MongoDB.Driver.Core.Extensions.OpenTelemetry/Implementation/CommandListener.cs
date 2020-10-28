using System.Diagnostics;
using System.Net;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Instrumentation;
using OpenTelemetry.Trace;

namespace MongoDB.Driver.Core.Extensions.OpenTelemetry.Implementation
{
    internal class CommandListener : ListenerHandler
    {
        private readonly ActivitySourceAdapter _activitySource;
        private readonly MongoDBInstrumentationOptions _options;

        public CommandListener(ActivitySourceAdapter activitySource, MongoDBInstrumentationOptions options)
            : base(DiagnosticsActivityEventSubscriber.ActivityName)
        {
            _activitySource = activitySource;
            _options = options;
        }

        public override void OnStartActivity(Activity activity, object payload)
        {
            if (!(payload is CommandStartedEvent message))
            {
                return;
            }

            activity.DisplayName = $"mongodb.{message.CommandName}";

            _activitySource.Start(activity, ActivityKind.Client);

            if (activity.IsAllDataRequested)
            {
                activity.AddTag("db.type", "mongo");
                activity.AddTag("db.instance", message.DatabaseNamespace.DatabaseName);
                var endPoint = message.ConnectionId?.ServerId?.EndPoint;
                switch (endPoint)
                {
                    case IPEndPoint ipEndPoint:
                        activity.AddTag("db.user", $"mongodb://{ipEndPoint.Address}:{ipEndPoint.Port}");
                        activity.AddTag("net.peer.ip", ipEndPoint.Address.ToString());
                        activity.AddTag("net.peer.port", ipEndPoint.Port.ToString());
                        break;
                    case DnsEndPoint dnsEndPoint:
                        activity.AddTag("db.user", $"mongodb://{dnsEndPoint.Host}:{dnsEndPoint.Port}");
                        activity.AddTag("net.peer.name", dnsEndPoint.Host);
                        activity.AddTag("net.peer.port", dnsEndPoint.Port.ToString());
                        break;
                }

                if (_options.CaptureCommandText)
                {
                    activity.AddTag("db.statement", message.Command.ToString());
                }
            }
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            _activitySource.Stop(activity);
        }

        public override void OnException(Activity activity, object payload)
        {
            if (!(payload is CommandFailedEvent message))
            {
                return;
            }

            if (activity.IsAllDataRequested)
            {
                activity.SetStatus(Status.Error.WithDescription(message.Failure.Message));
                activity.AddTag("error.type", message.Failure.GetType().FullName);
                activity.AddTag("error.msg", message.Failure.Message);
                activity.AddTag("error.stack", message.Failure.StackTrace);
            }

            _activitySource.Stop(activity);
        }
    }
}