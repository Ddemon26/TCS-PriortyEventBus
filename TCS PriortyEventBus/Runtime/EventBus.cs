using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace TCS.PriortyEventBus {
    /// <summary>
    /// The EventBus class serves as a central hub for managing event subscriptions and dispatching.
    /// It allows objects to register themselves to listen for specific types of events and ensures that
    /// those events are dispatched to all relevant subscribers when posted.
    /// </summary>
    public class EventBus {
        /// <summary>
        /// Handler for exceptions that occur during event propagation.
        /// </summary>
        /// <remarks>
        /// This Action delegate takes an Exception object as a parameter and is used to handle any
        /// exceptions thrown by subscriber methods during event dispatch.
        /// Default behavior logs exceptions using UnityEngine.Debug.LogException.
        /// </remarks>
        readonly Action<Exception> m_exceptionHandler;

        /// <summary>
        /// The EventBus class allows for registering, unregistering, and posting events to subscribers.
        /// It ensures thread-safe access to the subscriber list and supports priority-based event handling.
        /// </summary>
        public EventBus() => m_exceptionHandler = Debug.LogException;

        /// <summary>
        /// The EventBus class allows for registering, unregistering, and posting events to subscribers.
        /// It ensures thread-safe access to the subscriber list and supports priority-based event handling.
        /// </summary>
        public EventBus(Action<Exception> exceptionHandler) => m_exceptionHandler = exceptionHandler;

        /// <summary>
        /// A dictionary maintaining a list of subscribers for each event type.
        /// The key is the event type, and the value is a list of subscribers
        /// associated with that event type.
        /// </summary>
        readonly Dictionary<Type, List<Subscriber>> m_subscribers = new();

        /// <summary>
        /// Registers an object to the EventBus to listen for events. All methods annotated with event-specific attributes
        /// within the object will be added to the EventBus and invoked when the corresponding event is posted.
        /// </summary>
        /// <param name="obj">The object containing methods to register as event listeners.</param>
        public void Register(object obj) {
            lock (m_subscribers) {
                var type = obj.GetType();
                while (type != null) {
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);

                    foreach (var method in methods) {
                        var subscribeAttr = method.GetCustomAttribute<SubscribeAttribute>();
                        if (subscribeAttr == null)
                            continue;

                        // Check method return type is void
                        if (method.ReturnType != typeof(void))
                            throw new ArgumentException($"@Subscribe method \"{method}\" cannot return a value");

                        // Check method has exactly one parameter
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length != 1)
                            throw new ArgumentException($"@Subscribe method \"{method}\" must take exactly 1 argument");

                        // Check method is not static
                        if (method.IsStatic)
                            throw new ArgumentException($"@Subscribe method \"{method}\" cannot be static");

                        var parameterType = parameters[0].ParameterType;

                        // Check parameter is not primitive
                        if (parameterType.IsPrimitive)
                            throw new ArgumentException($"@Subscribe method \"{method}\" cannot subscribe to primitives");

                        // Check parameter is not abstract or interface
                        if (parameterType.IsAbstract || parameterType.IsInterface)
                            throw new ArgumentException($"@Subscribe method \"{method}\" cannot subscribe to polymorphic classes");

                        // Check that the method name is "On" + EventName
                        string preferredName = "On" + parameterType.Name;
                        if (method.Name != preferredName)
                            throw new ArgumentException($"Subscribed method {method} should be named {preferredName}");

                        // Check that no superclass of parameterType is already subscribed
                        var baseType = parameterType.BaseType;
                        while (baseType != null) {
                            if (m_subscribers.ContainsKey(baseType)) {
                                throw new ArgumentException($"@Subscribe method \"{method}\" cannot subscribe to class which inherits from subscribed class \"{baseType}\"");
                            }

                            baseType = baseType.BaseType;
                        }

                        // Create delegate
                        Action<object> action = null;
                        try {
                            // Create a strongly typed delegate
                            var delegateType = typeof(Action<>).MakeGenericType(parameterType);
                            var del = Delegate.CreateDelegate(delegateType, obj, method);
                            action = (o) => del.DynamicInvoke(o);
                        }
                        catch (Exception e) {
                            Debug.LogWarning($"Unable to create delegate for method {method}: {e}");
                        }

                        var subscriber = new Subscriber(obj, method, subscribeAttr.Priority, action);

                        // Add subscriber to dictionary
                        if (!m_subscribers.TryGetValue(parameterType, out List<Subscriber> subscriberList)) {
                            subscriberList = new List<Subscriber>();
                            m_subscribers[parameterType] = subscriberList;
                        }

                        subscriberList.Add(subscriber);

                        // Re-order the list
                        subscriberList.Sort
                        (
                            (a, b) => {
                                int priorityCompare = b.Priority.CompareTo(a.Priority); // Higher priority first
                                return priorityCompare != 0 ? priorityCompare : string.Compare(a.Object.GetType().Name, b.Object.GetType().Name, StringComparison.Ordinal);
                            }
                        );

                        Debug.Log($"Registering {parameterType} - {subscriber}");
                    }

                    type = type.BaseType;
                }
            }
        }

        /// <summary>
        /// Registers a new subscriber for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to subscribe to.</typeparam>
        /// <param name="subFn">The action to invoke when the event is posted.</param>
        /// <param name="priority">The priority of the subscriber. Higher priority subscribers are invoked first.</param>
        /// <returns>Returns the created <see cref="Subscriber"/> object.</returns>
        public Subscriber Register<T>(Action<T> subFn, float priority = 0) {
            var parameterType = typeof(T);
            lock (m_subscribers) {
                Action<object> action = new Action<object>(o => subFn((T)o));
                var subscriber = new Subscriber(subFn.Target, subFn.Method, priority, action);

                if (!m_subscribers.TryGetValue(parameterType, out List<Subscriber> subscriberList)) {
                    subscriberList = new List<Subscriber>();
                    m_subscribers[parameterType] = subscriberList;
                }

                subscriberList.Add(subscriber);

                // Re-order the list
                subscriberList.Sort
                (
                    (a, b) => {
                        int priorityCompare = b.Priority.CompareTo(a.Priority); // Higher priority first
                        return priorityCompare != 0 ? priorityCompare : string.Compare
                        (
                            a.Object.GetType().Name, b.Object.GetType().Name, StringComparison.Ordinal
                        );
                    }
                );

                return subscriber;
            }
        }

        /// <summary>
        /// Unregisters an object from the event bus, removing all associated subscribers.
        /// </summary>
        /// <param name="obj">The object to unregister.</param>
        public void Unregister(object obj) {
            lock (m_subscribers) {
                foreach (List<Subscriber> subscriberList in m_subscribers.Values) {
                    subscriberList.RemoveAll(s => s.Object == obj);
                }
            }
        }

        /// <summary>
        /// Unregisters a subscriber from the event bus.
        /// </summary>
        /// <param name="sub">The subscriber to be unregistered.</param>
        public void Unregister(Subscriber sub) {
            if (sub == null)
                return;

            lock (m_subscribers) {
                foreach (List<Subscriber> subscriberList in m_subscribers.Values) {
                    subscriberList.Remove(sub);
                }
            }
        }

        /// <summary>
        /// Posts an event to all registered subscribers.
        /// </summary>
        /// <param name="evt">The event object to be posted.</param>
        public void Post(object evt) {
            var eventType = evt.GetType();
            List<Subscriber> subscriberList;

            lock (m_subscribers) {
                if (!m_subscribers.TryGetValue(eventType, out subscriberList))
                    return;

                subscriberList = new List<Subscriber>(subscriberList); // Make a copy to avoid modification during iteration
            }

            foreach (var subscriber in subscriberList) {
                try {
                    subscriber.Invoke(evt);
                }
                catch (Exception e) {
                    m_exceptionHandler(e);
                }
            }
        }
    }
}