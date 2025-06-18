using Unity.Netcode;
using UnityEngine;

public class ExplosiveThrowingKnifeProjectile : NetworkBehaviour
{
    public float rotationSpeed;
    Rigidbody rb;
    [HideInInspector] public bool spin = true;
    Collider col;
    public LayerMask layerMask;
    [HideInInspector] public ulong clientId;
    [HideInInspector] public GameObject player;
    public bool isLive;
    Transform hitTransform;

    public enum KnifeType { LOCAL, SERVER };
    public KnifeType knifeType;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spin)
        {
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(-transform.forward, rb.linearVelocity.normalized);
                transform.rotation = lookRotation;
            }

            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
        }

        Vector3 direction = rb.linearVelocity.normalized;
        float distance = rb.linearVelocity.magnitude * Time.fixedDeltaTime;

        Vector3 center = transform.position;
        float height = 0.2f;
        float radius = 0.05f;
        Vector3 point0 = center + transform.up * (height / 2f);
        Vector3 point1 = center - transform.up * (height / 2f);

        if (knifeType != KnifeType.LOCAL) return;

        if (Physics.CapsuleCast(point0, point1, radius, direction, out RaycastHit hit, distance, layerMask))
        {
            hitTransform = hit.transform;
            transform.position = hit.point;
            spin = false;
            rb.isKinematic = true;

            ulong parentID = hitTransform.GetComponent<NetworkObject>().NetworkObjectId;
            Debug.Log(hitTransform);
            player.GetComponent<ServerKnifeHolder>().SpawnKnifeServerRpc(clientId, "ThrowingKnife", transform.position, transform.rotation, parentID);
            
            Destroy(gameObject);

        }


    }



    [ClientRpc]
    void DestroyLocalKnifeClientRpc()
    {
        Destroy(gameObject);
    }

}
