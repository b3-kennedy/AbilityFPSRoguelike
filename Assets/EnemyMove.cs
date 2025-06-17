using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : NetworkBehaviour
{
    public Transform target;
    public float speed = 3f;
    public float stoppingDistance = 0.2f;
    public float waypointTolerance = 0.1f;

    private NavMeshPath path;
    private int currentCorner = 0;
    private Rigidbody rb;

    void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();
        
    }

    void FixedUpdate()
    {
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

        if (path == null || path.corners.Length == 0 || currentCorner >= path.corners.Length)
            return;

        Vector3 direction = (path.corners[currentCorner] - transform.position);
        direction.y = 0; // Optional: ignore height if on flat ground

        if (direction.magnitude < waypointTolerance)
        {
            currentCorner++;
            return;
        }

        Vector3 velocity = direction.normalized * speed;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}

