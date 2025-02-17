using UnityEngine;
using UnityEngine.Events;

public class CallbackInteractable : Interactable
{
    [SerializeField] UnityEvent _eventCallback;

    public override void Interact(GameObject interactor)
    {
        _eventCallback?.Invoke();
    }
}
