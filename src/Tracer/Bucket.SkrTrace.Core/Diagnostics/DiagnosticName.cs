
using System;

namespace Bucket.SkrTrace.Core.Diagnostics
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