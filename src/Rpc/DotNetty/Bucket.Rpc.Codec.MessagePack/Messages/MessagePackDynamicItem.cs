using Bucket.Rpc.Codec.MessagePack.Utilities;
using MessagePack;
using System;
using System.Runtime.CompilerServices;

namespace Bucket.Rpc.Codec.MessagePack.Messages
{
    [MessagePackObject]
    public class MessagePackDynamicItem
    {
        #region Constructor

        public MessagePackDynamicItem()
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MessagePackDynamicItem(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var valueType = value.GetType();
            var code = Type.GetTypeCode(valueType);

            if (code != TypeCode.Object && valueType.BaseType != typeof(Enum))
                TypeName = valueType.FullName;
            else
                TypeName = valueType.AssemblyQualifiedName;

            Content = SerializerUtilitys.Serialize(value);
        }

        #endregion Constructor

        #region Property

        [Key(0)]
        public string TypeName { get; set; }

        [Key(1)]
        public byte[] Content { get; set; }
        #endregion Property

        #region Public Method
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Get()
        {
            if (Content == null || TypeName == null)
                return null;

            return SerializerUtilitys.Deserialize(Content, Type.GetType(TypeName));
        }
        #endregion Public Method
    }
}
