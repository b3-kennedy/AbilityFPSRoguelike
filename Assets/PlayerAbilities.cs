using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine.UI;

[System.Serializable]
public class HotkeyAndAbility
{
    public KeyCode hotKey;
    public Ability ability;
}

public class PlayerAbilities : NetworkBehaviour
{

    public List<HotkeyAndAbility> playerAbilities;

    public Transform abilityIconParent;
    public GameObject abilityIconPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        InitializeAbilities();
    }

    public void InitializeAbilities()
    {
        foreach (var ah in playerAbilities)
        {
            if(ah.ability.type == Ability.AbilityType.ACTIVE)
            {
                Image icon = Instantiate(abilityIconPrefab, abilityIconParent).GetComponent<Image>();
                icon.sprite = ah.ability.icon;
                icon.gameObject.GetComponent<AbilityIcon>().SetAbility(ah.ability);
                ah.ability.SetIcon(icon);
                ah.ability.SetKey(ah.hotKey);
            }

            ah.ability.SetCaster(gameObject);
            ah.ability.OnInitialise();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        foreach (var ah in playerAbilities) 
        {
            ah.ability.UpdateAbility();
            if (Input.GetKeyDown(ah.hotKey)) 
            {
                ah.ability.Cast();
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (var ah in playerAbilities)
        {
            ah.ability.FixedUpdateAbility();
        }
    }

    public Ability GetAbilityByName(string abilityName)
    {
        foreach (var ah in playerAbilities) 
        {
            if(ah.ability.abilityName == abilityName)
            {
                return ah.ability;
            }
        }
        return null;
    }
}
