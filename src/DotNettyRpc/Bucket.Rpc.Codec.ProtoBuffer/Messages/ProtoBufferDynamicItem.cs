using Bucket.Rpc.Codec.ProtoBuffer.Utilitys;
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

            //如果是简单类型则取短名称，否则取长名称。
            if (code != TypeCode.Object)
                TypeName = valueType.FullName;
            else
                TypeName = valueType.AssemblyQualifiedName;

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

            return SerializerUtilitys.Deserialize(Content, Type.GetType(TypeName));
        }

        #endregion Public Method
    }
}
