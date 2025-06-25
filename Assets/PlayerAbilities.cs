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

    private List<Image> activeAbilityIcons = new List<Image>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(GameManager.Instance.abilities.Length);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        InitializeAbilities();
    }

    public void InitializeAbilities()
    {
        activeAbilityIcons.Clear();

        foreach (var ah in playerAbilities)
        {
            if(ah.ability.type != Ability.AbilityType.PASSIVE)
            {
                Image icon = Instantiate(abilityIconPrefab, abilityIconParent).GetComponent<Image>();
                icon.sprite = ah.ability.icon;
                icon.gameObject.GetComponent<AbilityIcon>().SetAbility(ah.ability);
                ah.ability.SetIcon(icon);
                ah.ability.SetKey(ah.hotKey);
                activeAbilityIcons.Add(icon);
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
            if(ah.ability.type == Ability.AbilityType.INSTANT)
            {
                if (Input.GetKeyDown(ah.hotKey))
                {
                    ah.ability.Cast();
                }
            }
            else if(ah.ability.type == Ability.AbilityType.AIMABLE)
            {
                if (Input.GetKey(ah.hotKey) && ah.ability.CanCast())
                {
                    ah.ability.Aim();
                }
                else if (Input.GetKeyUp(ah.hotKey))
                {
                    ah.ability.Cast();
                }
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

    public Ability SwitchAbilities(string playerAbilityName, string newAbilityName)
    {
        int playerAbilityIndex = -1;
        int iconIndex = -1;
        int nonPassiveCount = 0;
        Ability newAbility = null;

        for (int i = 0; i < playerAbilities.Count; i++)
        {
            var ability = playerAbilities[i].ability;
            if (ability.name == playerAbilityName)
            {
                playerAbilityIndex = i;

                if (ability.type != Ability.AbilityType.PASSIVE)
                {
                    iconIndex = nonPassiveCount;
                }
                break;
            }

            if (ability.type != Ability.AbilityType.PASSIVE)
            {
                nonPassiveCount++;
            }
        }

        foreach (var ability in GameManager.Instance.abilities)
        {
            if (ability.name == newAbilityName)
            {
                newAbility = ability;
                break;
            }
        }

        if (playerAbilityIndex != -1 && newAbility != null)
        {
            if (iconIndex != -1) // original was non-passive
            {
                Destroy(activeAbilityIcons[iconIndex].gameObject);

                Image icon = Instantiate(abilityIconPrefab, abilityIconParent).GetComponent<Image>();
                icon.sprite = newAbility.icon;
                icon.gameObject.GetComponent<AbilityIcon>().SetAbility(newAbility);
                newAbility.SetIcon(icon);

                activeAbilityIcons[iconIndex] = icon;
            }

            newAbility.SetKey(playerAbilities[playerAbilityIndex].hotKey);
            newAbility.SetCaster(gameObject);
            playerAbilities[playerAbilityIndex].ability = newAbility;
            newAbility.OnInitialise();
            return newAbility;
        }
        else
        {
            Debug.LogWarning("Ability unable to be switched");
            return null;
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
