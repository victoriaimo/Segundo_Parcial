using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ClueJournalView : MonoBehaviour
{
    [SerializeField] private Transform _listContainer;
    [SerializeField] private GameObject _entryPrefab; // prefab simple con un Text/TMP_Text

    private IClueRepository _repository;
    private IGameEventBus _bus;

    public void Construct(IClueRepository repository, IGameEventBus bus)
    {
        _repository = repository;
        _bus = bus;
        _bus.Subscribe<ClueJournalUpdatedEvent>(OnJournalUpdated);

        Refresh();
    }

    private void OnDestroy() => _bus?.Unsubscribe<ClueJournalUpdatedEvent>(OnJournalUpdated);

    private void OnJournalUpdated(ClueJournalUpdatedEvent e) => Refresh();

    private void Refresh()
    {
        if (_listContainer == null) return;

        foreach (Transform child in _listContainer)
            Destroy(child.gameObject);

        foreach (var clue in _repository.GetAll())
        {
            var entry = Instantiate(_entryPrefab, _listContainer);
            var label = entry.GetComponentInChildren<UnityEngine.UI.Text>();
            if (label != null) label.text = clue.Title;
        }
    }
}