using Unity.Netcode;
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
    public float fuelConsumptionMultiplier;

    public float fuelRechargeMultiplier;

    public float fuel;

    GameObject fuelBar;

    public override void OnInitialise()
    {
        playerData = GetCaster().GetComponent<PlayerData>();
        if (!playerData.GetOwnership()) return;
        fuel = maxFuel;
        rb = GetCaster().GetComponent<Rigidbody>();

        
        
        move = GetCaster().GetComponent<PlayerMovement>();
        fuelBar = GetCaster().transform.Find("PlayerInterface/FuelBackground/Fuel").gameObject;
        isJetPacking = false;

        Debug.Log(force);
    }

    public override void FixedUpdateAbility()
    {
        if(isJetPacking && fuel > 0)
        {
            if (rb.linearVelocity.y < maxUpwardVelocity)
            {
                rb.AddForce(Vector3.up * force, ForceMode.Force);
            }
        }
    }

    public override void UpdateAbility()
    {
        if (!playerData.GetOwnership()) return;
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
        }

        if (!isJetPacking && fuel < maxFuel)
        {
            fuel += Time.deltaTime * fuelRechargeMultiplier;
        }

        fuelBar.transform.localScale = new Vector3(fuelBar.transform.localScale.x, fuel/maxFuel , fuelBar.transform.localScale.z);
    }

}
