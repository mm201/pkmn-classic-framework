using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace PkmnFoundations.Support
{
    public class AssertHelper
    {
        public static void Assert(bool condition, String message)
        {
            if (!condition) LogHelper.Write(message, EventLogEntryType.Error);

#if DEBUG
            Debug.Assert(condition, message);
#endif
        }

        public static void Assert(bool condition)
        {
            Assert(condition, "Assert failed.");
        }

        public static void Unreachable()
        {
            Assert(false, "Assert failed: Unreachable code has been reached.");
        }

        public static void Equals<T>(T first, T second) where T : IEquatable<T>
        {
            Assert(((IEquatable<T>)first).Equals((IEquatable<T>)second), "Assert failed: Values are not equal.");
        }
    }
}
