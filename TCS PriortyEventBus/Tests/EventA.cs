namespace TCS.PriortyEventBus.Tests {
    /// <summary>
    /// EventA represents an event with a message string.
    /// This event can be posted to the event bus and processed by subscribers.
    /// </summary>
    public class EventA {
        /// <summary>
        /// Gets the message associated with the EventA.
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// Represents an event that contains a message.
        /// </summary>
        public EventA(string message) {
            Message = message;
        }
    }
}