using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue
{
    public string clueName;
}

// Este es el evento concreto que viajará por EventBus
public class ClueCollectedEvent : IGameEvent
{
    public Clue ClueData { get; private set; }

    public ClueCollectedEvent(Clue clue)
    {
        ClueData = clue;
    }
}
