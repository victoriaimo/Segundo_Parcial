using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string Prompt { get; }
    void Interact(PlayerController player);
}

public interface IBlockable
{
    void Block();
}

