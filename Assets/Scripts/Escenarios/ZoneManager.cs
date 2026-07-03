using System.Collections.Generic;
using UnityEngine;

public sealed class ZoneManager : MonoBehaviour
{
    [SerializeField]
    private List<ZoneConfig> zones = new List<ZoneConfig>();

    private GameState state;
    private Dictionary<string, IUnlockCondition> conditions;

    private void Start()
    {
        state = GameState.Instance;
        conditions = BuildConditions();

        ReevaluateZones();
    }

    private Dictionary<string, IUnlockCondition> BuildConditions()
    {
        return new Dictionary<string, IUnlockCondition>
        {
            { "sotano", new HasKeyCondition("llave_sotano") },

            { "segundo_piso", new CompositeCondition(
                new IUnlockCondition[]
                {
                    new HasKeyCondition("llave_oficina"),
                    new FlagCondition("generador_encendido")
                },
                true)
            },

            { "azotea", new CompositeCondition(
                new IUnlockCondition[]
                {
                    new HasKeyCondition("pista_mapa"),
                    new HasKeyCondition("pista_escalera")
                },
                false)
            }
        };
    }

    public void ReevaluateZones()
    {
        foreach (ZoneConfig zone in zones)
        {
            if (zone.unlocked)
                continue;

            if (!conditions.ContainsKey(zone.id))
                continue;

            if (conditions[zone.id].IsMet(state))
                UnlockZone(zone);
        }
    }

    private void UnlockZone(ZoneConfig zone)
    {
        zone.unlocked = true;

        if (zone.barrier != null)
            zone.barrier.SetActive(false);

        Debug.Log("Zona desbloqueada: " + zone.id);
    }
}