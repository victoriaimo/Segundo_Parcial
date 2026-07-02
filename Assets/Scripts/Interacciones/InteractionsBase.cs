using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string Prompt { get; }
    void Interact(PlayerMotor player);
}

public interface IBlockable
{
    void Block();
}

