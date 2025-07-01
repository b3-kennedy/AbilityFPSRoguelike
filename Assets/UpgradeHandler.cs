using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.Netcode;
using System.Runtime.Serialization;



public class UpgradeHandler : NetworkBehaviour
{
    [HideInInspector] public UnityEvent added;

    public List<UpgradeEffect> upgrades = new List<UpgradeEffect>();

    private void Start()
    {
        added.AddListener(OnUpgradeAdded);
    }

    public void AddUpgrade(UpgradeEffect upgrade)
    {
        if (!IsOwner) return;

        if (!upgrades.Contains(upgrade))
        {
            upgrade.count = 0;
            upgrades.Add(upgrade);
            IncreaseUpgradeCount(upgrade);
        }
        else
        {
            Debug.Log("increase count");
            IncreaseUpgradeCount(upgrade);
            //increase count variable in upgradeeffect base class
        }
        
        upgrade.SetPlayer(gameObject);
        upgrade.Apply();
        added.Invoke();
    }

    void IncreaseUpgradeCount(UpgradeEffect effect)
    {
        foreach (var upg in upgrades)
        {
            if (upg == effect)
            {
                upg.count++;
            }
        }
    }

    void OnUpgradeAdded()
    {
        //upgrades[upgrades.Count - 1].Apply();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Upgrade"))
        {
            UpgradeEffect upgrade = other.GetComponent<UpgradeHolder>().effect;
            if (upgrade)
            {
                AddUpgrade(upgrade);
            }
            ObjectSpawnManager.Instance.DestroyObjectServerRpc(other.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }


}
