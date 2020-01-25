using System;

namespace nGrpc.MatchMakeService
{
    public class PlayerIsAlreadyInRoomException : Exception
    {
        public PlayerIsAlreadyInRoomException()
        {
        }

        public PlayerIsAlreadyInRoomException(string message) : base(message)
        {
        }
    }

    public class RoomIsClosedException : Exception
    {
        public RoomIsClosedException()
        {
        }

        public RoomIsClosedException(string message) : base(message)
        {
        }
    }
}
