using System;
namespace TCS.PriortyEventBus {
    /// Attribute used to mark methods for subscription to events within an event bus system.
    /// This attribute allows the annotated method to be registered and invoked when an event
    /// of the corresponding type is published.
    /// Methods annotated with this attribute must adhere to certain constraints:
    /// - The return type must be void.
    /// - They must accept exactly one parameter.
    /// - The parameter type must not be primitive, abstract, or an interface.
    /// - The annotated method must not be static.
    /// - The method name should follow the convention "On" + EventName.
    /// - Only one subscriber per event type is allowed within a single class hierarchy.
    /// Properties:
    /// - Priority: Determines the order in which subscribers are notified of events.
    /// Higher priority subscribers are notified first.
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeAttribute : Attribute {
        /// <summary>
        /// Specifies the priority level assigned to a method marked with the <see cref="SubscribeAttribute"/>.
        /// </summary>
        /// <remarks>
        /// This property is utilized to determine the sequence in which methods are executed when multiple methods subscribe to the same event.
        /// Higher priority values will cause methods to be executed before those with lower priority values.
        /// </remarks>
        public float Priority { get; }

        /// Attribute used to mark methods for subscription to events within an event bus system.
        /// This attribute allows the annotated method to be registered and invoked when an event
        /// of the corresponding type is published.
        /// Methods annotated with this attribute must adhere to certain constraints:
        /// - The return type must be void.
        /// - They must accept exactly one parameter.
        /// - The parameter type must not be primitive, abstract, or an interface.
        /// - The annotated method must not be static.
        /// - The method name should follow the convention "On" + EventName.
        /// - Only one subscriber per event type is allowed within a single class hierarchy.
        /// Properties:
        /// - Priority: Determines the order in which subscribers are notified of events.
        /// Higher priority subscribers are notified first.
        public SubscribeAttribute(float priority = 0) => Priority = priority;
    }
}