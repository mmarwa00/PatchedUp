/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DoorScript
{
	[RequireComponent(typeof(AudioSource))]


public class Door : MonoBehaviour {
	public bool open;
	public float smooth = 1.0f;
	float DoorOpenAngle = -90.0f;
    float DoorCloseAngle = 0.0f;
	public AudioSource asource;
	public AudioClip openDoor,closeDoor;
	// Use this for initialization
	void Start () {
		asource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (open)
		{
            var target = Quaternion.Euler (0, DoorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
	
		}
		else
		{
            var target1= Quaternion.Euler (0, DoorCloseAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
	
		}  
	}

	public void OpenDoor(){
		open =!open;
		asource.clip = open?openDoor:closeDoor;
		asource.Play ();
	}
}
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        [Header("Tür-Zustand")]
        [Tooltip("Ist der Gegner im Raum?")]
        public bool enemyInside;

        [Header("Winkel-Einstellungen")]
        [SerializeField] private float doorSpaltAngle = -15.0f; // Der Standard-Spalt (leicht offen)
        [SerializeField] private float doorFullOpenAngle = -90.0f; // Ganz offen, wenn der Gegner kommt
        [SerializeField] private float doorClosedAngle = 0.0f; // Komplett geschlossen (verriegelt)

        [Header("Puzzle-Sperre")]
        [SerializeField] private bool lockedUntilPuzzlesComplete = true;

        [Header("Geschwindigkeit & Sound")]
        public float smooth = 1.0f;
        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        private bool isUnlocked;
        private bool subscribedToPuzzleManager;

        void Start()
        {
            asource = GetComponent<AudioSource>();
            
            if (!lockedUntilPuzzlesComplete)
            {
                isUnlocked = true;
            }
        }

        void OnDestroy()
        {
            if (subscribedToPuzzleManager && PuzzleManager.Instance != null)
            {
                PuzzleManager.Instance.OnAllPuzzlesCompleted -= Unlock;
            }
        }

        void Update()
        {
            if (!isUnlocked)
            {
                LinkToPuzzleManager();

                Quaternion closedRotation = Quaternion.Euler(0, doorClosedAngle, 0);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, closedRotation, Time.deltaTime * 5 * smooth);
                return;
            }

            // Entriegelt: Originalverhalten – ganz offen für den Gegner, sonst nur ein Spalt.
            float targetAngle = enemyInside ? doorFullOpenAngle : doorSpaltAngle;

            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5 * smooth);
        }

        private void LinkToPuzzleManager()
        {
            PuzzleManager pm = PuzzleManager.Instance;
            if (pm == null) return;

            if (!subscribedToPuzzleManager)
            {
                pm.OnAllPuzzlesCompleted += Unlock;
                subscribedToPuzzleManager = true;
            }
            
            if (pm.AllPuzzlesCompleted)
            {
                Unlock();
            }
        }
        
        public void Unlock()
        {
            if (isUnlocked) return;
            isUnlocked = true;

            if (asource != null && openDoor != null)
            {
                asource.clip = openDoor;
                asource.Play();
            }

            Debug.Log("[Door] Puzzle-Bedingung erfüllt, Tür entriegelt.");
        }

        // Diese Funktion kannst du von deinem Trigger oder Gegner-Skript aus aufrufen
        public void SetEnemyInside(bool isInside)
        {
            // Verhindert, dass der Sound doppelt triggert, wenn sich nichts ändert
            if (enemyInside == isInside) return;

            enemyInside = isInside;

            // Sound abspielen
            if (asource != null)
            {
                asource.clip = enemyInside ? openDoor : closeDoor;
                asource.Play();
            }
        }
    }
}
