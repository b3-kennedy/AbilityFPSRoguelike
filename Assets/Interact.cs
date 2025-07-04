using UnityEngine;

public class Interact : MonoBehaviour
{
    public LayerMask layerMask;
    public GameObject cam;
    public float interactRange;

    public GameObject interactMessage;
    GameObject selectedObject;

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit,interactRange, layerMask, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.CompareTag("Chest"))
            {
                selectedObject = hit.collider.gameObject;
                Interactable interactable = selectedObject.GetComponent<Interactable>();
                
                if(interactable is Chest chest)
                {
                    chest.interactText = interactMessage;
                }
                selectedObject.GetComponent<Interactable>().OnHover();
            }
            else if (hit.collider.CompareTag("Upgrade"))
            {
                selectedObject = hit.collider.gameObject;
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable)
                {
                    interactable.OnHover();
                }
            }
            else
            {
                if (selectedObject)
                {
                    selectedObject.GetComponent<Interactable>().OnUnHover();
                }
                
            }
            
        }
        else
        {
            interactMessage.SetActive(false);
            if (selectedObject)
            {
                selectedObject.GetComponent<Interactable>().OnUnHover();
            }
            
        }


        if(Input.GetKeyDown(KeyCode.E) && selectedObject)
        {
            Interactable interactable = selectedObject.GetComponent<Interactable>();
            if (interactable)
            {
                //interactable.interactingPlayer = gameObject;
                interactable.Interact();
            }
        }
    }
}
