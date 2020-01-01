using nGrpc.ServerCommon;
using NSubstitute;
using System;

namespace nGrpc.UnitTests
{
    public class TimerMock : ITimer
    {
        public ITimer SpyTimer = Substitute.For<ITimer>();
        public Action Callback;


        public bool Change(int dueTime, int period)
        {
            SpyTimer.Change(dueTime, period);
            return true;
        }

        public bool Change(long dueTime, long period)
        {
            SpyTimer.Change(dueTime, period);
            return true;
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            SpyTimer.Change(dueTime, period);
            return true;
        }

        public void SetCallback(Action callback)
        {
            SpyTimer.SetCallback(callback);
            Callback = callback;
        }

        public bool Stop()
        {
            SpyTimer.Stop();
            return true;
        }
    }
}
