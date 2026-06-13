using System.Collections.Generic;
using UnityEngine;

public class lightFlicker : MonoBehaviour
{
    private Light _light;

    [Header("Helligkeit")]
    [Tooltip("Minimale Helligkeit beim Flackern")]
    [SerializeField] private float minIntensity = 0.2f;
    [Tooltip("Maximale Helligkeit beim Flackern")]
    [SerializeField] private float maxIntensity = 1.5f;

    [Header("Geschwindigkeit (Die Bremse)")]
    [Tooltip("Kürzeste Pause zwischen zwei Flacker-Momenten (in Sekunden)")]
    [SerializeField] private float minDelay = 0.05f;
    [Tooltip("Längste Pause zwischen zwei Flacker-Momenten (in Sekunden)")]
    [SerializeField] private float maxDelay = 0.4f;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void OnEnable()
    {
        // Startet die Flacker-Schleife
        StartCoroutine(FlickerRoutine());
    }

    private System.Collections.IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // 1. Würfle eine zufällige Helligkeit aus
            _light.intensity = Random.Range(minIntensity, maxIntensity);

            // 2. Würfle aus, wie lange diese Helligkeit halten soll
            float randomDelay = Random.Range(minDelay, maxDelay);

            // 3. Warte exakt diese Zeit ab, bevor die Schleife von vorne beginnt
            yield return new WaitForSeconds(randomDelay);
        }
    }
}
