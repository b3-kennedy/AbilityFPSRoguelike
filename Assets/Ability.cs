using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{

    public enum AbilityType {ACTIVE,PASSIVE};

    public AbilityType type;

    public string abilityName;

    public float cooldown;

    float lastCastTime = Mathf.NegativeInfinity;

    GameObject caster;

    public void SetCaster(GameObject c) 
    {
        caster = c;
    }

    public virtual void OnInitialise()
    {
        lastCastTime = Mathf.NegativeInfinity;
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

    bool CanCast()
    {
        Debug.Log($"Time: {Time.time}, Last: {lastCastTime}, Cooldown: {cooldown}, CanCast: {Time.time >= lastCastTime + cooldown}");
        return Time.time >= lastCastTime + cooldown;
    }

    public virtual void Cast()
    {


        if (type != AbilityType.ACTIVE) return;

        if (!CanCast()) return;

        PerformCast();

        lastCastTime = Time.time;
    }

    public virtual void PerformCast()
    {

    }
}
