using System;

namespace nGrpc.Common
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
        public Guid SecretKey { get; set; }
    }
    public class LoginRes
    {
        public int PlayerId { get; set; }
        public Guid SessionId { get; set; }
    }

    public class ChangeNameReq
    {
        public string NewName { get; set; }
    }
    public class ChangeNameRes
    {
        public PlayerData PlayerData { get; set; }
    }
}
