using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameEventBus : IGameEventBus
{
    private readonly Dictionary<Type, Delegate> _handlers = new();

    public void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
    {
        if (_handlers.TryGetValue(typeof(TEvent), out var d))
        {
            ((Action<TEvent>)d)?.Invoke(gameEvent);
        }
    }

    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
    {
        var t = typeof(TEvent);

        _handlers[t] = _handlers.TryGetValue(t, out var existing)
            ? Delegate.Combine(existing, handler)
            : handler;
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
    {
        var t = typeof(TEvent);

        if (_handlers.TryGetValue(t, out var existing))
        {
            _handlers[t] = Delegate.Remove(existing, handler);
        }
    }
}
