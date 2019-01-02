using Bucket.SkrTrace.Core.Abstractions;
using System;

namespace Bucket.SkrTrace.Core
{
    public class RuntimeEnvironment : IRuntimeEnvironment
    {
        public static IRuntimeEnvironment Instance { get; } = new RuntimeEnvironment();

        public string ApplicationCode { get; set; }
        public bool Initialized => !string.IsNullOrWhiteSpace(ApplicationCode);
        public Guid AgentUUID { get; } = Guid.NewGuid();
    }
}
