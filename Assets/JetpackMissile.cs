using UnityEngine;

public class JetpackMissile : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 10f;
    Vector3 target;
    public float rotationSpeed = 180f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //LaunchArcMissile(transform.position, Vector3.zero, 25);
    }

    public void SetTarget(Vector3 tgt)
    {
        target = tgt;
    }

    private void Update()
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        if (directionToTarget.sqrMagnitude > 0.001f)
        {
            // Calculate the target rotation facing the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Rotate towards the target rotation at rotationSpeed degrees per second
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move forward in the current forward direction
            rb.linearVelocity = transform.forward * speed;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
