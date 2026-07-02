using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ReadableNote : MonoBehaviour, IInteractable, IEventBusDependent
{
    [SerializeField] private string _clueId = "nota_diario_hermana";
    [SerializeField] private string _clueTitle = "Nota del diario de la hermana";
    [TextArea][SerializeField] private string _clueDescription;

    private IGameEventBus _bus;

    public void Construct(IGameEventBus bus) => _bus = bus;

    public string Prompt => "Leer nota";

    public void Interact(PlayerMotor player)
    {
        if (_bus == null)
        {
            Debug.LogError("Error: El EventBus no fue inyectado en esta nota.");
            return;
        }

        var clue = new Clue
        {
            Id = _clueId,
            Title = _clueTitle,
            Description = _clueDescription
        };

        Debug.Log($"Leyendo: {clue.Title}. Publicando evento al bus...");
        _bus.Publish(new ClueCollectedEvent(clue));
    }
}