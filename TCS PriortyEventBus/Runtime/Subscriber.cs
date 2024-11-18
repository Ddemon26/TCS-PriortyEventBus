using System;
using System.Reflection;
namespace TCS.PriortyEventBus {
    /// <summary>
    /// Represents a subscriber object which holds a method to be invoked.
    /// </summary>
    public class Subscriber {
        /// <summary>
        /// Gets the object associated with the subscriber instance.
        /// </summary>
        public object Object { get; }
        /// <summary>
        /// Gets the MethodInfo object representing the method associated with this subscriber.
        /// </summary>
        public MethodInfo Method { get; }
        /// <summary>
        /// Gets the priority level of the subscriber. Determines the order of execution
        /// with higher numbers indicating higher priority.
        /// </summary>
        public float Priority { get; }
        /// <summary>
        /// Gets the action delegate associated with this subscriber.
        /// </summary>
        public Action<object> Action { get; }

        /// <summary>
        /// Represents a subscriber that holds the method to be invoked along with other relevant information.
        /// </summary>
        public Subscriber(object obj, MethodInfo method, float priority, Action<object> action) {
            Object = obj;
            Method = method;
            Priority = priority;
            Action = action;
        }

        /// <summary>
        /// Invokes the subscriber's action or method using the provided argument.
        /// </summary>
        /// <param name="arg">The argument to pass to the subscriber's action or method.</param>
        public void Invoke(object arg) {
            if (Action != null) {
                Action(arg);
            }
            else {
                Method.Invoke
                (
                    Object, new[] { arg }
                );
            }
        }
    }
}