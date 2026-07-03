using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class HasKeyCondition : IUnlockCondition
{
    private readonly string _keyId;

    public HasKeyCondition(string keyId)
    {
        _keyId = keyId;
    }

    public bool IsMet(GameState state)
    {
        return state.HasItem(_keyId);
    }
}