using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.ConfigCenter.Exceptions
{
    public class RemoteException : Exception
    {
        public RemoteException(string message) : base(message) { }
        public RemoteException(string message, Exception ex) : base(message, ex) { }
    }
}
