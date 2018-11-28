using Bucket.Config.Abstractions;
using System.Collections.Generic;

namespace Bucket.Config.Implementation
{
    public static class DataChangeListenerDictionary
    {
        private static List<IDataChangeListener> _listeners = new List<IDataChangeListener>();
        public static void Add(IDataChangeListener dataChangeListener)
        {
            _listeners.Add(dataChangeListener);
        }
        public static List<IDataChangeListener> ToList()
        {
            return _listeners;
        }
    }
}
