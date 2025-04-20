using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public abstract class EventSubscriberMonoBehaviour : MonoBehaviour
{
    private readonly List<(object target, string eventName, Delegate handler, Action unsubscribe)> _subscriptions = new();

    protected EventSubscription Subscribe() => new EventSubscription(this, _subscriptions);

    protected virtual void OnDestroy()
    {
        foreach (var (_, _, _, unsubscribe) in _subscriptions)
        {
            unsubscribe?.Invoke();
        }
        _subscriptions.Clear();
    }

    public class EventSubscription
    {
        private readonly EventSubscriberMonoBehaviour _owner;
        private readonly List<(object target, string eventName, Delegate handler, Action unsubscribe)> _subscriptions;

        public EventSubscription(EventSubscriberMonoBehaviour owner, List<(object target, string eventName, Delegate handler, Action unsubscribe)> subscriptions)
        {
            _owner = owner;
            _subscriptions = subscriptions;
        }

        public EventSubscription On<TEvent>(TEvent eventAction, Delegate handler)
            where TEvent : Delegate
        {
            var (target, eventName) = ExtractEventInfo(eventAction);
            SubscribeToEvent(target, eventName, handler);
            return this;
        }

        private (object target, string eventName) ExtractEventInfo<TEvent>(TEvent eventAction)
            where TEvent : Delegate
        {
            var methodInfo = eventAction.Method;
            var target = eventAction.Target;

            var declaringType = methodInfo.DeclaringType;
            var events = declaringType.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var eventInfo in events)
            {
                var addMethod = eventInfo.GetAddMethod(true);
                if (addMethod != null && addMethod == methodInfo)
                {
                    return (target, eventInfo.Name);
                }
            }

            var eventName = methodInfo.Name.StartsWith("add_") 
                ? methodInfo.Name.Substring(4) 
                : throw new ArgumentException("Unable to extract event info from delegate.");
            return (target, eventName);
        }

        private void SubscribeToEvent(object target, string eventName, Delegate handler)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Event target cannot be null.");

            var targetType = target.GetType();
            var eventInfo = targetType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (eventInfo == null)
                throw new ArgumentException($"Event {eventName} not found on type {targetType.Name}");

            var eventDelegateType = eventInfo.EventHandlerType;
            if (!eventDelegateType.IsAssignableFrom(handler.GetType()))
                throw new ArgumentException($"Handler type {handler.GetType().Name} is not compatible with event type {eventDelegateType.Name}");

            eventInfo.AddEventHandler(target, handler);

            Action unsubscribe = () =>
            {
                try
                {
                    eventInfo.RemoveEventHandler(target, handler);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to unsubscribe from {eventName} on {targetType.Name}: {ex.Message}");
                }
            };

            _subscriptions.Add((target, eventName, handler, unsubscribe));
        }

        private void LogSubscription(object target, string eventName, Delegate handler)
        {
            Debug.Log($"Subscribed to {eventName} on {target.GetType().Name} with handler {handler.Method.Name}");
        }

        private bool IsAlreadySubscribed(object target, string eventName, Delegate handler)
        {
            return _subscriptions.Any(s => s.target == target && s.eventName == eventName && s.handler == handler);
        }

        public void DebugSubscriptions()
        {
            foreach (var (target, eventName, handler, _) in _subscriptions)
            {
                Debug.Log($"Active subscription: {eventName} on {target.GetType().Name} with {handler.Method.Name}");
            }
        }
    }
}