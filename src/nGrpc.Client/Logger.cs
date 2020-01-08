using System;

namespace nGrpc.Client
{
    public static class ClientLogger
    {
        public static ILogger Logger = new DefaultLogger();
    }

    public interface ILogger
    {
        void LogInfo(string messag);
        void LogInfo(string message, params object[] args);

        void LogWarning(string message);
        void LogWarning(string message, params object[] args);

        void LogError(string message);
        void LogError(string message, params object[] args);

        void LogError(Exception exception, string message);
        void LogError(Exception exception, string message, params object[] args);
    }

    public class DefaultLogger : ILogger
    {
        public void LogError(string message, params object[] args)
        {
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
        }

        public void LogError(string message)
        {
        }

        public void LogError(Exception exception, string message)
        {
        }

        public void LogInfo(string message, params object[] args)
        {
        }

        public void LogInfo(string messag)
        {
        }

        public void LogWarning(string message, params object[] args)
        {
        }

        public void LogWarning(string message)
        {
        }
    }
}
