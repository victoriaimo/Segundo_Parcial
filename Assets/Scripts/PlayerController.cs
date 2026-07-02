using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;

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
        Vector3 movimiento = new Vector3(movimientoX, 0, movimientoZ);

        rb.MovePosition(transform.position + movimiento * velocidad * Time.fixedDeltaTime);
    }
}