using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    private readonly HashSet<string> items = new HashSet<string>();
    private readonly HashSet<string> clues = new HashSet<string>();
    private readonly Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool HasItem(string itemId)
    {
        return items.Contains(itemId);
    }

    public void RegisterItem(ItemPickupData item)
    {
        items.Add(item.ItemId);
    }

    public bool HasClue(string clueId)
    {
        return clues.Contains(clueId);
    }

    public void RegisterClue(string clueId
        )
    {
        clues.Add(clueId
           );
    }

    public bool HasFlag(string flagId)
    {
        return flags.TryGetValue(flagId, out bool value) && value;
    }

    public void SetFlag(string flagId, bool value)
    {
        flags[flagId] = value;
    }
}