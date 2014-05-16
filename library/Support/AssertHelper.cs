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
            if (!condition) try
            {
                using (StreamWriter s = File.CreateText("assert.log"))
                {
                    s.Write("Assert failed: ");
                    s.WriteLine(message);
                    s.Write("Date: ");
                    s.WriteLine(DateTime.Now.ToString("G"));
                    s.Write("Stack trace: ");
                    s.WriteLine(new StackTrace(true).ToString());
                    s.WriteLine();
                }
            }
            catch (Exception)
            {
                // directory not found or no write permissions there
            }

            Debug.Assert(condition, message);
        }

        public static void Assert(bool condition)
        {
            Assert(condition, "Assert failed.");
        }

        public static void Unreachable()
        {
            Assert(false, "Unreachable code has been reached.");
        }

        public static void Equals<T>(T first, T second) where T : IEquatable<T>
        {
            Assert(((IEquatable<T>)first).Equals((IEquatable<T>)second), "Values are not equal.");
        }
    }
}
