using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueJournal
{
    private IGameEventBus _bus;

    public ClueJournal(IGameEventBus bus)
    {
        _bus = bus;
        // Simulamos la auto-suscripción
        _bus.Subscribe<ClueCollectedEvent>(OnClueCollected);
    }

    private void OnClueCollected(ClueCollectedEvent ev)
    {
        UnityEngine.Debug.Log($"[ClueJournal] Pista registrada en el diario: {ev.ClueData.clueName}");
    }
}
