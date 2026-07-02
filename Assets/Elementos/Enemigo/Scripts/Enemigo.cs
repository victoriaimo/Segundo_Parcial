using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemigoMovimiento))]
public class Enemigo : MonoBehaviour
{
    [Header("Configuración general de colliders")]
    [SerializeField] private LayerMask capasJugador;

    [Header("Configuración de captura")]
    [SerializeField] private ColliderAnclado colliderCaptura;
    [SerializeField] private float capturaAlcance = 1f;

    [Header("Configuración de visión")]
    [SerializeField] private ColliderAnclado colliderVision;
    [SerializeField] private float VistaTiempoMax;
    [SerializeField] private float visionAlcance = 1f;
    [SerializeField] private float anguloVision = 90f;
    [SerializeField] private LayerMask capasObstruccion;
    [SerializeField] private Transform ojosEnemigo;


    [SerializeField] private float VistaTiempoActual;

    private EnemigoMovimiento movimiento;

    private void Start()
    {
        movimiento = GetComponent<EnemigoMovimiento>();

        //colliderVision.SubscribirColision(JugadorEnAreaDeVision);
        //colliderVision.SubscribirColisionSalida(JugadorSalioAreaDeVision);
        colliderCaptura.SubscribirColision(JugadorEnAreaDeCaptura);
        //colliderVision.SetTargetLayers(capasJugador);
        colliderCaptura.SetTargetLayers(capasJugador);
    }

    private void JugadorSalioAreaDeVision(Collider personaje)
    {
        movimiento.PerderObjetivo();
    }
    // Asegúrate de tener la referencia al script de movimiento en tu script de visión
    [SerializeField] private EnemigoMovimiento movimientoEnemigo;

    private void JugadorEnAreaDeVision(Collider personaje)
    {
        Transform origen = ojosEnemigo != null ? ojosEnemigo : transform;

        Vector3 posicionPersonaje = personaje.transform.position;
        posicionPersonaje.y += 1f; // Apuntar al pecho

        Vector3 direccionAlPersonaje = (posicionPersonaje - origen.position).normalized;
        float distanciaAlPersonaje = Vector3.Distance(origen.position, posicionPersonaje);

        float anguloActual = Vector3.Angle(origen.forward, direccionAlPersonaje);

        // Chequeo de Ángulo
        if (anguloActual < anguloVision / 2f)
        {
            // Chequeo de Obstrucción (Raycast)
            if (!Physics.Raycast(origen.position, direccionAlPersonaje, distanciaAlPersonaje, capasObstruccion))
            {
                // CASO A: ¡Visión directa confirmada!
                // Le pasamos el transform para que lo persiga en tiempo real
                movimientoEnemigo.EstablecerObjetivo(personaje.transform);
            }
            else
            {
                // CASO B: Está en el cono, pero el Raycast chocó con una pared (Obstáculo)
                // Aquí es donde activamos el tiempo de gracia
                movimientoEnemigo.PerderObjetivo();
            }
        }
        else
        {
            // CASO C: Está dentro del colisionador pero se salió del ángulo de visión (ej. se paró detrás del enemigo)
            movimientoEnemigo.PerderObjetivo();
        }
    }

    private void JugadorEnAreaDeCaptura(Collider personaje)
    {
        Debug.Log("Jugador dentro del área de captura");
    }

    private void OnDisable()
    {
        //colliderVision.DesubscribirColision(JugadorEnAreaDeVision);
        colliderCaptura.DesubscribirColision(JugadorEnAreaDeCaptura);
    }
}
