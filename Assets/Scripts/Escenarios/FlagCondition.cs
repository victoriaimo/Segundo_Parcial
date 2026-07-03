using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FlagCondition : IUnlockCondition
{
    private readonly string _flagId;

    public FlagCondition(string flagId)
    {
        _flagId = flagId;
    }

    public bool IsMet(GameState state)
    {
        return state.HasFlag(_flagId);
    }
}