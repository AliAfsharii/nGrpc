using System;

namespace nGrpc.Common
{
    /// <summary>
    /// All keys should be in lower case
    /// </summary>
    public enum HeaderKeys { credential }

    public class PlayerCredentials
    {
        public int PlayerId { get; set; }
        public Guid SessionId { get; set; }
    }
}
