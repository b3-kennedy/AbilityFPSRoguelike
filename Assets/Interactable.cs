using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void OnHover() { }

    public virtual void Interact() { }

    public virtual void OnUnHover() { }
}
