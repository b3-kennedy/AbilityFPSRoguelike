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
            if(riseTimer <= riseTime)
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

                // Rotate towards the target rotation at rotationSpeed degrees per second
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

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
