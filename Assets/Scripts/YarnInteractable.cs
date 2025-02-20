using Reflex.Attributes;
using UnityEngine;

public class YarnInteractable : Interactable
{
    [Inject] private readonly DialogueManager _dialogueManager;
    [Inject] private readonly UIManager _uiManager;

    [SerializeField] protected string _converstationStartNode;
    [SerializeField] protected AudioClip _dialogueSFX;

    public override void Interact(GameObject interactor)
    {
        var detector = interactor.GetComponent<InteractionDetector>();

        detector.CanInteract = false;

        _dialogueManager.StartConversation(_converstationStartNode, () =>
        {
            detector.CanInteract = true;
        }, _dialogueSFX);
    }
}
