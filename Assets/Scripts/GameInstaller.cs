using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Se ejecuta antes que cualquier otro script de la escena.
[DefaultExecutionOrder(-1000)]
public class GameInstaller : MonoBehaviour
{
    private IGameEventBus _bus;
    private ClueJournal _clueJournal;

    private void Awake()
    {
        _bus = new GameEventBus();
        _clueJournal = new ClueJournal(_bus); // se auto-suscribe a ClueCollectedEvent

        InjectEventBusDependents();
        InjectClueJournalView();
    }

    private void InjectEventBusDependents()
    {
        // Busca TODO lo que dependa del bus, sin conocer los tipos concretos.
        var dependents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var dependent in dependents)
        {
            if (dependent is IEventBusDependent busDependent)
                busDependent.Construct(_bus);
        }
    }

    private void InjectClueJournalView()
    {
        // Caso especial: la vista necesita el repositorio Y el bus, no solo
        // el bus, así que no entra en el patrón genérico de arriba.
        var view = FindFirstObjectByType<ClueJournalView>();
        view?.Construct(_clueJournal, _bus);
    }
}