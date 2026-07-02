using UnityEngine;

public class ConoDeVision : MonoBehaviour
{
    [Header("Configuraciones de Visión")]
    [SerializeField] private float anguloVision = 90f;
    [SerializeField] private LayerMask capasObstruccion; // IMPORTANTE: No incluyas la capa del jugador aquí
    [SerializeField] private Transform ojosEnemigo;

    [Header("Referencias")]
    [SerializeField] private EnemigoMovimiento movimientoEnemigo;

    private void OnTriggerStay(Collider other)
    {

        // 2. Calcular dirección y distancia
        Transform origen = ojosEnemigo != null ? ojosEnemigo : transform;
        Vector3 posicionDestino = other.transform.position + Vector3.up * 1f; // Apuntar al pecho
        Vector3 direccionAlJugador = (posicionDestino - origen.position).normalized;
        float distanciaAlJugador = Vector3.Distance(origen.position, posicionDestino);

        // 3. Chequeo de Ángulo
        float anguloActual = Vector3.Angle(origen.forward, direccionAlJugador);
        if (anguloActual < anguloVision / 2f)
        {
            // 4. Chequeo Físico de Obstrucción (Raycast)
            // Agregamos un pequeño margen (+0.1f) para evitar errores de redondeo numérico
            if (!Physics.Raycast(origen.position, direccionAlJugador, distanciaAlJugador + 0.1f, capasObstruccion))
            {
                // CASO ÉXITO: El ángulo es correcto y NO hay paredes obstruyendo.
                movimientoEnemigo.EstablecerObjetivo(other.transform);
                return; // Salimos del método con éxito
            }
        }

        // CASO FALLO: Si el jugador se salió del ángulo O el Raycast chocó contra una pared,
        // le avisamos de inmediato al movimiento que perdimos la visión directa.
        movimientoEnemigo.PerderObjetivo();
    }

    private void OnTriggerExit(Collider other)
    {
        // Si el jugador sale físicamente del rango del colisionador, perdemos el objetivo por completo
        if (other.CompareTag("Player"))
        {
            movimientoEnemigo.PerderObjetivo();
        }
    }
}
