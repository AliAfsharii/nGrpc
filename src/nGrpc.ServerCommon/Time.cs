using System;

namespace nGrpc.ServerCommon
{
    internal class Time : ITime
    {
        public DateTime UTCTime => DateTime.UtcNow;
    }
}
