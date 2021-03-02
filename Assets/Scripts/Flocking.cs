using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Flocking : MonoBehaviour
{
    public Vector3 Velocity { get => rb.velocity; }
    public Vector3 Position { get => transform.position; }

    public float initialVelocity = 0f;
    public bool isPid = false;

    [SerializeField] float maxSpeed = 40f;
    [SerializeField] float maxSteerForce = 20f;
    [SerializeField, Range(0f, 5f)] float alignmentFactor = 1f;
    [SerializeField, Range(0f, 5f)] float cohesionFactor = 1f;
    [SerializeField, Range(0f, 5f)] float separationFactor = 1f;
    [SerializeField, Range(0f, 5f)] float wallAvoidanceFactor = 4f;
    
    Rigidbody rb;
    HashSet<Flocking> nearbyAgents = new HashSet<Flocking>();
    HashSet<Collider> walls = new HashSet<Collider>();

    public float p, i, d = 0.1f;
    PidController pidControllerX = new PidController();
    PidController pidControllerY = new PidController();
    PidController pidControllerZ = new PidController();

    Vector3 wallAvoidance = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * initialVelocity;
    }

    void FixedUpdate()
    {
        Debug.Log(Velocity.magnitude);

        if (nearbyAgents.Count < 1) return;

        Vector3 steeringForce = Vector3.zero;

        // alignment - attraction to neighbours' average velocity vector
        Vector3 avgVelocityVector = nearbyAgents.Aggregate(
            Vector3.zero,
            (sum, neighbour) => sum += neighbour.Velocity,
            (sum) => sum / nearbyAgents.Count
        );

        // cohesion - attraction to neighbours' average position
        Vector3 avgPosition = nearbyAgents.Aggregate(
            Vector3.zero,
            (sum, neighbour) => sum += neighbour.Position,
            (sum) => sum / nearbyAgents.Count
        );
        Vector3 cohesionVector = avgPosition - Position;

        // separation - repulsion from each neighbour scaled inversely and averaged
        Vector3 separationVector = nearbyAgents.Aggregate(
            Vector3.zero,
            (sum, neighbour) =>
            {
                Vector3 repulsion = Position - neighbour.Position;
                sum += repulsion * (1 / repulsion.magnitude);
                return sum;
            },
            (sum) => sum / nearbyAgents.Count
        );

        steeringForce += NormaliseAndShit(avgVelocityVector) * alignmentFactor;
        steeringForce += NormaliseAndShit(cohesionVector) * cohesionFactor;
        steeringForce += NormaliseAndShit(separationVector) * separationFactor;

        // Walls
        Vector3 wallAvoidance = walls.Aggregate(
            Vector3.zero,
            (sum, wall) =>
            {
                Vector3 nearestPoint = wall.ClosestPoint(Position);
                Debug.DrawLine(Position, nearestPoint, Color.magenta);
                Vector3 repulsion = Position - nearestPoint;
                sum += repulsion * (1 / repulsion.magnitude);
                return sum;
            },
            (sum) => sum / walls.Count
        );

        steeringForce += NormaliseAndShit(wallAvoidance) * wallAvoidanceFactor;
        // Walls

        Vector3 forceDelta = new Vector3(
            pidControllerX.LiveTuneUpdate(steeringForce.x, Velocity.x, Time.fixedDeltaTime, p, i, d),
            pidControllerY.LiveTuneUpdate(steeringForce.y, Velocity.y, Time.fixedDeltaTime, p, i, d),
            pidControllerZ.LiveTuneUpdate(steeringForce.z, Velocity.z, Time.fixedDeltaTime, p, i, d)
        );

        rb.AddForce(forceDelta, ForceMode.VelocityChange);

        int rayMultiplier = 1;
        Debug.DrawRay(rb.position, forceDelta * rayMultiplier, Color.blue);
        Debug.DrawRay(rb.position, steeringForce * rayMultiplier, Color.red);
        Debug.DrawRay(rb.position, rb.velocity * rayMultiplier, Color.green);
    }

    Vector3 NormaliseAndShit(Vector3 vector)
    {
        Vector3 v = vector.normalized * maxSpeed - Velocity;
        return Vector3.ClampMagnitude(v, maxSteerForce);
    }

    void OnTriggerEnter(Collider other)
    {
        Flocking otherAgent = other.GetComponent<Flocking>();
        if (otherAgent && otherAgent != this)
        {
            nearbyAgents.Add(otherAgent);
            Debug.Log(nearbyAgents.Count);
        }

        if (other is BoxCollider)
        {
            walls.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Flocking otherAgent = other.GetComponent<Flocking>();
        if (otherAgent && otherAgent != this)
        {
            nearbyAgents.Remove(otherAgent);
            Debug.Log(nearbyAgents.Count);
        }

        if (other is BoxCollider)
        {
            walls.Remove(other);
        }
    }
}
