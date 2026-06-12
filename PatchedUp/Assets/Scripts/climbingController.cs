using UnityEngine;

public class climbing : MonoBehaviour
{
    public float climbSpeed = 5f;
    private bool isNearCloth = false;
    private bool isClimbing = false;

    private Rigidbody rb;

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // start climbing
        if (isNearCloth && Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            isClimbing = true;
        }
        if (!isNearCloth)
        {
            isClimbing = false;
        }
    }

    void FixedUpdate()
    {
        if (isClimbing)
        {
            // no gravity
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero; //Stoppt das rutschen...

            
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 climbDirection = new Vector3(0, verticalInput * climbSpeed, 0);
            rb.MovePosition(transform.position + climbDirection * Time.fixedDeltaTime);
        }
        else
        {
            rb.useGravity = true;
        }
    }

    //Trigger-Cube!
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isNearCloth = true;
        }
    }

    //
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isNearCloth = false;
            isClimbing = false;
        }
    }
}
