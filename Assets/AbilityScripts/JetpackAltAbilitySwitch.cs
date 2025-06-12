using UnityEngine;

public class JetpackAltAbilitySwitch : Ability
{

    public bool isAlt;

    public override void OnInitialise()
    {
        isAlt = false;
    }

    public override void UpdateAbility()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            isAlt = true;
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            isAlt = false;
        }
    }
}
