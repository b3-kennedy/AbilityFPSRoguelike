using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : NetworkBehaviour
{
    private NavMeshPath path;
    TargetHolder holder;
    NavMeshAgent agent;
    Rigidbody rb;
    bool hasAppliedForce;
    float timer;

    void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();
        holder = GetComponent<TargetHolder>();
        agent = GetComponent<NavMeshAgent>();

    }

    void Update()
    {
        

        if (hasAppliedForce)
        {
            timer += Time.deltaTime;
            if(timer >= 0.5f && rb.linearVelocity.magnitude <= 0.2f)
            {
                StopForce();
                timer = 0;
            }
        }

        if (!IsServer) return;

        if (agent.enabled)
        {
            NavMesh.CalculatePath(transform.position, holder.target.position, NavMesh.AllAreas, path);
            agent.SetPath(path);
        }

        //rb.linearVelocity = agent.velocity;
    }

    public void OnApplyForce(Vector3 dir, float force, ForceMode forceMode)
    {
        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(dir * force, forceMode);
        hasAppliedForce = true;
        Debug.Log("apply force");
       
    }

    public void DisableAgent()
    {
        agent.enabled = false;
    }

    public void EnableAgent()
    {
        agent.enabled = true;
    }

    public void StopForce()
    {
        Debug.Log("stop");
        agent.enabled = true;
        rb.isKinematic = true;
        hasAppliedForce = false;
    }
}

