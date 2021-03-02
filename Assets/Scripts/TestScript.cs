using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public float minSpeed = 0.01f;
    public float maxSpeed = 0.2f;
    public List<Transform> Waypoints = new List<Transform>();

    Rigidbody rb;
    int targetIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // if (Vector3.Distance(transform.position, Waypoints[targetIndex].position) < 0.5f)
        // {
        //     targetIndex = (targetIndex + 1) % Waypoints.Count;
        // }
        // transform.position = Vector3.MoveTowards(transform.position, Waypoints[targetIndex].position, Random.Range(minSpeed, maxSpeed));
        if (Input.GetKeyDown(KeyCode.Y))
        {
            rb.AddForce(Vector3.up * 3, ForceMode.Acceleration);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            rb.AddForce(Vector3.up * -1, ForceMode.Acceleration);
        }
    }
}
