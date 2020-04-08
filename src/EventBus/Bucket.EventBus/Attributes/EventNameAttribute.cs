using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.EventBus.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class EventNameAttribute : Attribute
    {
        public string EventName
        {
            get { return _eventName; }
        }

        private string _eventName { get; set; }

        public EventNameAttribute(string eventName)
        {
            _eventName = eventName;
        }
    }
}
