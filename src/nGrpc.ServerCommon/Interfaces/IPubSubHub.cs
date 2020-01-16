using System;

namespace nGrpc.ServerCommon
{
    public interface IPubSubHub
    {
        void Subscribe<T>(object subscriber, Action<T> handler);
        void Unsubscribe(object subscriber);
        void Unsubscribe<T>(object subscriber, Action<T> handler);
        void Publish<T>(T message);
    }
}
