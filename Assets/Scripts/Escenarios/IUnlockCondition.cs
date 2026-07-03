using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnlockCondition
{
    bool IsMet(GameState state);
}
