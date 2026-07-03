using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    public float velocidadCorrer = 8f;
    public float fuerzaSalto = 5f;

    public Transform camaraJugador;

    private Rigidbody rb;

    private float movimientoX;
    private float movimientoZ;

    private bool estaEnSuelo = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movimientoX = Input.GetAxis("Horizontal");
        movimientoZ = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && estaEnSuelo)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            estaEnSuelo = false;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(camaraJugador.position, camaraJugador.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 3f))
            {
                Debug.Log("Objeto interactuable detectado: " + hit.collider.name);
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                interactable?.Interact(this);
            }
        }
    }

    void FixedUpdate()
    {
        float velocidadActual = velocidad;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidadActual = velocidadCorrer;
        }

        Vector3 adelante = camaraJugador.forward;
        Vector3 derecha = camaraJugador.right;

        adelante.y = 0;
        derecha.y = 0;

        adelante.Normalize();
        derecha.Normalize();

        Vector3 movimiento = adelante * movimientoZ + derecha * movimientoX;

        rb.MovePosition(transform.position + movimiento * velocidadActual * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            estaEnSuelo = true;
        }
    }
}