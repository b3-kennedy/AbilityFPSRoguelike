using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{

    Ability ability;
    Image cooldownIndicator;
    float smoothedFill;
    float cdTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cooldownIndicator = transform.GetChild(0).GetComponent<Image>();
    }

    public void SetAbility(Ability ab)
    {
        ability = ab;
    }

    public void StartCooldownEffect()
    {
        cdTimer = ability.cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (ability == null) return;

        if (!ability.CanCast())
        {
            cdTimer -= Time.deltaTime;
            float value = Mathf.Clamp01(cdTimer / ability.cooldown);


            cooldownIndicator.fillAmount = value;
        }


    }
}
