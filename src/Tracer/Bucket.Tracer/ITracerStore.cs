using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.Tracer
{
    public interface ITracerStore
    {
        Task Post(TraceLogs traceLogs);
    }
}