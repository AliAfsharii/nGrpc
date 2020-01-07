using System;

namespace nGrpc.ServerCommon
{
    public class Time : ITime
    {
        public DateTime UTCTime => DateTime.UtcNow;
    }
}
