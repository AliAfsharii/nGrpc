using System;
using System.Collections.Generic;
using System.Text;

namespace nGrpc.ServerCommon
{
    public interface ITimerProvider
    {
        ITimer GetNewTimer();
    }

    public interface ITimer
    {
        void SetCallback(Action callback);
        bool Change(int dueTime, int period);
        bool Change(long dueTime, long period);
        bool Change(TimeSpan dueTime, TimeSpan period);
        bool Stop();
    }
}
