
using StarterAssets;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.InputSystem;

public class ClimbingController : MonoBehaviour
{
    public float climbSpeed = 4f;

    [Header("Übergang oben")]
    [Tooltip("Spieler nach oben versetzt beim Ausstieg")]
    public float exitOffsetUp = 0.5f;
    [Tooltip("Spieler in Blickrichtung nach vorne geschubst beim Ausstieg")]
    public float exitOffsetForward = 0.6f;

    [Header("Inventar-Abfrage")]
    public string climbingPinName = "Pin";

    private bool isNearCloth = false;
    private bool isClimbing = false;

    private CharacterController controller;
    private StarterAssetsInputs playerInputs;
    private PersonController personController;
    private PlayerInventory inventory;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInputs = GetComponent<StarterAssetsInputs>();
        personController = GetComponent<PersonController>();
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (playerInputs == null) return;
        if (Keyboard.current == null) return;

        // start with F-Taste
        if (isNearCloth && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (!isClimbing)
            {
                if (inventory != null && inventory.HasItem(climbingPinName))
                {
                    StartClimbing();
                }
                else
                {
                    Debug.Log("Missing: Pin");
                }
            }
            else
            {
                StopClimbing(false); // manuell no push
            }
        }
    }

    void FixedUpdate()
    {
        if (isClimbing && controller != null && playerInputs != null)
        {
            float verticalInput = playerInputs.move.y;
            Vector3 climbDirection = new Vector3(0, verticalInput * climbSpeed, 0);
            controller.Move(climbDirection * Time.fixedDeltaTime);

            // touch floor automatic start of movement
            if (controller.isGrounded && verticalInput < 0)
            {
                StopClimbing(false);
            }
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        Debug.Log("Kletter AKTIV - Lauf-Skript pausiert");
        if (personController != null) personController.enabled = false;
    }

    // Erweitert um einen Parameter, der entscheidet, ob wir den Schubs nach vorne ausführen
    private void StopClimbing(bool snapToPlatform)
    {
        if (isClimbing)
        {
            isClimbing = false;
            Debug.Log("Klettern beendet - Normales Lauf-Skript wieder.");

            if (snapToPlatform && controller != null)
            {
                // CharacterController inaktive
                controller.enabled = false;

                // streight forward calculation 
                Vector3 forwardFlat = transform.forward;
                forwardFlat.y = 0;
                forwardFlat.Normalize();

                // Teleportation 
                transform.position += Vector3.up * exitOffsetUp + forwardFlat * exitOffsetForward;

                controller.enabled = true;
            }

            if (personController != null) personController.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            Debug.Log("CHECK: berührt Tuch! Drücke einmal F zum Klettern.");
            isNearCloth = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isNearCloth = false;

            // if climibing up and leave , push forward
            if (isClimbing && playerInputs.move.y > 0.1f)
            {
                StopClimbing(true); // true!!!!!!
            }
        }
    }
}






