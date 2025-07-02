using UnityEngine;
using UnityEngine.Rendering;
using UnityEngineInternal;

public class FlyingEnemyMove : MonoBehaviour
{
    Rigidbody rb;
    Vector3 directionToTarget;
    public float speed;
    public float checkTargetTime;
    float timer;
    public float attackRange;
    TargetHolder holder;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        holder = GetComponent<TargetHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!holder.target) return;

        if (Vector3.Distance(transform.position, holder.target.position) <= attackRange)
        {
            rb.linearVelocity = Vector3.zero;
        }

        timer += Time.deltaTime;
        if(timer >= checkTargetTime)
        {
            if (CanSeeTarget())
            {
                if(Vector3.Distance(transform.position, holder.target.position) > attackRange)
                {
                    rb.linearVelocity = directionToTarget * speed;
                }
                              
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
                Vector3? teleportPos = FindTeleportPosition();
                if (teleportPos.HasValue)
                {
                    transform.position = teleportPos.Value;
                }
            }
            timer = 0;
        }

    }

    bool CanSeeTarget()
    {
        directionToTarget = (holder.target.position - transform.position).normalized;
        float dist = Vector3.Distance(holder.target.position, transform.position);
        if(Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit,dist))
        {
            if(hit.transform == holder.target)
            {
                return true;
            }
        }
        return false;
    }

    Vector3? FindTeleportPosition()
    {
        float radius = 20f; // How far from the target to search
        int checks = 10; // Number of positions to try
        float heightOffset = 10f; // How high off the ground the enemy can teleport

        for (int i = 0; i < checks; i++)
        {
            // Random direction around target
            Vector3 randomDirection = Random.onUnitSphere;
            randomDirection.y = Mathf.Clamp(randomDirection.y, 0.2f, 1f); // Keep above ground-ish
            Vector3 candidatePos = holder.target.position + randomDirection * radius;
            Debug.Log(candidatePos);
            // Check visibility from candidatePos to target
            Vector3 dirToTarget = (holder.target.position - candidatePos).normalized;
            float distance = Vector3.Distance(candidatePos, holder.target.position);

            if (Physics.Raycast(candidatePos, dirToTarget, out RaycastHit hit ,distance))
            {
                if (hit.transform == holder.target)
                {
                    return candidatePos;
                }
            }
        }

        return null; // No valid position found
    }
}
