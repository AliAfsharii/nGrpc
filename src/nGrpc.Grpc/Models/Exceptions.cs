using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace nGrpc.Grpc
{
    public class InvalidCredentialGrpcException : RpcException
    {
        public InvalidCredentialGrpcException() : base(new Status(StatusCode.InvalidArgument, "Invalid Player Credentials"))
        {
        }
        public InvalidCredentialGrpcException(string message) : base(new Status(StatusCode.InvalidArgument, message))
        {
        }
    }

    public class NotLoggedInGrpcException : RpcException
    {
        public NotLoggedInGrpcException() : base(new Status(StatusCode.Unauthenticated, "This player is not logged in"))
        {
        }
        public NotLoggedInGrpcException(string message) : base(new Status(StatusCode.Unauthenticated, message))
        {
        }
    }
}
