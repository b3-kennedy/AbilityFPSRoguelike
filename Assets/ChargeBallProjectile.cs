using Unity.Netcode;
using UnityEngine;

public class ChargeBallProjectile : MonoBehaviour
{
    float storedDamage;
    GameObject player;
    float radius;

    private void Start()
    {
        storedDamage = 0;
    }

    public void AddDamage(float dmg)
    {
        storedDamage += dmg;
        Debug.Log(storedDamage);
    }

    public void SetPlayer(GameObject plyer)
    {
        player = plyer;
    }

    public void SetRadius(float rad)
    {
        radius = rad;
    }

    public void Detonate()
    {
        DealDamage();
        Destroy(gameObject);
        ProjectileManager.Instance.DestroyLocalProjectileFromServerRpc(NetworkManager.Singleton .LocalClientId,GetComponent<Projectile>().ID);
    }

    void DealDamage()
    {
        Collider[] colliders = new Collider[50];
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i] == null) continue;

            UnitData unitData = colliders[i].GetComponent<UnitData>();

            if (unitData != null && unitData.GetTeam() == UnitData.Team.BAD)
            {
                Health health = colliders[i].GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamageServerRpc(storedDamage);
                }
            }
        }
    }



    private void OnCollisionEnter(Collision other)
    {
        player.GetComponent<PlayerAbilities>().SwitchAbilities("DetonateChargeBall", "ChargeBall");
        player.GetComponent<PlayerAbilities>().GetAbilityByName("ChargeBall").StartCooldown();
        DealDamage();
        Destroy(gameObject);
        
    }
}
