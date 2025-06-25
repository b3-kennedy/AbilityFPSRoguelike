using UnityEngine;

public class DetonateChargeBall : Ability
{

    PlayerAbilities abilities;
    GameObject spawnedChargeBall;
    public override void OnInitialise()
    {
        base.OnInitialise();
        abilities = GetCaster().GetComponent<PlayerAbilities>();
    }

    public void SetChargeBall(GameObject ball)
    {
        spawnedChargeBall = ball;
    }

    public override void PerformCast()
    {
        spawnedChargeBall.GetComponent<ChargeBallProjectile>().Detonate();
        abilities.SwitchAbilities("DetonateChargeBall", "ChargeBall");
        abilities.GetAbilityByName("ChargeBall").StartCooldown();

    }
}
