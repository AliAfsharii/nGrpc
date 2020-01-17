using PubSub;
using System;
namespace nGrpc.ServerCommon
{
    public class PubSubHub : IPubSubHub
    {
        private readonly Hub hub;

        public PubSubHub()
        {
            hub = new Hub();
        }

        public void Publish<T>(T message)
        {
            hub.Publish<T>(message);
        }

        public void Subscribe<T>(object subscriber, Action<T> handler)
        {
            hub.Subscribe<T>(subscriber, handler);
        }

        public void Unsubscribe(object subscriber)
        {
            hub.Unsubscribe(subscriber);
        }

        public void Unsubscribe<T>(object subscriber, Action<T> handler)
        {
            hub.Unsubscribe<T>(subscriber, handler);
        }
    }
}
