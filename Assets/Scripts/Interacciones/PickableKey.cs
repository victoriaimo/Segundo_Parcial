using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PickableKey : MonoBehaviour, IInteractable, IEventBusDependent
{
    [SerializeField] private string _itemId = "key_basement";
    [SerializeField] private string _displayName = "Llave del sótano";

    private IGameEventBus _bus;

    public void Construct(IGameEventBus bus) => _bus = bus;
    public void ConstructWithParam(IGameEventBus bus, string keyId, string displayName)
    {
        _bus = bus;
        _itemId = keyId;
        _displayName = displayName;
    }
    public string Prompt => $"Tomar {_displayName}";

    public void Interact(PlayerController player)
    {
        if (_bus == null)
        {
            Debug.LogError("PickableKey: el EventBus no fue inyectado.");
            return;
        }

        Debug.Log($"Recogida: {_displayName}. Publicando ItemAcquiredEvent...");

        _bus.Publish(new ItemAcquiredEvent(new ItemPickupData
        {
            ItemId = _itemId,
            DisplayName = _displayName
        }));

        gameObject.SetActive(false); // desaparece del mundo al recogerla
    }
}
