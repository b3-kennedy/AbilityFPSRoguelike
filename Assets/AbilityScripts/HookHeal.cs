using UnityEngine;

public class HookHeal : Ability
{
    public float radius = 3f;
    Collider[] colliders;
    int enemyCount;
    public float baseHeal = 2f;
    Health playerHealth;

    public override void OnInitialise()
    {
        base.OnInitialise();
        enemyCount = 0;
        playerHealth = GetCaster().GetComponent<Health>();
        colliders = new Collider[20];
    }

    public override void PerformCast()
    {
        int count = Physics.OverlapSphereNonAlloc(GetCaster().transform.position, radius, colliders);
        for (int i = 0; i < count; i++) 
        {
            UnitData data = colliders[i].GetComponent<UnitData>();
            Health health = colliders[i].GetComponent<Health>();
            Debug.Log(colliders[i].gameObject + " data: " + data);
            Debug.Log(colliders[i].gameObject + " health: " + health);
            if (data && health && data.GetTeam() == UnitData.Team.BAD)
            {
                enemyCount++;
            }
        }

        
        float heal = baseHeal * enemyCount;
        playerHealth.HealServerRpc(heal);


    }
}
