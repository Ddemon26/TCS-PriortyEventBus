using UnityEngine;
namespace TCS.PriortyEventBus.Tests {
    /// <summary>
    /// The <c>EventBusExample</c> class is part of the TCS.PriorityEventBus.Tests namespace.
    /// It inherits from MonoBehaviour and provides an example of how to use the priority event bus system.
    /// </summary>
    public class EventBusExample : MonoBehaviour {
        /// <summary>
        /// Initializes the event bus and registers subscribers.
        /// Posts events of type EventA and EventB for demonstration purposes.
        /// </summary>
        void Start() {
            var eventBus = new EventBus();

            var subscriberA = new SubscriberA();
            var subscriberB = new SubscriberB();

            eventBus.Register(subscriberA);
            eventBus.Register(subscriberB);

            // Post EventA
            eventBus.Post(new EventA("Hello World"));

            // Post EventB
            eventBus.Post(new EventB(42));
        }
    }
}