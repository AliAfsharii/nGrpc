using System;
namespace nGrpc.ServerCommon
{
    public interface ITime
    {
        DateTime UTCTime { get; }
    }
}
