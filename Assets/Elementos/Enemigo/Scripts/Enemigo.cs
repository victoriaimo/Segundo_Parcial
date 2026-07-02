using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EnemigoMovimiento))]
public class Enemigo : MonoBehaviour
{
    [Header("Configuración de captura")]
    

    [Header("Configuraciones de Visión")]
    [SerializeField] private float anguloVision = 90f;
    [SerializeField] private LayerMask capasObstruccion;
    [SerializeField] private Transform ojosEnemigo;

    [Header("Referencias")]
    [SerializeField] private ColliderAnclado colliderCaptura;
    [SerializeField] private EnemigoMovimiento movimientoEnemigo;
    [SerializeField] private ConoDeVision conoDeVision;

    private void Awake()
    {
        colliderCaptura.SubscribirColision(JugadorEnAreaDeCaptura);
        conoDeVision.Inicializar(anguloVision, capasObstruccion, ojosEnemigo, movimientoEnemigo);
    }

    private void JugadorEnAreaDeCaptura(Collider personaje)
    {
        Debug.Log("Jugador dentro del área de captura");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDisable()
    {
        colliderCaptura.DesubscribirColision(JugadorEnAreaDeCaptura);
    }
}
