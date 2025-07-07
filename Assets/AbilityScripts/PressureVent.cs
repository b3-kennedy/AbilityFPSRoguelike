using UnityEngine;
using UnityEngine.Windows.WebCam;

public class PressureVent : Ability
{

    PlayerAbilities playerAbilities;
    int combo = 0;
    public float baseForce;
    GameObject cam;
    public float castRange = 10f;
    public LayerMask layerMask;
    Rigidbody rb;

    public override void OnInitialise()
    {
        base.OnInitialise();
        playerAbilities = GetCaster().GetComponent<PlayerAbilities>();
        cam = GetCamera();
        rb = GetCaster().GetComponent<Rigidbody>();
        combo = 0;
    }

    public override void PerformCast()
    {
        Ability ability = playerAbilities.GetAbilityByName("TrickReload");
        if (ability is TrickReload trick)
        {
            combo = trick.GetCombo();
        }

        if(Physics.Raycast(cam.transform.position, cam.transform.forward,out RaycastHit hit, castRange, layerMask))
        {
            Rigidbody hitRb = hit.collider.GetComponent<Rigidbody>();
            if (hit.collider && hitRb)
            {
                Vector3 direction = (cam.transform.position - hit.point).normalized;
                EnemyMove eMove = hit.collider.gameObject.GetComponent<EnemyMove>();
                if (eMove)
                {
                    eMove.OnApplyForce(-direction, baseForce * combo, ForceMode.Impulse);
                }
                else
                {
                    hitRb.AddForce(-direction * baseForce * combo, ForceMode.Impulse);
                }
                
                Vector3 playerDirection = (GetCaster().transform.position - hit.point).normalized;
                rb.AddForce(playerDirection * baseForce * combo, ForceMode.Impulse);
            }
            else if(hit.collider && !hitRb)
            {
                Vector3 direction = (GetCaster().transform.position - hit.point).normalized;
                rb.AddForce(direction * baseForce * combo, ForceMode.Impulse);
            }
            
        }
    }
}
