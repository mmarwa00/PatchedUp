using UnityEngine;
using DoorScript;

public class DoorTriggerZone : MonoBehaviour
{
    public Door theDoor; // Ziehe hier im Inspector deine Tür rein

    private void OnTriggerEnter(Collider other)
    {
        // Prüfe, ob das Objekt, das den Raum betritt, der Gegner ist
        if (other.CompareTag("Enemy"))
        {
            theDoor.SetEnemyInside(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Wenn der Gegner den Bereich wieder verlässt
        if (other.CompareTag("Enemy"))
        {
            theDoor.SetEnemyInside(false);
        }
    }
}