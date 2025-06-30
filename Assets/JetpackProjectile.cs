using Unity.Netcode;
using UnityEngine;

public class JetpackProjectile : MonoBehaviour
{
    float damage;
    bool hasHit = false;
    public GameObject explosionEffect;
    GameObject spawnedEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetDamage(float dmg)
    {

        damage = dmg;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (hasHit) return;
        hasHit = true;
        Collider[] colliders = new Collider[50];
        spawnedEffect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        //ObjectSpawnManager.Instance.SpawnEffectServerRpc(NetworkManager.Singleton.LocalClientId, transform.position, "SmallExplosion");
        int count = Physics.OverlapSphereNonAlloc(transform.position, 1.5f, colliders);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i] == null) continue;

            UnitData unitData = colliders[i].GetComponent<UnitData>();

            if (unitData != null && unitData.GetTeam() == UnitData.Team.BAD)
            {
                Health health = colliders[i].GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamageServerRpc(damage);
                }
            }
        }
        
        Destroy(gameObject);
    }
}
