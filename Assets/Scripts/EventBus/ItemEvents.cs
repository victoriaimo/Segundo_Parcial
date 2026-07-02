using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct ItemPickupData
{
    public string ItemId { get; init; }
    public string DisplayName { get; init; }
}

public class ItemAcquiredEvent : IGameEvent
{
    public ItemPickupData Item { get; }
    public ItemAcquiredEvent(ItemPickupData item) => Item = item;
}
