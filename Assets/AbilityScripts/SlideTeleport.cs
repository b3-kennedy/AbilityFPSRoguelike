using UnityEngine;

public class SlideTeleport : Ability
{
    public GameObject teleportMarker;
    public GameObject teleportMarkerIndicator;
    GameObject spawnedTeleportMarker;
    Vector3 hitPoint;
    GameObject cam;
    GameObject spawnedIndicator;
    public LayerMask layerMask;
    PlayerAbilities playerAbilities;
    public float castRange = 10f;

    public override void OnInitialise()
    {
        base.OnInitialise();
        cam = GetCamera();
        playerAbilities = GetCaster().GetComponent<PlayerAbilities>();
    }

    public override void Aim()
    {
        hitPoint = cam.transform.position + cam.transform.forward * 1000;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, castRange, layerMask))
        {
            if (hit.collider)
            {
                hitPoint = hit.point;
                Debug.Log(hit.collider);

                if (!spawnedIndicator)
                {
                    spawnedIndicator = Instantiate(teleportMarkerIndicator, hit.point, Quaternion.identity);
                }
                else
                {
                    spawnedIndicator.transform.position = hit.point;
                }

            }
        }
        else
        {
            if (spawnedIndicator)
            {
                Destroy(spawnedIndicator);
            }
        }
    }

    public override void PerformCast()
    {
        hitPoint = cam.transform.position + cam.transform.forward * 1000;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, castRange, layerMask))
        {
            if (hit.collider)
            {
                hitPoint = hit.point;

                if (!spawnedTeleportMarker)
                {
                    Vector3 position = new Vector3(hit.point.x, hit.point.y + teleportMarker.transform.localScale.y, hit.point.z);
                    spawnedTeleportMarker = Instantiate(teleportMarker, hit.point, teleportMarker.transform.rotation);
                    Ability ability = playerAbilities.SwitchAbilities("SlideTeleport", "SlideTeleportActivate");
                    if(ability is SlideTeleportActivate slideTeleportActivate)
                    {
                        slideTeleportActivate.SetMarker(spawnedTeleportMarker);
                    }
                }
                else
                {
                    Destroy(spawnedTeleportMarker);
                }

            }
        }

        if (spawnedIndicator)
        {
            Destroy(spawnedIndicator);
        }
    }
}
