using UnityEngine;

public class ConoDeVision : MonoBehaviour
{
    private float anguloVision = 90f;
    private LayerMask capasObstruccion;
    private Transform ojosEnemigo;
    private EnemigoMovimiento movimientoEnemigo;

    public void Inicializar(float angulo, LayerMask capasObstruyentes, Transform ojos, EnemigoMovimiento movimiento)
    {
        anguloVision = angulo;
        capasObstruccion = capasObstruyentes;
        ojosEnemigo = ojos;
        movimientoEnemigo = movimiento;
    }

    private void OnTriggerStay(Collider other)
    {
        Transform origen = ojosEnemigo != null ? ojosEnemigo : transform;
        Vector3 posicionDestino = other.transform.position + Vector3.up * 1f;
        Vector3 direccionAlJugador = (posicionDestino - origen.position).normalized;
        float distanciaAlJugador = Vector3.Distance(origen.position, posicionDestino);

        float anguloActual = Vector3.Angle(origen.forward, direccionAlJugador);
        if (anguloActual < anguloVision / 2f)
        {
            if (!Physics.Raycast(origen.position, direccionAlJugador, distanciaAlJugador + 0.1f, capasObstruccion))
            {
                movimientoEnemigo.EstablecerObjetivo(other.transform);
                return;
            }
        }

        movimientoEnemigo.PerderObjetivo();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            movimientoEnemigo.PerderObjetivo();
        }
    }
}
