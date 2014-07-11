using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public static class LogHelper
    {
        static LogHelper()
        {
            Type = EventLogTypes.StandardError;
        }

        public static void Write(String message, EventLogEntryType type, int eventID, ushort category, byte[] rawData)
        {
            switch (Type)
            {
                case EventLogTypes.StandardOutput:
                    Console.WriteLine(message);
                    break;
                case EventLogTypes.StandardError:
                    Console.Error.WriteLine(message);
                    break;
                case EventLogTypes.File:
                    try
                    {
                        using (FileStream fs = File.Open(m_filename, FileMode.Append))
                        {
                            StreamWriter sw = new StreamWriter(fs);
                            sw.Write("{0}\t{1}\n", DateTime.Now, message);
                            sw.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Can't open logfile at {0}.\nException: {1}\nMessage: {2}", m_filename, ex.Message, message);
                    }
                    break;
                case EventLogTypes.Windows:
                    m_event_log.WriteEntry(message, type, eventID, (short)category, rawData);
                    break;
            }
        }

        public static void Write(String message, EventLogEntryType type)
        {
            Write(message, type, 0, 0, null);
        }

        public static void Write(String message)
        {
            Write(message, EventLogEntryType.Information, 0, 0, null);
        }

        public static void UseStandardOutput()
        {
            Type = EventLogTypes.StandardOutput;
            m_event_log = null;
        }

        public static void UseStandardError()
        {
            Type = EventLogTypes.StandardError;
            m_event_log = null;
        }

        public static void UseFile(String filename)
        {
            if (filename == null) throw new ArgumentNullException("filename");
            Type = EventLogTypes.File;
            m_filename = filename;
            m_event_log = null;
        }

        public static void UseEventLog(EventLog event_log)
        {
            if (event_log == null) throw new ArgumentNullException("event_log");
            Type = EventLogTypes.Windows;
            m_event_log = event_log;
        }

        public static EventLogTypes Type
        {
            get;
            private set;
        }

        private static String m_filename;
        private static EventLog m_event_log;
    }

    public enum EventLogTypes
    {
        StandardOutput,
        StandardError,
        File,
        Windows
    }
}
