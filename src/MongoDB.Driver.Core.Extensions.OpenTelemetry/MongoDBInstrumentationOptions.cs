﻿namespace MongoDB.Driver.Core.Extensions.OpenTelemetry
{
    public class MongoDBInstrumentationOptions
    {
        public bool CaptureCommandText { get; set; }
        public string DisplayName { get; set; }
    }
}