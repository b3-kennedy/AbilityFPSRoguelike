using UnityEngine;

public class GravityBoost : MonoBehaviour
{
    public Transform forwardDirection;
    public float force;
    private void Start()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            PlayerMovement move = other.GetComponent<PlayerMovement>();
            if (move)
            {
                move.OnGravBoost();
            }
            rb.linearVelocity = Vector3.zero;
            Vector3 forceVec = new Vector3(forwardDirection.forward.x, 0.75f, forwardDirection.forward.z);
            rb.AddForce(forceVec * force, ForceMode.Impulse);
            Debug.Log(forwardDirection.forward);
        }
    }
}
