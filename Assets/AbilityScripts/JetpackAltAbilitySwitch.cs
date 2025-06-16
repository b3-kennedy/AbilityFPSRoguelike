using UnityEngine;

public class JetpackAltAbilitySwitch : Ability
{

    public bool isAlt;
    public Sprite alternateJump;
    public Sprite alternateImpulse;
    public Sprite baseJumpIcon;
    public Sprite baseImpulseIcon;

    public override void OnInitialise()
    {
        base.OnInitialise();
        isAlt = false;
    }

    public override void PerformCast()
    {
        Ability superJump = GetCaster().GetComponent<PlayerAbilities>().GetAbilityByName("Super Jump");
        Ability impulse = GetCaster().GetComponent<PlayerAbilities>().GetAbilityByName("Vacuum");

        if (isAlt)
        {
            isAlt = false;
            superJump.SetIconSprite(baseJumpIcon);
            impulse.SetIconSprite(baseImpulseIcon);
            
        }
        else
        {
            isAlt = true;
            superJump.SetIconSprite(alternateJump);
            impulse.SetIconSprite(alternateImpulse);
        }
    }

    public override void UpdateAbility()
    {

    }
}
