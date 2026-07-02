using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ClueJournal : IClueRepository
{
    private readonly List<Clue> _clues = new();
    private readonly IGameEventBus _bus;

    public ClueJournal(IGameEventBus bus)
    {
        _bus = bus;
        _bus.Subscribe<ClueCollectedEvent>(OnClueCollected);
    }

    private void OnClueCollected(ClueCollectedEvent e) => Add(e.ClueData);

    public void Add(Clue clue)
    {
        if (_clues.Any(c => c.Id == clue.Id))
            return; // evita duplicar si el jugador interactúa dos veces con la misma nota

        _clues.Add(clue);
        _bus.Publish(new ClueJournalUpdatedEvent(_clues.Count));
    }

    public IReadOnlyList<Clue> GetAll() => _clues;
}
