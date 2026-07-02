using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderAnclado : MonoBehaviour
{
    private event Action<Collider> OnCollisionEnter;
    private event Action<Collider> OnCollisionExit;

    private BoxCollider boxCollider;

    [SerializeField] private float alcanceEnZ = 1f;
    private float ultimoAlcanceEnZ = 1f;
    [SerializeField] private float alcanceEnX = 1f;
    private float ultimoAlcanceEnX = 1f;

    private float initialScaleZ;
    private float initialPosZ;
    private float initialScaleX;
    private float initialPosX;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        initialScaleZ = transform.localScale.z;
        initialPosZ = transform.localPosition.z;
        initialScaleX = transform.localScale.x;
        initialPosX = transform.localPosition.x;
    }

    void Update()
    {
        if (Mathf.Abs(alcanceEnZ - ultimoAlcanceEnZ) > 0.001f || Mathf.Abs(alcanceEnX - ultimoAlcanceEnX) > 0.001f)
        {
            UpdateColliderDimensions();
            ultimoAlcanceEnZ = alcanceEnZ;
            ultimoAlcanceEnX = alcanceEnX;
        }
    }

    private void UpdateColliderDimensions()
    {
        transform.localScale = new Vector3(alcanceEnX, transform.localScale.y, alcanceEnZ);
        float scaleChangeZ = alcanceEnZ - initialScaleZ;
        float scaleChangeX = alcanceEnX - initialScaleX;
        float newPosZ = initialPosZ + (scaleChangeZ * 0.5f);
        float newPosX = initialPosX + (scaleChangeX * 0.5f);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newPosZ);
    }

    private void OnTriggerStay(Collider other)
    {
        OnCollisionEnter?.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.LogWarning("Jugador salio");
        OnCollisionExit?.Invoke(other);
    }

    public void SubscribirColision(Action<Collider> callback) { OnCollisionEnter += callback; }
    public void SubscribirColisionSalida(Action<Collider> callback) { OnCollisionExit += callback; }
    public void DesubscribirColision(Action<Collider> callback) { OnCollisionEnter -= callback; }
    public void DesubscribirColisionSalida(Action<Collider> callback) { OnCollisionExit -= callback; }
}
