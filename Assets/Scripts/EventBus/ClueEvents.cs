using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueCollectedEvent : IGameEvent
{
    public Clue ClueData { get; }
    public ClueCollectedEvent(Clue clue) => ClueData = clue;
}

public class ClueJournalUpdatedEvent : IGameEvent
{
    public int TotalClues { get; }
    public ClueJournalUpdatedEvent(int totalClues) => TotalClues = totalClues;
}