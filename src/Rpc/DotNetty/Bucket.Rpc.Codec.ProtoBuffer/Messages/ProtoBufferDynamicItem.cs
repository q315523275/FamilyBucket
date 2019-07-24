using Bucket.Rpc.Codec.ProtoBuffer.Utilitys;
using Bucket.Rpc.Utilitys;
using Newtonsoft.Json;
using ProtoBuf;
using System;

namespace Bucket.Rpc.Codec.ProtoBuffer.Messages
{
    [ProtoContract]
    public class ProtoBufferDynamicItem
    {
        #region Constructor

        public ProtoBufferDynamicItem()
        {
        }

        public ProtoBufferDynamicItem(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var valueType = value.GetType();
            var code = Type.GetTypeCode(valueType);

            if (code != TypeCode.Object)
                TypeName = valueType.FullName;
            else
                TypeName = valueType.AssemblyQualifiedName;

            if (valueType == UtilityType.JObjectType || valueType == UtilityType.JArrayType)
                Content = SerializerUtilitys.Serialize(value.ToString());
            else
                Content = SerializerUtilitys.Serialize(value);
        }

        #endregion Constructor

        #region Property

        [ProtoMember(1)]
        public string TypeName { get; set; }

        [ProtoMember(2)]
        public byte[] Content { get; set; }

        #endregion Property

        #region Public Method

        public object Get()
        {
            if (Content == null || TypeName == null)
                return null;

            var typeName = Type.GetType(TypeName);
            if (typeName == null)
            {
                return SerializerUtilitys.Deserialize<object>(Content);
            }
            else if (typeName == UtilityType.JObjectType || typeName == UtilityType.JArrayType)
            {
                var content = SerializerUtilitys.Deserialize<string>(Content);
                return JsonConvert.DeserializeObject(content, typeName);
            }
            else
            {
                return SerializerUtilitys.Deserialize(Content, typeName);
            }
        }

        #endregion Public Method
    }
}
