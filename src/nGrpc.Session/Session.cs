using nGrpc.Common;
using nGrpc.ServerCommon;
using Nito.AsyncEx;
using System;

namespace nGrpc.Sessions
{
    internal class Session
    {
        public Guid Id { get; } = Guid.NewGuid();
        public PlayerData PlayerData { get; private set; }
        public ITimer Timer { get; private set; }

        private AsyncLock _asyncLock { get; } = new AsyncLock();

        internal Session(PlayerData playerData, ITimer timer)
        {
            PlayerData = playerData;
            Timer = timer;
        }


        public AwaitableDisposable<IDisposable> LockAsync()
        {
            return _asyncLock.LockAsync();
        }

        public IDisposable Lock()
        {
            return _asyncLock.Lock();
        }
    }
}
