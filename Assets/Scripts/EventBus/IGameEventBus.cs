using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IGameEvent { }

public interface IGameEventBus
{
    void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent;

    void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent;

    void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent;
}