using UnityEngine;
namespace TCS.PriortyEventBus.Tests {
    /// <summary>
    /// The SubscriberB class is a subscriber designed to handle events
    /// in the priority event bus system.
    /// </summary>
    public class SubscriberB {
        /// <summary>
        /// Method to handle EventA.
        /// This method will be triggered when an EventA is published to the event bus.
        /// It logs the event's message to the debug console.
        /// </summary>
        /// <param name="evt">The EventA instance containing event data.</param>
        [Subscribe(0)] // Lower priority
        public void OnEventA(EventA evt) {
            Debug.Log($"SubscriberB received EventA with message: {evt.Message}");
        }

        /// Handles the EventB when it is published.
        /// <param name="evt">An instance of EventB that contains information about the event.</param>
        [Subscribe(2)] // Higher priority
        public void OnEventB(EventB evt) {
            Debug.Log($"SubscriberB received EventB with value: {evt.Value}");
        }
    }
}