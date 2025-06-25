using Unity.Netcode;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float force;
    Vector3 direction;
    public string ID;
    
    public void SetValues(float f, Vector3 dir, string id)
    {
        force = f;
        direction = dir;
        ID = id;

    }

    public float GetForce()
    {
        return force;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }

}
