using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public float maxHealth = 100f;
    float health;
    public GameObject healthBar;

    public override void OnNetworkSpawn()
    {
        health = maxHealth;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float dmg)
    {
        TakeDamageClientRpc(dmg);
    }

    [ClientRpc]
    void TakeDamageClientRpc(float dmg)
    {
        health -= dmg;
        float percent = health / maxHealth;
        healthBar.transform.localScale = new Vector3(percent, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
