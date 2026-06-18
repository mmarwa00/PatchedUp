
using StarterAssets;
using UnityEngine;
using UnityEngine.ProBuilder;

public class climbing : MonoBehaviour
{
    public float climbSpeed = 4f;

    [Header("Inventar-Abfrage")]
    [Tooltip("Exakter Name der Fähigkeit im Inspector des Items")]
    public string climbingPinName = "Pin";

    private bool isNearCloth = false;
    private bool isClimbing = false;

    private CharacterController controller;
    private StarterAssetsInputs playerInputs;
    private PersonController personController;

    // Referenz invetory
    private PlayerInventory inventory;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInputs = GetComponent<StarterAssetsInputs>();
        personController = GetComponent<PersonController>();

        // get inventory
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (playerInputs == null) return;

        float verticalInput = playerInputs.move.y;

        // Start climbing if close and W is pressed
        if (isNearCloth && Mathf.Abs(verticalInput) > 0.1f)
        {
            if (!isClimbing)
            {
                // if Pin is there
                if (inventory != null && inventory.HasItem(climbingPinName))
                {
                    isClimbing = true;
                    // Deactivate running script and gravity
                    if (personController != null) personController.enabled = false;
                }
                else
                {
                    // play missing sound maybe
                    Debug.Log("Missing: Pin");


                }
            }
        }

        if (!isNearCloth && isClimbing)
        {
            StopClimbing();
        }
    }

    void FixedUpdate()
    {
        if (isClimbing && controller != null && playerInputs != null)
        {
            float verticalInput = playerInputs.move.y;
            Vector3 climbDirection = new Vector3(0, verticalInput * climbSpeed, 0);
            controller.Move(climbDirection * Time.fixedDeltaTime);

            if (controller.isGrounded && verticalInput < 0)
            {
                StopClimbing();
            }
        }
    }

    private void StopClimbing()
    {
        isClimbing = false;
        // reavtivate normal movement
        if (personController != null) personController.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            Debug.Log("CHECK: berührt Tuch!");
            isNearCloth = true;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isNearCloth = false;
            StopClimbing();
        }
    }
}





/*using UnityEngine;
using StarterAssets;

public class climbing : MonoBehaviour
{
    public float climbSpeed = 4f;
    private bool isNearCloth = false;
    private bool isClimbing = false;

    private CharacterController controller;
    private StarterAssetsInputs playerInputs;

    // Person controller for movment
    private PersonController personController;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInputs = GetComponent<StarterAssetsInputs>();
        personController = GetComponent<PersonController>();
    }

    void Update()
    {
        if (playerInputs == null) return;

        float verticalInput = playerInputs.move.y;

        // Start climbing if close and wW is pressed
        if (isNearCloth && Mathf.Abs(verticalInput) > 0.1f)
        {
            if (!isClimbing)
            {
                isClimbing = true;

                // Deactivate running script and gravity
                if (personController != null) personController.enabled = false;
            }
        }

        if (!isNearCloth && isClimbing)
        {
            StopClimbing();
        }
    }

    void FixedUpdate()
    {
        if (isClimbing && controller != null && playerInputs != null)
        {
            float verticalInput = playerInputs.move.y;

            // move up
            Vector3 climbDirection = new Vector3(0, verticalInput * climbSpeed, 0);
            controller.Move(climbDirection * Time.fixedDeltaTime);

            // if ground stopp movment 
            if (controller.isGrounded && verticalInput < 0)
            {
                StopClimbing();
            }
        }
    }

    private void StopClimbing()
    {
        isClimbing = false;
        // reavtivate normal movement
        if (personController != null) personController.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            Debug.Log("MAMMUT-CHECK: Ich berühre das Tuch!");
            isNearCloth = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isNearCloth = false;
            StopClimbing();
        }
    }
}*/