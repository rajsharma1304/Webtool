using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Diagnostics;

namespace SICT
{
    public static class SICTLogger
    {
        public static void WriteVerbose(string ClassName, string MethodName, string Message)
        {
            Logger.Write(new LogEntry
            {
                Message = string.Format("{0},{1},{2}", ClassName, MethodName, Message),
                Severity = TraceEventType.Verbose
            });
        }

        public static void WriteInfo(string ClassName, string MethodName, string Message)
        {
            Logger.Write(new LogEntry
            {
                Message = string.Format("{0},{1},{2}", ClassName, MethodName, Message),
                Severity = TraceEventType.Information
            });
        }

        public static void WriteWarning(string ClassName, string MethodName, string Message)
        {
            Logger.Write(new LogEntry
            {
                Message = string.Format("{0},{1},{2}", ClassName, MethodName, Message),
                Severity = TraceEventType.Warning
            });
        }

        public static void WriteError(string ClassName, string MethodName, string Message)
        {
            Logger.Write(new LogEntry
            {
                Message = string.Format("{0},{1},{2}", ClassName, MethodName, Message),
                Severity = TraceEventType.Error
            });
        }

        public static void WriteException(string ClassName, string MethodName, Exception Ex)
        {
            Logger.Write(new LogEntry
            {
                Message = string.Format("{0},{1},{2},{3}", new object[]
                {
                    ClassName,
                    MethodName,
                    Ex.Message.Replace(',', ';'),
                    Ex.StackTrace.Replace(',', ';').Replace("\r\n", ";").Trim()
                }),
                Severity = TraceEventType.Critical
            });
        }
    }
}
