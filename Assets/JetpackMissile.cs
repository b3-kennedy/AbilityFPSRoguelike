using UnityEngine;
using UnityEngine.Rendering.Universal;

public class JetpackMissile : TargetedProjectile
{
    Rigidbody rb;
    float speed;
    public float risingSpeed;
    public float fallingSpeed;
    public float rotationSpeed = 180f;
    public enum MissileState {RISING, FALLING};
    public MissileState state;
    public float riseTime = 3f;
    float riseTimer;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //LaunchArcMissile(transform.position, Vector3.zero, 25);
        state = MissileState.RISING;

    }


    private void Update()
    {

        if(state == MissileState.RISING)
        {
            speed = risingSpeed;
            riseTimer += Time.deltaTime;

            // Rotate Y and Z axes toward target while rising
            Vector3 directionToTarget = (target - transform.position).normalized;
            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                Vector3 targetEuler = targetRotation.eulerAngles;
                Vector3 currentEuler = transform.rotation.eulerAngles;

                // Only change Y and Z rotation, keep X the same
                Vector3 constrainedEuler = new Vector3(currentEuler.x, targetEuler.y, targetEuler.z);
                Quaternion constrainedRotation = Quaternion.Euler(constrainedEuler);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, constrainedRotation, rotationSpeed * Time.deltaTime);
            }

            if (riseTimer <= riseTime)
            {
                rb.linearVelocity = transform.forward * speed;
            }
            else
            {
                state = MissileState.FALLING;
                riseTimer = 0;
            }
            
        }
        else
        {
            speed = fallingSpeed;
            Vector3 directionToTarget = (target - transform.position).normalized;
            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                // Calculate the target rotation facing the target
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        
                // Extract only the X-axis rotation (pitch)
                Vector3 targetEuler = targetRotation.eulerAngles;
                Vector3 currentEuler = transform.rotation.eulerAngles;
        
                // Only change the X rotation, keep Y and Z the same
                Vector3 constrainedEuler = new Vector3(targetEuler.x, currentEuler.y, currentEuler.z);
                Quaternion constrainedRotation = Quaternion.Euler(constrainedEuler);
        
                // Rotate towards the constrained target rotation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, constrainedRotation, rotationSpeed * Time.deltaTime);
        
                // Move forward in the current forward direction
                rb.linearVelocity = transform.forward * speed;
            }
        }



        


        

    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
