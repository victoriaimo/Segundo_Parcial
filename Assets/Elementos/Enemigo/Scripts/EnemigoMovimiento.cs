using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoMovimiento : MonoBehaviour
{
    private enum EstadoEnemigo { Inactivo, Persiguiendo, Investigando }
    private EstadoEnemigo estadoActual = EstadoEnemigo.Inactivo;

    private NavMeshAgent agente;
    private Transform jugadorObjetivo;
    private Vector3 ultimaPosicionConocida;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadPersecucion = 5f;
    [SerializeField] private float velocidadBusqueda = 2.5f;
    [SerializeField] private float velocidadMerodeo = 2f; // Movimiento más pausado al patrullar

    [Header("Mecánica de Memoria")]
    [SerializeField] private float tiempoDeMemoria = 1.5f;
    private Coroutine corrutinaPerderObjetivo;

    [Header("Mecánica de Búsqueda")]
    [SerializeField] private float radioDeBusqueda = 6f;
    [SerializeField] private int cantidadDePuntosDeBusqueda = 3;
    private int puntosRevisadosActualmente = 0;

    [Header("Mecánica de Merodeo (Acecho Pasivo)")]
    [SerializeField] private Transform jugadorReferencia; // Referencia general (para saber el área del jugador)
    [SerializeField] private float radioMaximoMerodeo = 20f; // Distancia máxima inicial
    [SerializeField] private float radioMinimoMerodeo = 5f;  // Lo más cerca que llegará merodeando
    [SerializeField] private float tiempoEsperaEnPunto = 3f; // Segundos que se queda quieto antes de elegir otro punto
    private float cronometroEspera = 0f;

    private bool llegoALaUltimaPosicionConocida = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadMerodeo;
    }

    void Update()
    {
        switch (estadoActual)
        {
            case EstadoEnemigo.Inactivo:
                MecanicaMerodeo();
                break;

            case EstadoEnemigo.Persiguiendo:
                if (jugadorObjetivo != null)
                {
                    agente.SetDestination(jugadorObjetivo.position);
                }
                break;

            case EstadoEnemigo.Investigando:
                // Esperamos a que el NavMeshAgent termine de procesar la ruta para evitar falsos positivos
                if (agente.pathPending) return;

                // Verificamos si el agente llegó a su destino actual
                if (agente.remainingDistance <= agente.stoppingDistance)
                {
                    // PASO 1: El enemigo apenas venía viajando a la última posición conocida
                    if (!llegoALaUltimaPosicionConocida)
                    {
                        Debug.Log("[Investigación] Enemigo llegó a la última posición conocida. Iniciando registro del área...");
                        llegoALaUltimaPosicionConocida = true;
                        puntosRevisadosActualmente = 0;

                        // Forzamos el primer punto aleatorio aquí mismo
                        BuscarSiguientePuntoAleatorio();
                    }
                    // PASO 2: Ya estaba buscando puntos aleatorios y llegó a uno de ellos
                    else
                    {
                        if (puntosRevisadosActualmente < cantidadDePuntosDeBusqueda)
                        {
                            BuscarSiguientePuntoAleatorio();
                        }
                        else
                        {
                            Debug.Log("El enemigo perdió el rastro por completo. Volviendo a merodear.");
                            estadoActual = EstadoEnemigo.Inactivo;
                            agente.speed = velocidadMerodeo;
                        }
                    }
                }
                break;
        }
    }

    private void MecanicaMerodeo()
    {
        if (jugadorReferencia == null) return;

        // Si el agente está caminando hacia su punto de merodeo actual, esperamos a que llegue
        if (agente.pathPending || agente.remainingDistance > agente.stoppingDistance) return;

        // Si ya llegó al punto, iniciamos el cronómetro de espera en el lugar
        cronometroEspera += Time.deltaTime;
        if (cronometroEspera >= tiempoEsperaEnPunto)
        {
            cronometroEspera = 0f;
            CalcularSiguientePuntoMerodeo();
        }
    }

    private void CalcularSiguientePuntoMerodeo()
    {
        // 1. Obtener la distancia actual real entre el enemigo y el jugador
        float distanciaAlJugador = Vector3.Distance(transform.position, jugadorReferencia.position);

        // 2. Definir un radio de búsqueda que obligue al enemigo a acercarse un poco más.
        // Reducimos sutilmente la distancia actual para que el nuevo punto tienda a acercarse.
        float radioObjetivo = Mathf.Clamp(distanciaAlJugador * 0.75f, radioMinimoMerodeo, radioMaximoMerodeo);

        // 3. Generar un punto aleatorio en un anillo (ancho) alrededor del jugador
        Vector2 circuloAleatorio = Random.insideUnitCircle.normalized * radioObjetivo;
        Vector3 puntoDestino = new Vector3(circuloAleatorio.x, 0, circuloAleatorio.y) + jugadorReferencia.position;

        // 4. Proyectar sobre la NavMesh para asegurar que sea transitable
        if (NavMesh.SamplePosition(puntoDestino, out NavMeshHit hit, radioObjetivo, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
        }
    }

    // --- Métodos de interacción con el script de visión ---

    public void EstablecerObjetivo(Transform objetivo)
    {
        // Ya estamos persiguiendo a este jugador, no hacer nada.
        if (estadoActual == EstadoEnemigo.Persiguiendo &&
            jugadorObjetivo == objetivo)
            return;

        if (corrutinaPerderObjetivo != null)
        {
            StopCoroutine(corrutinaPerderObjetivo);
            corrutinaPerderObjetivo = null;
        }

        jugadorObjetivo = objetivo;
        jugadorReferencia = objetivo;
        estadoActual = EstadoEnemigo.Persiguiendo;
        agente.speed = velocidadPersecucion;

        if (agente.isStopped)
            agente.isStopped = false;
    }

    public void PerderObjetivo()
    {
        Debug.Log($"[Movimiento] PerderObjetivo invocado. Estado actual: {estadoActual}, Corrutina activa: {corrutinaPerderObjetivo != null}");
        if (estadoActual == EstadoEnemigo.Persiguiendo && corrutinaPerderObjetivo == null)
        {
            corrutinaPerderObjetivo = StartCoroutine(RutinaTiempoDeGracia());
        }
    }

    private System.Collections.IEnumerator RutinaTiempoDeGracia()
    {
        yield return new WaitForSeconds(tiempoDeMemoria);

        if (jugadorObjetivo != null) ultimaPosicionConocida = jugadorObjetivo.position;

        jugadorObjetivo = null;
        puntosRevisadosActualmente = 0;
        agente.speed = velocidadBusqueda;

        // CAMBIO AQUÍ: Inicializamos el estado de investigación correctamente
        llegoALaUltimaPosicionConocida = false;
        estadoActual = EstadoEnemigo.Investigando;

        agente.SetDestination(ultimaPosicionConocida);
        corrutinaPerderObjetivo = null;
    }


    private void BuscarSiguientePuntoAleatorio()
    {
        Vector3 direccionAleatoria = Random.insideUnitSphere * radioDeBusqueda;
        direccionAleatoria += ultimaPosicionConocida;

        if (NavMesh.SamplePosition(direccionAleatoria, out NavMeshHit hit, radioDeBusqueda, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
            puntosRevisadosActualmente++;
        }
    }
}
