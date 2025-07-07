using UnityEngine;

public class Charge : Ability
{

    float baseFireRate;
    public float duration = 3f;
    public float fireRateMultiplier = 2f;
    Gun gun;
    float timer = 0;
    bool isActive = false;
    PlayerMovement movement;
    Rigidbody rb;
    public float chargeSpeed = 6f;
    public float sensitivityMultiplier = 4f;
    float baseSens;
    PlayerLook look;
    Collider[] colliders = new Collider[20];
    public float pushForce = 5f;
    public override void OnInitialise()
    {
        base.OnInitialise();
        gun = GetGun();
        isActive = false;
        timer = 0;
        gun.SetUseAmmo(true);
        movement = GetCaster().GetComponent<PlayerMovement>();
        movement.canInput = true;
        rb = GetCaster().GetComponent<Rigidbody>();
        look = GetCaster().GetComponent<PlayerLook>();

    }

    public override void PerformCast()
    {
        baseFireRate = gun.GetFireRate();
        baseSens = look.sensitivity;
        float increasedFireRate = baseFireRate / fireRateMultiplier;
        look.sensitivity /= sensitivityMultiplier;
        gun.SetFireRate(increasedFireRate);
        gun.SetUseAmmo(false);
        isActive = true;
        movement.canInput = false;
        
    }

    public override void UpdateAbility()
    {
        if (isActive)
        {
            int count = Physics.OverlapSphereNonAlloc(GetCaster().transform.position, 1.5f, colliders);
            for (int i = 0; i < count; i++)
            {
                Rigidbody colliderRb = colliders[i].GetComponent<Rigidbody>();
                if (colliderRb)
                {
                    Vector3 direction = (GetCaster().transform.position - colliders[i].transform.position).normalized;
                    EnemyMove eMove = colliders[i].GetComponent<EnemyMove>();
                    if (eMove)
                    {
                        eMove.OnApplyForce(-direction, pushForce, ForceMode.Impulse);
                    }
                    else 
                    {
                        colliderRb.AddForce(-direction * pushForce, ForceMode.Impulse);
                    }
                    
                }
                

            }
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 chargeVelocity = movement.orientation.forward * chargeSpeed;
            chargeVelocity.y = currentVelocity.y;

            rb.linearVelocity = chargeVelocity;
            timer += Time.deltaTime;
            if(timer >= duration)
            {
                Debug.Log("ENDED");
                timer = 0;
                gun.SetFireRate(baseFireRate);
                gun.SetUseAmmo(true);
                look.sensitivity = baseSens;
                isActive = false;
                movement.canInput = true;
            }
        }
    }
}
