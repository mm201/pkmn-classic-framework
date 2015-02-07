using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Data
{
    internal abstract class DbParameterFactoryBase
    {
        internal DbParameterFactoryBase()
        {

        }

        internal abstract DbParameter CreateParameter(String name, object value, Type t);
        internal abstract DbParameter CloneParameter(DbParameter orig);
    }
}
