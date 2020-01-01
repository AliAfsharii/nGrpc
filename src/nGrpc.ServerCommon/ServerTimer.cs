using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace nGrpc.ServerCommon
{
    public class TimerProvider : ITimerProvider
    {
        public ITimer CreateTimer()
        {
            return new ServerTimer();
        }
    }

    public class ServerTimer : ITimer
    {
        Timer _timer;

        public ServerTimer()
        {

        }

        public bool Change(int dueTime, int period)
        {
            return _timer.Change(dueTime, period);
        }

        public bool Change(long dueTime, long period)
        {
            return _timer.Change(dueTime, period);
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            return _timer.Change(dueTime, period);
        }

        public void SetCallback(Action callback)
        {
            _timer = new Timer(x => callback());
        }

        public bool Stop()
        {
            return _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
