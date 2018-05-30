
using System;

namespace Bucket.Tracing.Diagnostics
{
    public class DiagnosticName :Attribute
    {
        public string Name { get; }

        public DiagnosticName(string name)
        {
            Name = name;
        }
    }
}