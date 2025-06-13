using UnityEngine;

public class JetpackAltAbilitySwitch : Ability
{

    public bool isAlt;

    public override void OnInitialise()
    {
        isAlt = false;
    }

    public override void PerformCast()
    {
        if (isAlt)
        {
            isAlt = false;
        }
        else
        {
            isAlt = true;
        }
    }

    public override void UpdateAbility()
    {

    }
}
