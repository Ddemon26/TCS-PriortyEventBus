using UnityEngine;
namespace TCS.PriortyEventBus.Tests {
    /// <summary>
    /// SubscriberA is a class that defines event handling methods for EventA and EventB.
    /// The methods are subscribed to the event bus with specified priority levels.
    /// </summary>
    public class SubscriberA {
        /// Method handling EventA subscription.
        /// The method processes the EventA and performs actions based on the event data.
        /// <param name="evt">Instance of EventA containing event data.</param>
        [Subscribe(1)] // Higher priority
        public void OnEventA(EventA evt) {
            Debug.Log($"SubscriberA received EventA with message: {evt.Message}");
        }

        /// Method to handle the EventB type event.
        /// The method is marked with the Subscribe attribute prioritizing it within the event handling system.
        /// This method logs a message indicating the reception of EventB along with its associated value.
        /// <param name="evt">The EventB instance carrying the event data.</param>
        [Subscribe(0)] // Lower priority
        public void OnEventB(EventB evt) {
            Debug.Log($"SubscriberA received EventB with value: {evt.Value}");
        }
    }
}