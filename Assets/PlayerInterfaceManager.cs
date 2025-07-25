using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterfaceManager : NetworkBehaviour
{

    public GameObject ammoUI;
    public TextMeshProUGUI gunNameText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI moneyText;

    

    PlayerData playerData;

    GameObject currentActiveUI;

    GameObject collectionBox;

    public GameObject playerInterface;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        //ammoUI.SetActive(false);
    }

    public void OnGunPickup(GameObject gun)
    {
        if (!IsOwner) return;

        Gun g = gun.transform.GetChild(0).GetComponent<Gun>();
        GunData gData = g.gunData;

        g.SetPlayerInterface(this);

        gunNameText.text = gData.gunName;

        ammoUI.SetActive(true);

    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Escape) && currentActiveUI)
        {
            Gun gun = GetComponent<PlayerData>().GetGunParent().GetChild(0).GetChild(0).GetComponent<Gun>();
            gun.SetCanShoot(true);
        }
    }
   
    public void UpdateNameText(string name)
    {
        gunNameText.text = name;
    }

    public void UpdateAmmoText(int ammo)
    {
        ammoText.text = ammo.ToString();
    }

    public void UpdateMagText(int mags)
    {
        //magText.text = mags.ToString();
    }
}
