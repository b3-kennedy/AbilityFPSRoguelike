using UnityEngine;

public class Slide : Ability
{
    GameObject player;
    Rigidbody rb;
    public float maxSlideTime;
    float slideTimer;
    PlayerMovement movement;
    bool hasCast = false;
    float startDrag;
    Transform groundCheck;
    public LayerMask groundLayer;
    Gun gun;

    public override void OnInitialise()
    {
        base.OnInitialise();
        hasCast = false;
        slideTimer = 0;
        player = GetCaster();
        rb = player.GetComponent<Rigidbody>();
        movement = player.GetComponent<PlayerMovement>();
        startDrag = movement.groundDrag;
        groundCheck = movement.groundCheck;
        gun = player.GetComponent<PlayerData>().GetGunParent().GetChild(0).GetChild(0).GetComponent<Gun>();
    }

    public override void PerformCast()
    {
        hasCast = true;
        movement.canInput = false;
        movement.enableSpeedControl = false;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 launchVector = new Vector3(horizontal, 0, vertical);
        Vector3 launchDir = movement.orientation.TransformDirection(launchVector);
        rb.AddForce(launchDir * 10f, ForceMode.Impulse);
        movement.groundDrag = 0;
    }

    public override void UpdateAbility()
    {
        if (hasCast)
        {
            slideTimer += Time.deltaTime;

            gun.SetUseAmmo(false);

            slideTimer += Time.deltaTime;

            if (GetGroundSlope(out Vector3 slopeDir, out float angle))
            {
                float slideStrength = Mathf.Lerp(2f, 10f, angle / 45f);
                rb.AddForce(slopeDir * slideStrength, ForceMode.Force);
            }


            if (Input.GetKeyUp(GetKey()) || rb.linearVelocity.magnitude <= 0.1f)
            {
                hasCast = false;
                movement.canInput = true;
                movement.enableSpeedControl = true;
                movement.groundDrag = startDrag;
                gun.SetUseAmmo(true);
                slideTimer = 0;
            }
        }
    }

    private bool GetGroundSlope(out Vector3 slopeDir, out float angle)
    {
        slopeDir = Vector3.zero;
        angle = 0f;

        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, 1.5f, groundLayer))
        {
            angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > 0.1f) // Any slope, even shallow ones
            {
                slopeDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                return true;
            }
        }
        return false;
    }


}
