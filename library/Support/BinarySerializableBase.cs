using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PkmnFoundations.Support
{
    /// <summary>
    /// Base class for objects which serialize using BinaryReader and BinaryWriter
    /// </summary>
    public abstract class BinarySerializableBase : ISerializable
    {
        // xxx: ISerializable may be useless or even non-idiomatic on this class.

        public BinarySerializableBase()
        {
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("data", Save());
        }

        protected abstract void Save(BinaryWriter writer);
        protected abstract void Load(BinaryReader reader);

        /// <summary>
        /// Size of the serialized structure in bytes
        /// </summary>
        public abstract int Size
        {
            get;
        }

        public byte[] Save()
        {
            byte[] data = new byte[Size];
            Save(new BinaryWriter(new MemoryStream(data)));
            return data;
        }

        public void Load(byte[] data, int offset)
        {
            if (offset + Size > data.Length) throw new ArgumentOutOfRangeException("offset");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");
            MemoryStream m = new MemoryStream(data, offset, Size);
            Load(new BinaryReader(m));
        }

        public void Load(byte[] data)
        {
            if (Size > data.Length) throw new ArgumentException("Buffer is too small");
            Load(data, 0);
        }
    }
}
