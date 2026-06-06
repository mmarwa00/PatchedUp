using UnityEngine;

public class lightFlicker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Light horrorLight;

    // Wie schnell das Licht flackert
    public float speed = 0.1f;

    void Start()
    {
        horrorLight = GetComponent<Light>();
    }

    void Update()
    {
        // Ändert die Intensität zufällig in einem kleinen Bereich
        if (Random.value > 0.8f)
        {
            horrorLight.intensity = Random.Range(0.1f, 0.5f);
        }
    }
}
