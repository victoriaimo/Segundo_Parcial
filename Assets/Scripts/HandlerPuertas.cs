using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerPuertas : MonoBehaviour, IEventBusDependent
{
    [SerializeField] private PickableKey keyPrefab;
    [SerializeField] private List<Door> doors;
    [SerializeField] private List<Transform> keySpawnPoint;
    [SerializeField] private IGameEventBus bus;

    public void Construct(IGameEventBus bus)
    {
        this.bus = bus;
    }

    private void Start()
    {
        foreach (var door in doors)
        {
            string keyId = door.KeyID;
            string displayName = "Algun Nombre";

            Transform spawnPoint = BuscarSpawnPointVacio();
            
            if(spawnPoint != null)
            {
                PickableKey keyInstance = Instantiate(keyPrefab);
                keyInstance.ConstructWithParam(bus, keyId, displayName);

                keyInstance.transform.position = spawnPoint.position;
                keyInstance.transform.SetParent(spawnPoint);

                Debug.Log("Llave: " + keyId + " en spawn point: " + spawnPoint.name);
            }
            else
                Debug.LogWarning("No hay spawn points vacíos disponibles para la llave: " + keyId);
        }
    }

    private Transform BuscarSpawnPointVacio()
    {
        List<Transform> freeSpawns = new List<Transform>();
        foreach (Transform point in keySpawnPoint)
        {
            if (point.childCount == 0)
                freeSpawns.Add(point);
        }

        if (freeSpawns.Count > 0)
        {
            int randomIndex = Random.Range(0, freeSpawns.Count);
            return freeSpawns[randomIndex];
        }
        return null;
    }
}
