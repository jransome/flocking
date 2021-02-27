using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Steering : MonoBehaviour
{
    public Vector3 Velocity { get => rb.velocity; }

    public bool isSteering = true;

    [SerializeField] float maxVelocity = 50f;
    [SerializeField] Rigidbody rb;
    [SerializeField] HashSet<Steering> nearbyAgents = new HashSet<Steering>();

    PidController pidControllerX = new PidController(0.1f, 0.01f, 0.01f);
    PidController pidControllerY = new PidController(0.1f, 0.01f, 0.01f);
    PidController pidControllerZ = new PidController(0.1f, 0.01f, 0.01f);

    void Start()
    {
        if (!isSteering)
        {
            rb.velocity = transform.forward * 40;
            return;
        }

        rb.velocity = new Vector3(
            Random.Range(-10, 10),
            Random.Range(-10, 10),
            Random.Range(-10, 10)
        );
    }

    void FixedUpdate()
    {
        if (!isSteering || nearbyAgents.Count < 1) return;

        Vector3 steeringForce = Vector3.zero;

        // align
        Vector3 neighbourAverageVelocity = nearbyAgents.Aggregate(
            Vector3.zero,
            (sum, neighbour) => sum += neighbour.Velocity,
            (sum) => sum / nearbyAgents.Count
        );

        steeringForce += neighbourAverageVelocity;

        Vector3 forceDelta = new Vector3(
          Mathf.Clamp(pidControllerX.Update(steeringForce.x, rb.velocity.x, Time.fixedDeltaTime), -maxVelocity, maxVelocity),
          Mathf.Clamp(pidControllerY.Update(steeringForce.y, rb.velocity.y, Time.fixedDeltaTime), -maxVelocity, maxVelocity),
          Mathf.Clamp(pidControllerZ.Update(steeringForce.z, rb.velocity.z, Time.fixedDeltaTime), -maxVelocity, maxVelocity)
        );

        rb.AddForce(forceDelta, ForceMode.VelocityChange);
        Debug.DrawRay(rb.position, forceDelta * 10f, Color.green);
        Debug.DrawRay(rb.position, rb.velocity * 10f, Color.red);
    }

    void OnTriggerEnter(Collider other)
    {
        Steering otherAgent = other.GetComponent<Steering>();
        if (otherAgent && otherAgent != this)
        {
            nearbyAgents.Add(otherAgent);
            Debug.Log(nearbyAgents.Count);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Steering otherAgent = other.GetComponent<Steering>();
        if (otherAgent && otherAgent != this)
        {
            nearbyAgents.Remove(otherAgent);
            Debug.Log(nearbyAgents.Count);
        }
    }
}
