using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AttitudeController : MonoBehaviour
{
    Rigidbody rb;
    
    PidController pidControllerX = new PidController(0.1f, 0.01f, 0.01f);
    PidController pidControllerY = new PidController(0.1f, 0.01f, 0.01f);
    PidController pidControllerZ = new PidController(0.1f, 0.01f, 0.01f);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        rb.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
    }
}
