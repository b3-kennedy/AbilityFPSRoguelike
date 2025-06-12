using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Float : Ability
{
    Rigidbody rb;
    PlayerData playerData;
    PlayerMovement move;

    bool isJetPacking;

    public float force;
    public float maxUpwardVelocity;

    public float maxFuel = 100f;
    public float fuelConsumptionMultiplier = 1f;

    public float fuelRechargeMultiplier = 1f;

    public float fuel;

    public override void OnInitialise()
    {
        fuel = maxFuel;
        rb = GetCaster().GetComponent<Rigidbody>();
        playerData = GetCaster().GetComponent<PlayerData>();
        move = GetCaster().GetComponent<PlayerMovement>();
        isJetPacking = false;
    }

    public override void UpdateAbility()
    {
        if (Input.GetKeyDown(playerData.jumpKey) && !move.IsGrounded())
        {
            isJetPacking = true;
        }
        else if (Input.GetKeyUp(playerData.jumpKey) && !move.IsGrounded())
        {
            isJetPacking = false;
        }


        if (isJetPacking && fuel > 0)
        {
            fuel -= Time.deltaTime * fuelConsumptionMultiplier;
            if (rb.linearVelocity.y < maxUpwardVelocity)
            {
                rb.AddForce(Vector3.up * force, ForceMode.Force);
            }
        }

        if (!isJetPacking && fuel < maxFuel)
        {
            fuel += Time.deltaTime * fuelRechargeMultiplier;
        }
    }

}
