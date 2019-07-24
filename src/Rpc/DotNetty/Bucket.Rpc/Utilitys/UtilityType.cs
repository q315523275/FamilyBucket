using Newtonsoft.Json.Linq;
using System;

namespace Bucket.Rpc.Utilitys
{
    public static class UtilityType
    {
        public static Type JObjectType = typeof(JObject);

        public static Type JArrayType = typeof(JArray);

        public static Type ObjectType = typeof(Object);

        public static Type ConvertibleType = typeof(IConvertible);
    }
}
