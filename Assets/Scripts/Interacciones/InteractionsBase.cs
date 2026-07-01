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

// MOCK TEMPORAL: Esto simula el script PlayerMotor.
// Bórralo cuando se suba el verdadero PlayerMotor.
public class PlayerMotor : MonoBehaviour { }
