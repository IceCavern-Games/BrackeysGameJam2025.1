using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string InteractionPrompt = "Interact";

    public virtual void Interact(GameObject interactor) { }
}
