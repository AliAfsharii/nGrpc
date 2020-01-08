using System;

namespace nGrpc.Common.Models
{
    public class RegisterReq
    {
    }
    public class RegisterRes
    {
        public int PlayerId { get; set; }
        public Guid SecretKey { get; set; }
    }

    public class LoginReq
    {
        public int PlayerId { get; set; }
        public Guid UniqueKey { get; set; }
    }
    public class LoginRes
    {
        public int PlayerId { get; set; }
        public Guid SessionId { get; set; }
    }
}
