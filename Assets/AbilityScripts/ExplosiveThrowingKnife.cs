using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ExplosiveThrowingKnife : Ability
{
    Transform throwPoint;
    public GameObject throwingKnifePrefab;
    public float force;
    public List<GameObject> knives = new List<GameObject>();
    public float explosionDamage;


    public override void OnInitialise()
    {
        base.OnInitialise();
        throwPoint = GetCaster().transform.Find("CameraHolder/Recoil/Camera/GunPosition/GunParent/Gun/ThrowPoint");
        knives.Clear();

    }
    
    public override void PerformCast()
    {
        GameObject spawnedThrowingKnife = Instantiate(throwingKnifePrefab, throwPoint.position, Quaternion.identity);
        knives.Add(spawnedThrowingKnife);
        string projectileID = System.Guid.NewGuid().ToString();
        spawnedThrowingKnife.GetComponent<Projectile>().SetValues(force, GetCamera().transform.forward, projectileID);
        spawnedThrowingKnife.GetComponent<ExplosiveThrowingKnifeProjectile>().clientId = NetworkManager.Singleton.LocalClientId;
        spawnedThrowingKnife.GetComponent<ExplosiveThrowingKnifeProjectile>().player = GetCaster();
        spawnedThrowingKnife.GetComponent<ExplosiveThrowingKnifeProjectile>().knifeType = ExplosiveThrowingKnifeProjectile.KnifeType.LOCAL;
        ProjectileManager.Instance.SpawnThrowingKnifeServerRpc(NetworkManager.Singleton.LocalClientId, "ThrowingKnife", throwPoint.position, GetCamera().transform.forward, force, projectileID);
    }

    public override void UpdateAbility()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            GetCaster().GetComponent<ServerKnifeHolder>().DestroyKnivesServerRpc(NetworkManager.Singleton.LocalClientId,explosionDamage);
        }
    }
}
