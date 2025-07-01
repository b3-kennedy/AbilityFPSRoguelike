using UnityEngine;

public class IncreaseJumps : UpgradeEffect
{
    public override void Apply()
    {
        PlayerMovement movement = GetPlayer().GetComponent<PlayerMovement>();
        movement.IncreaseNumberOfJumps();
    }

    public override void Remove()
    {
        PlayerMovement movement = GetPlayer().GetComponent<PlayerMovement>();
        movement.maxNumberOfJumps = 1;
    }
}
