using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{

    public enum AbilityType {ACTIVE,PASSIVE};

    public AbilityType type;

    public string abilityName;

    public Sprite icon;

    Image abilityUIIcon;

    Image cooldownIndicator;

    public float cooldown;

    float lastCastTime = Mathf.NegativeInfinity;

    GameObject caster;

    KeyCode key;

    public void SetCaster(GameObject c) 
    {
        caster = c;
    }

    public void SetKey(KeyCode code)
    {
        key = code;
    }

    public KeyCode GetKey()
    {
        return key;
    }

    public GameObject GetCamera()
    {
        if (caster)
        {
            return caster.transform.Find("CameraHolder/Recoil/Camera").gameObject;
        }
        Debug.Log("Cannot get camera, caster is null");
        return null;
        
    }

    public Gun GetGun()
    {
        if (caster)
        {
            return caster.transform.Find("CameraHolder/Recoil/Camera/GunPosition/GunParent/Gun").GetComponent<Gun>();
        }
        Debug.Log("Cannot get gun, caster is null");
        return null;
    }

    public float GetLastCastTime()
    {
        return lastCastTime;
    }

    public virtual void OnInitialise()
    {
        lastCastTime = Mathf.NegativeInfinity;
        if (!string.IsNullOrEmpty(abilityName)) return;
        abilityName = GetType().Name;
        
    }

    public void SetIcon(Image icon)
    {
        abilityUIIcon = icon;
    }
    

    public void SetIconSprite(Sprite iconSprite)
    {
        abilityUIIcon.sprite = iconSprite;
    }

    public virtual void UpdateAbility()
    {

    }

    public virtual void FixedUpdateAbility()
    {

    }

    protected GameObject GetCaster() 
    { 
        return caster; 
    }

    public bool CanCast()
    {
        return Time.time >= lastCastTime + cooldown;
    }

    public float GetCooldownPercent()
    {
        if (cooldown <= 0f) return 0f;

        float remaining = Mathf.Clamp(lastCastTime + cooldown - Time.time, 0f, cooldown);
        float percent = remaining / cooldown;
        return Mathf.Round(percent * 1000f) / 1000f;
    }

    public float GetCooldownTimeRemaining()
    {
        return Mathf.Clamp(lastCastTime + cooldown - Time.time, 0f, cooldown);
    }

    public virtual void Cast()
    {

        if (type != AbilityType.ACTIVE) return;

        if (!CanCast()) return;

        PerformCast();
        abilityUIIcon.GetComponent<AbilityIcon>().StartCooldownEffect();

        lastCastTime = Time.time;
    }

    public virtual void PerformCast()
    {

    }
}
