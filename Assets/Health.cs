using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public float maxHealth = 100f;
    float health;
    public GameObject healthBar;
    TextMeshProUGUI healthValueText;

    public override void OnNetworkSpawn()
    {
        health = maxHealth;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float dmg, int critChance = 0, float critMultiplier = 1f)
    {
        TakeDamageClientRpc(dmg, critChance, critMultiplier);
    }

    [ClientRpc]
    void TakeDamageClientRpc(float dmg, int critChance = 0, float critMultiplier = 1f)
    {
        int randomNum = Random.Range(1, 101);

        float damage = dmg;

        if(randomNum <= critChance)
        {
            damage *= critMultiplier;
            Debug.Log("crit for " + damage);
        }


        health -= damage;
        if (healthBar)
        {
            UpdateHealthBar();
        }

        if(health <= 0)
        {
            Destroy(gameObject);
        }

        UpdateHealthValue();
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(float heal)
    {
        HealClientRpc(heal);
    }

    void UpdateHealthBar()
    {
        float percent = health / maxHealth;
        healthBar.transform.localScale = new Vector3(percent, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    void UpdateHealthValue()
    {
        Transform parent = healthBar.transform.parent;

        if (parent != null && parent.childCount > 1)
        {
            healthValueText = parent.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (healthValueText != null)
            {
                healthValueText.text = health.ToString("F0");
            }
        }
    }

    [ClientRpc]
    void HealClientRpc(float heal)
    {
        health += heal;
        if (healthBar)
        {
            UpdateHealthBar();
        }

        if (health >= maxHealth)
        {
            health = maxHealth;
        }

        UpdateHealthValue();
    }
}
