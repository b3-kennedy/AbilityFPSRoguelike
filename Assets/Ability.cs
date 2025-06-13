using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{

    public enum AbilityType {ACTIVE,PASSIVE};

    public AbilityType type;

    public string abilityName;

    GameObject caster;

    public void SetCaster(GameObject c) 
    {
        caster = c;
    }

    public virtual void OnInitialise()
    {
        if (!string.IsNullOrEmpty(abilityName)) return;
        abilityName = GetType().Name;
    }

    public virtual void UpdateAbility()
    {

    }

    protected GameObject GetCaster() 
    { 
        return caster; 
    }

    public virtual void Cast()
    {
        if (type != AbilityType.ACTIVE) return;
        Debug.Log($"Client {NetworkManager.Singleton.LocalClientId} has casted {abilityName}");
    }
}
