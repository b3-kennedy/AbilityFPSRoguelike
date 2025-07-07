using System.Collections.Generic;
using Unity.Netcode;
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
    PlayerData playerData;
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
        playerData = GetCaster().GetComponent<PlayerData>();

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
            int count = Physics.OverlapSphereNonAlloc(GetCaster().transform.position, 4f, colliders);
            HashSet<ulong> processed = new HashSet<ulong>();

            for (int i = 0; i < count; i++)
            {
                NetworkObject netObj = colliders[i].GetComponent<NetworkObject>();
                if (netObj == null) continue;

                ulong netId = netObj.NetworkObjectId;

                if (processed.Contains(netId))
                    continue;

                processed.Add(netId);

                Rigidbody colliderRb = colliders[i].GetComponent<Rigidbody>();
                if (colliderRb)
                {
                    Vector3 direction = (GetCaster().transform.position - colliders[i].transform.position).normalized;
                    EnemyMove eMove = colliders[i].GetComponent<EnemyMove>();

                    if (eMove)
                    {
                        ProjectileManager.Instance.AddForceToEnemyServerRpc(
                            GetCaster().GetComponent<NetworkObject>().NetworkObjectId,
                            netId,
                            pushForce,
                            ForceMode.Impulse
                        );
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
                processed.Clear();
            }
        }
    }
}
