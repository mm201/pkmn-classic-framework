using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public abstract class EncodedStringBase
    {
        public EncodedStringBase()
            : this(null)
        {

        }

        public EncodedStringBase(byte[] data)
        {
            RawData = data;
        }

        // todo: move more of the encoded string implementation over here for DRY reasons

        public abstract int Size { get; }
        public abstract string Text { get; set; }
        public abstract byte[] RawData { get; set; }
    }
}
