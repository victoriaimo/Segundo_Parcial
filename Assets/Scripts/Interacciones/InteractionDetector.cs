using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class InteractionDetector : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float _range = 2.5f;
    [SerializeField] private Camera _playerCamera;

    private IInteractable _current;
    private PlayerMotor _playerMotor;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        FindClosestInteractable();

        // Si estamos mirando un objeto interactuable y presionamos la tecla E
        if (_current != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[InteractionDetector] Ejecutando: {_current.Prompt}");
            _current.Interact(_playerMotor);
        }
    }

    private void FindClosestInteractable()
    {
        // Lanzamos el rayo desde el centro de la pantalla
        Ray ray = _playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, _range))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                _current = interactable;
                return;
            }
        }

        _current = null;
    }
}
