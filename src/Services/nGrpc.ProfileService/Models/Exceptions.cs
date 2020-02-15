using System;

namespace nGrpc.ProfileService
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException()
        {
        }

        public LoginFailedException(string message) : base(message)
        {
        }
    }

    public class PlayerHasAlreadyLoginedException : Exception
    {
        public PlayerHasAlreadyLoginedException()
        {
        }

        public PlayerHasAlreadyLoginedException(string message) : base(message)
        {
        }
    }
}
