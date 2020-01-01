using System;

namespace nGrpc.Sessions
{
    public class ThereIsNoPlayerDataForSuchPlayerException : Exception
    {
        public ThereIsNoPlayerDataForSuchPlayerException()
        {
        }

        public ThereIsNoPlayerDataForSuchPlayerException(string message) : base(message)
        {
        }
    }
}
