using UnityEngine;

public class IncreaseJumps : UpgradeEffect
{
    public override void Apply()
    {
        PlayerMovement movement = GetPlayer().GetComponent<PlayerMovement>();
        movement.IncreaseNumberOfJumps();
    }

    public override void OnSpawned()
    {
        base.OnSpawned();
        SetToolTipDescription($"Increase jump count by {1}");
    }

    public override void Remove()
    {
        PlayerMovement movement = GetPlayer().GetComponent<PlayerMovement>();
        movement.maxNumberOfJumps = 1;
    }
}
