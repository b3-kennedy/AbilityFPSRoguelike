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


            if (movement.IsGrounded() && movement.IsOnSlope())
            {
                RaycastHit hit;

                float capsuleHeight = 2f;
                float radius = 0.5f;
                float castDistance = 0.1f; // How far below the capsule to cast

                Vector3 center = player.transform.position;
                Vector3 top = center + Vector3.up * ((capsuleHeight / 2f) - radius);
                Vector3 bottom = center + Vector3.down * ((capsuleHeight / 2f) - radius);

                // Cast down to detect the ground
                if (Physics.CapsuleCast(top, bottom, radius - 0.01f, Vector3.down, out hit, castDistance + 0.01f, groundLayer))
                {
                    Vector3 slopeDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                    rb.AddForce(slopeDirection * 2.5f, ForceMode.Force);
                }

            }

            if (!movement.IsOnSlope() && rb.linearVelocity.magnitude <= 0.1f)
            {
                hasCast = false;
                movement.canInput = true;
                movement.enableSpeedControl = true;
                movement.groundDrag = startDrag;
                gun.SetUseAmmo(true);
            }
        }
    }


}
