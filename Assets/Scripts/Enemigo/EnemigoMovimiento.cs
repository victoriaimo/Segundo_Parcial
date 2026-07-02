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
    [SerializeField] private float velocidadMerodeo = 2f;

    [Header("Mecánica de Memoria")]
    [SerializeField] private float tiempoDeMemoria = 1.5f;
    private Coroutine corrutinaPerderObjetivo;

    [Header("Mecánica de Búsqueda")]
    [SerializeField] private float radioDeBusqueda = 6f;
    [SerializeField] private int cantidadDePuntosDeBusqueda = 3;
    private int puntosRevisadosActualmente = 0;

    [Header("Mecánica de Merodeo (Acecho Pasivo)")]
    [SerializeField] private Transform jugadorReferencia;
    [SerializeField] private float radioMaximoMerodeo = 20f;
    [SerializeField] private float radioMinimoMerodeo = 5f;
    [SerializeField] private float tiempoEsperaEnPunto = 3f;
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
                if (agente.pathPending) return;

                if (agente.remainingDistance <= agente.stoppingDistance)
                {
                    if (!llegoALaUltimaPosicionConocida)
                    {
                        llegoALaUltimaPosicionConocida = true;
                        puntosRevisadosActualmente = 0;

                        BuscarSiguientePuntoAleatorio();
                    }
                    else
                    {
                        if (puntosRevisadosActualmente < cantidadDePuntosDeBusqueda)
                        {
                            BuscarSiguientePuntoAleatorio();
                        }
                        else
                        {
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

        if (agente.pathPending || agente.remainingDistance > agente.stoppingDistance) return;

        cronometroEspera += Time.deltaTime;
        if (cronometroEspera >= tiempoEsperaEnPunto)
        {
            cronometroEspera = 0f;
            CalcularSiguientePuntoMerodeo();
        }
    }

    private void CalcularSiguientePuntoMerodeo()
    {
        float distanciaAlJugador = Vector3.Distance(transform.position, jugadorReferencia.position);

        float radioObjetivo = Mathf.Clamp(distanciaAlJugador * 0.75f, radioMinimoMerodeo, radioMaximoMerodeo);

        Vector2 circuloAleatorio = Random.insideUnitCircle.normalized * radioObjetivo;
        Vector3 puntoDestino = new Vector3(circuloAleatorio.x, 0, circuloAleatorio.y) + jugadorReferencia.position;

        if (NavMesh.SamplePosition(puntoDestino, out NavMeshHit hit, radioObjetivo, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
        }
    }

    public void EstablecerObjetivo(Transform objetivo)
    {
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
        if (estadoActual == EstadoEnemigo.Persiguiendo && corrutinaPerderObjetivo == null)
        {
            corrutinaPerderObjetivo = StartCoroutine(RutinaTiempoDeGracia());
        }
    }

    private IEnumerator RutinaTiempoDeGracia()
    {
        yield return new WaitForSeconds(tiempoDeMemoria);

        if (jugadorObjetivo != null) ultimaPosicionConocida = jugadorObjetivo.position;

        jugadorObjetivo = null;
        puntosRevisadosActualmente = 0;
        agente.speed = velocidadBusqueda;

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
