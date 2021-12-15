using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Steering : MonoBehaviour
{
    public Vector3 Velocity { get => rb.velocity; }
    public Vector3 Position { get => transform.position; }

    Rigidbody rb;

    [SerializeField] float maxSpeed = 40f;
    [SerializeField] float maxSteerForce = 20f;
    [SerializeField] float detectionRadius = 100f;
    [SerializeField, Range(0f, 5f)] float alignmentFactor = 2f;
    [SerializeField, Range(0f, 5f)] float cohesionFactor = 1.75f;
    [SerializeField, Range(0f, 5f)] float separationFactor = 2.5f;
    [SerializeField, Range(0f, 5f)] float obstacleAvoidanceFactor = 2.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Collider[] nearByColliders = Physics.OverlapSphere(Position, detectionRadius);
        HashSet<Steering> nearbyAgents = new HashSet<Steering>();
        List<Vector3> nearByObstaclePoints = new List<Vector3>();

        foreach (Collider col in nearByColliders)
        {
            Steering s = col.GetComponentInParent<Steering>();
            if (s)
            {
                if (s != this) nearbyAgents.Add(s);
            }
            else
            {
                nearByObstaclePoints.Add(col.ClosestPoint(Position));
            }
        }

        Vector3 desiredSteering = Vector3.zero;
        bool hasNearbyAgents = nearbyAgents.Count > 0;
        bool hasNearbyObstacles = nearByObstaclePoints.Count > 0;

        if (hasNearbyAgents)
        {
            Vector3 flockSteering = CalculateFlockSteering(nearbyAgents);
            Debug.DrawRay(rb.position, flockSteering, Color.blue);
            desiredSteering += flockSteering;
        }

        if (hasNearbyObstacles)
        {
            Vector3 obstacleAvoidanceSum = nearByObstaclePoints.Aggregate(
                Vector3.zero,
                (sum, point) =>
                {
                    Vector3 repulsion = Position - point;
                    Debug.DrawLine(Position, point, Color.magenta);
                    // Debug.DrawRay(Position, (repulsion * (1 / repulsion.magnitude)) * 1.3f, Color.magenta);
                    sum += repulsion * (1 / repulsion.magnitude);
                    return sum;
                }
            );
            Vector3 obstacleSteering = Normalise(obstacleAvoidanceSum / nearByObstaclePoints.Count) * obstacleAvoidanceFactor;
            Debug.DrawRay(rb.position, obstacleSteering, Color.red);
            desiredSteering += obstacleSteering;
        }

        if (!hasNearbyAgents && !hasNearbyObstacles)
        {
            // wander
            desiredSteering = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        }

        Debug.DrawRay(rb.position, desiredSteering, Color.green);
        rb.AddForce(desiredSteering, ForceMode.Acceleration);
    }

    Vector3 CalculateFlockSteering(HashSet<Steering> nearbyAgents) => nearbyAgents.Aggregate(
        new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero },
        (steeringForces, neighbour) =>
        {
            steeringForces[0] += neighbour.Velocity; // alignment
            steeringForces[1] += neighbour.Position; // cohesion
            Vector3 repulsion = Position - neighbour.Position;
            steeringForces[2] += repulsion * (1 / repulsion.magnitude); // separation
            return steeringForces;
        },
        (steeringForces) =>
        {
            Vector3 finalForce = Vector3.zero;
            int totalAgents = nearbyAgents.Count;
            finalForce += Normalise(steeringForces[0] / totalAgents) * alignmentFactor;
            finalForce += Normalise((steeringForces[1] / totalAgents) - Position) * cohesionFactor;
            finalForce += Normalise(steeringForces[2] / totalAgents) * separationFactor;
            return finalForce;
        }
    );

    Vector3 Normalise(Vector3 steering)
    {
        Vector3 steeringForce = steering.normalized * maxSpeed - Velocity;
        Vector3 clamped = Vector3.ClampMagnitude(steeringForce, maxSteerForce);
        return new Vector3(
            clamped.x,
            Mathf.Clamp(clamped.y, -2, 2),
            clamped.z
        );
    }
}
