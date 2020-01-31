using System;

namespace nGrpc.ReversiGameService
{
    public class DiskOnWrongPositionException : Exception
    {
        public DiskOnWrongPositionException()
        {
        }

        public DiskOnWrongPositionException(string message) : base(message)
        {
        }
    }

    public class WrongPlayerIdException : Exception
    {
        public WrongPlayerIdException()
        {
        }

        public WrongPlayerIdException(string message) : base(message)
        {
        }
    }

    public class ThereIsNoSuchGameException : Exception
    {
        public ThereIsNoSuchGameException()
        {
        }

        public ThereIsNoSuchGameException(string message) : base(message)
        {
        }
    }
}
