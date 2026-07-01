using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ReadableNote : MonoBehaviour, IInteractable
{
    // Usamos el evento Clue
    private Clue _clue = new Clue { clueName = "Nota del diario de la hermana" };
    private IGameEventBus _bus;

    // Este método será llamado por el GameInstaller
    public void Construct(IGameEventBus bus)
    {
        _bus = bus;
    }

    public string Prompt => "Leer nota";

    public void Interact(PlayerMotor player)
    {
        if (_bus != null)
        {
            Debug.Log($"Leyendo: {_clue.clueName}. Publicando evento al bus...");
            _bus.Publish(new ClueCollectedEvent(_clue));
        }
        else
        {
            Debug.LogError("Error: El EventBus no fue inyectado en esta nota.");
        }
    }
}
