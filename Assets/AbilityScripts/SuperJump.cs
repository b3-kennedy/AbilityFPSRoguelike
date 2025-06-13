using UnityEngine;

public class SuperJump : Ability
{
    Rigidbody rb;
    public float jumpForce;
    PlayerAbilities abilities;
    public override void OnInitialise()
    {
        base.OnInitialise();
        rb = GetCaster().GetComponent<Rigidbody>();
        abilities = GetCaster().GetComponent<PlayerAbilities>();
    }

    public override void PerformCast()
    {
        var altSwitch = abilities.GetAbilityByName("JetpackSwitch");
        if(altSwitch is JetpackAltAbilitySwitch switchScript) 
        {
            if (!switchScript.isAlt)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
            }
        }

    }
}
