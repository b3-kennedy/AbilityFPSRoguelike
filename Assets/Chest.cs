using Unity.Netcode;
using UnityEngine;

public class Chest : Interactable
{

    public float cost;
    [HideInInspector] public GameObject interactText;




    public override void OnHover()
    {
        interactText.SetActive(true);
    }

    public override void Interact()
    {
        if(GameManager.Instance.money >= cost)
        {
            GameManager.Instance.OpenChest(transform.position);
            GameManager.Instance.money -= cost;
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInterfaceManager>().moneyText.text = "$"+GameManager.Instance.money;
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Not enough money");
        }
        
        
    }

    public override void OnUnHover()
    {
        interactText.SetActive(false);
    }
}
