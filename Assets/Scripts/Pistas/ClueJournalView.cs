using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este sí es MonoBehaviour porque probablemente controle la UI del Canvas
public class ClueJournalView : MonoBehaviour
{
    private ClueJournal _journal;
    private IGameEventBus _bus;

    public void Construct(ClueJournal journal, IGameEventBus bus)
    {
        _journal = journal;
        _bus = bus;
        Debug.Log("[ClueJournalView] UI conectada correctamente con el Journal y el EventBus.");
    }
}
