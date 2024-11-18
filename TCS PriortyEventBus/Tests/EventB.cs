namespace TCS.PriortyEventBus.Tests {
    /// <summary>
    /// Represents an event carrying an integer value.
    /// </summary>
    public class EventB {
        /// <summary>
        /// Gets the value associated with this EventB instance.
        /// </summary>
        public int Value { get; }
        /// <summary>
        /// EventB is a simple event class that encapsulates an integer value.
        /// This class is used to demonstrate event handling by passing a single integer value to subscribers.
        /// </summary>
        public EventB(int value) {
            Value = value;
        }
    }
}