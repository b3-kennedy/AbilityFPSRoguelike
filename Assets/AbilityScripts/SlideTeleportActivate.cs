using Unity.Netcode.Components;
using UnityEngine;

public class SlideTeleportActivate : Ability
{

    GameObject marker;
    GameObject caster;
    PlayerAbilities playerAbilities;
    public override void OnInitialise()
    {
        base.OnInitialise();
        caster = GetCaster();
        playerAbilities = caster.GetComponent<PlayerAbilities>();
    }

    public void SetMarker(GameObject obj)
    {
        marker = obj;
    }

    public override void PerformCast()
    {
        Vector3 position = new Vector3(marker.transform.position.x, marker.transform.position.y + caster.transform.localScale.y, marker.transform.position.z);
        caster.GetComponent<NetworkTransform>().Teleport(position, caster.transform.rotation, caster.transform.localScale);
        Destroy(marker);
        Ability ability = playerAbilities.SwitchAbilities("SlideTeleportActivate", "SlideTeleport");
        ability.StartCooldown();
    }
}
