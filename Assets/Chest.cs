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
        GameManager.Instance.OpenChest(transform.position);
        Destroy(gameObject);
    }

    public override void OnUnHover()
    {
        interactText.SetActive(false);
    }
}
