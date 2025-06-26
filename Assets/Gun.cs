using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Gun : MonoBehaviour
{

    public GunData gunData;
    protected bool isAiming;
    protected float aimProgress;

    protected float shootTimer;

    public LayerMask layerMask;

    PlayerData playerData;

    GameObject cam;

    Recoil recoil;

    protected int ammo;

    protected int magCount;

    protected Animator anim;

    bool isReloading = false;

    protected PlayerInterfaceManager playerInterfaceManager;

    float reloadTimer;

    bool canShoot = true;

    protected float fireRate;

    float damageMultiplier = 1f;

    RaycastHit[] hitBuffer = new RaycastHit[10];

    [HideInInspector] public UnityEvent sightAttached;
    [HideInInspector] public UnityEvent shot;
    [HideInInspector] public UnityEvent reload;
    [HideInInspector] public UnityEvent shotHit;

    bool useAmmo = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (transform.parent.parent && transform.parent.parent.name == "GunPosition")
        {
            ammo = gunData.magazineSize;
            cam = transform.parent.parent.parent.gameObject;
            playerData = transform.parent.parent.parent.parent.parent.parent.GetComponent<PlayerData>();
            playerInterfaceManager = transform.parent.parent.parent.parent.parent.parent.GetComponent<PlayerInterfaceManager>();
            UpdateAmmoCount();
            recoil = transform.parent.parent.parent.parent.GetComponent<Recoil>();
            recoil.SetData(gunData.recoilX, gunData.recoilY, gunData.recoilZ, gunData.snap, gunData.returnSpeed);
            //transform.parent.GetComponent<Collider>().enabled = false;
            transform.parent.localPosition = gunData.position;
            shootTimer = gunData.fireRate;
            fireRate = gunData.fireRate;
            anim = transform.parent.GetComponent<Animator>();
            if (playerInterfaceManager)
            {
                playerInterfaceManager.UpdateAmmoText(ammo);
                playerInterfaceManager.UpdateMagText(magCount);
            }

        }
        else if (!transform.parent)
        {
            enabled = false;
            transform.parent.GetComponent<Rigidbody>().isKinematic = false;
            transform.parent.GetComponent<Collider>().enabled = true;
        }

    }

    public float GetFireRate()
    {
        return fireRate;
    }

    public void SetFireRate(float fr)
    {
        fireRate = fr;
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }

    public bool CanShoot()
    {
        return canShoot;
    }

    public void SetUseAmmo(bool value)
    {
        useAmmo = value;
    }

    public bool GetUseAmmo()
    {
        return useAmmo;
    }

    public GameObject GetCamera()
    {
        return cam;
    }

    public void SetPlayerInterface(PlayerInterfaceManager manager)
    {
        playerInterfaceManager = manager;
    }

    public PlayerInterfaceManager GetPlayerInterfaceManager()
    {
        return playerInterfaceManager;
    }

    public int GetAmmoCount()
    {
        return ammo;
    }

    public int GetMagCount()
    {
        return magCount;
    }

    public void UpdateAmmoCount()
    {

    }



    // Update is called once per frame
    void Update()
    {
        if (!playerData || !playerData.GetOwnership()) return;

        Aiming();
        Shoot();
        Reload();
    }

    public void Recoil()
    {
        recoil.RecoilFire();
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    void Reload()
    {
        if(ammo <= 0)
        {
            isReloading = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (anim)
            {
                anim.SetTrigger("reload");
            }
            
            isReloading = true;
        }

        if (isReloading)
        {
            
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= gunData.reloadTime)
            {
                if (anim)
                {
                    anim.SetTrigger("unreload");
                }

                reload.Invoke();
                isReloading = false;
                ammo = gunData.magazineSize;
                reloadTimer = 0;
                playerInterfaceManager.UpdateMagText(magCount);
                playerInterfaceManager.UpdateAmmoText(ammo);

                

            }
        }
    }

    public void FinishReload()
    {
        reloadTimer = gunData.reloadTime;
    }

    public virtual void Shoot()
    {
        
    }


    public bool IsADS()
    {
        return isAiming;
    }

    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = multiplier;
    }

    public virtual Vector3 ProjectileRaycast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000, layerMask)) 
        {
            return hit.point;
        }
        return cam.transform.position + cam.transform.forward * 1000f;
    }

    public virtual RaycastHit Raycast()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000, layerMask))
        {
            Health health = hit.collider.GetComponent<Health>();
            UnitData data = hit.collider.GetComponent<UnitData>();
            if(health && data && data.GetTeam() == UnitData.Team.BAD)
            {
                shotHit.Invoke();
                if(damageMultiplier < 1f)
                {
                    damageMultiplier = 1f;
                }
                Debug.Log(gunData.damage * damageMultiplier);
                health.TakeDamageServerRpc(gunData.damage * damageMultiplier);
            }
            return hit;
        }
        return default;
    }

    void Aiming()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            isAiming = true;
        }
        else if(Input.GetButtonUp("Fire2"))
        {
            isAiming = false;
        }
        
        if (isAiming)
        {
            aimProgress += Time.deltaTime * gunData.adsTime;

        }
        else
        {
            aimProgress -= Time.deltaTime * gunData.adsTime;
        }

        aimProgress = Mathf.Clamp01(aimProgress);
        
    }
}
