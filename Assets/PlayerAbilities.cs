using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;

[System.Serializable]
public class HotkeyAndAbility
{
    public KeyCode hotKey;
    public Ability ability;
}

public class PlayerAbilities : NetworkBehaviour
{

    public List<HotkeyAndAbility> playerAbilities;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeAbilities();
    }

    public void InitializeAbilities()
    {
        foreach (var ah in playerAbilities)
        {
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
