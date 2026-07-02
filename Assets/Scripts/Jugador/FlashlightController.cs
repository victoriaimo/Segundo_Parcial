using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    private Light linterna;

    void Start()
    {
        linterna = GetComponent<Light>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            linterna.enabled = !linterna.enabled;
        }
    }
}