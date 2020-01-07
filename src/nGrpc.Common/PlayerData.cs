using System;

namespace nGrpc.Common
{
    public class PlayerData
    {
        public int Id { get; set; }
        public Guid SecretKey { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
