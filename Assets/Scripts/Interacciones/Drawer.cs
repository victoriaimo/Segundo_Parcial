using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Drawer : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _drawerFront;
    [SerializeField] private float _openDistance = 0.35f;

    private bool _isOpen;
    private Vector3 _closedLocalPos;
    private Vector3 _openLocalPos;

    private void Awake()
    {
        if (_drawerFront == null) _drawerFront = transform;
        _closedLocalPos = _drawerFront.localPosition;
        _openLocalPos = _closedLocalPos + Vector3.forward * _openDistance;
    }

    public string Prompt => _isOpen ? "Cerrar cajón" : "Abrir cajón";

    public void Interact(PlayerController player)
    {
        _isOpen = !_isOpen;
        // El deslizamiento animado (Lerp/DOTween) se resuelve en la etapa de
        // pulido visual; acá dejamos el cambio de estado funcional.
        _drawerFront.localPosition = _isOpen ? _openLocalPos : _closedLocalPos;
    }
}