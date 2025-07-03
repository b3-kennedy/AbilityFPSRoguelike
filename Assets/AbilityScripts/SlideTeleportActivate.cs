using Unity.Netcode.Components;
using UnityEngine;

public class SlideTeleportActivate : Ability
{

    GameObject marker;
    GameObject player;
    PlayerAbilities playerAbilities;
    public override void OnInitialise()
    {
        base.OnInitialise();
        player = GetCaster();
        playerAbilities = player.GetComponent<PlayerAbilities>();
    }

    public void SetMarker(GameObject obj)
    {
        marker = obj;
    }

    public override void PerformCast()
    {
        Vector3 position = new Vector3(marker.transform.position.x, marker.transform.position.y + player.transform.localScale.y, marker.transform.position.z);
        player.GetComponent<NetworkTransform>().Teleport(position, player.transform.rotation, player.transform.localScale);
        Destroy(marker);
        Ability ability = playerAbilities.SwitchAbilities("SlideTeleportActivate", "SlideTeleport");
        ability.StartCooldown();
    }
}
