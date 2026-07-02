using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Door : MonoBehaviour, IInteractable, IBlockable
{
    [SerializeField] private bool _locked;
    private bool _isOpen = false;

    public string Prompt => _locked ? "Cerrada con llave" : (_isOpen ? "Cerrar puerta" : "Abrir puerta");

    public void Interact(PlayerController player)
    {
        if (_locked)
        {
            Debug.Log("Intento de abrir fallido: La puerta está bloqueada.");
            return;
        }

        _isOpen = !_isOpen;
        float angle = _isOpen ? 90f : -90f;
        transform.Rotate(0, angle, 0); // Rota la puerta visualmente
        Debug.Log(_isOpen ? "Puerta abierta." : "Puerta cerrada.");
    }

    public void Block() => _locked = true;
}