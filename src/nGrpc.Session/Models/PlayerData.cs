using System;
using System.Collections.Generic;
using System.Text;

namespace nGrpc.Sessions
{
    public class PlayerData
    {
        public int Id { get; set; }
        public Guid SecretKey { get; set; }
    }
}
