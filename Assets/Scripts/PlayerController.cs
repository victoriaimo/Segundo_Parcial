using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    public float velocidadCorrer = 8f;

    private Rigidbody rb;

    private float movimientoX;
    private float movimientoZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movimientoX = Input.GetAxis("Horizontal");
        movimientoZ = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        float velocidadActual = velocidad;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidadActual = velocidadCorrer;
        }

        Vector3 movimiento = new Vector3(movimientoX, 0, movimientoZ);

        rb.MovePosition(transform.position + movimiento * velocidadActual * Time.fixedDeltaTime);
    }
}