using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderAnclado : MonoBehaviour
{
    private event Action<Collider> onCollisionEnter;
    private event Action<Collider> onCollisionExit;

    private List<LayerMask> targetLayers = null;

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

    public void SetTargetLayers(LayerMask combinedLayers)
    {
        boxCollider.excludeLayers = ~combinedLayers;
        boxCollider.includeLayers = combinedLayers;
        boxCollider.layerOverridePriority = 1;
    }

    private void OnTriggerStay(Collider other)
    {
        onCollisionEnter?.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.LogWarning("Jugador salio");
        onCollisionExit?.Invoke(other);
    }

    public void SubscribirColision(Action<Collider> callback)
    {
        onCollisionEnter += callback;
    }
    public void SubscribirColisionSalida(Action<Collider> callback)
    {
        onCollisionExit += callback;
    }
    public void DesubscribirColision(Action<Collider> callback)
    {
        onCollisionEnter -= callback;
    }
    public void DesubscribirColisionSalida(Action<Collider> callback)
    {
        onCollisionExit -= callback;
    }
}
