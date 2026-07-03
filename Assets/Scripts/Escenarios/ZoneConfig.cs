using UnityEngine;

[System.Serializable]
public class ZoneConfig
{
    public string id;

    [Tooltip("Objeto que bloquea la zona (puerta, pared, etc.)")]
    public GameObject barrier;

    [HideInInspector]
    public bool unlocked;
}