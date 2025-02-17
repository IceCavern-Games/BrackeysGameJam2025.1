using Reflex.Attributes;
using UnityEngine;

[RequireComponent(typeof(PlayerPickup))]
public class InteractionDetector : MonoBehaviour
{
    public bool CanInteract { get; set; } = true;
    public Interactable CurrentInteractable { get; private set; }

    [Inject] private readonly UIManager _uiManager;

    [SerializeField] private float _detectionRange = 1.0f;
    [SerializeField] private float _detectionRadius = 0.5f;
    [SerializeField] private float _height = 1.0f;
    [SerializeField] private LayerMask _detectionLayer;

    private PlayerPickup _pickup;

    private void Awake()
    {
        _pickup = GetComponent<PlayerPickup>();
    }

    public void DetectInteractable()
    {
        if (Camera.main == null) return;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 capsuleStart = transform.position + Vector3.up * _height;
        Vector3 capsuleEnd = capsuleStart + cameraForward * _detectionRange;

        if (_pickup.HeldObject != null)
            return;

        Collider[] results = Physics.OverlapCapsule(capsuleStart, capsuleEnd, _detectionRadius, _detectionLayer);
        Interactable firstInteractable = null;

        // Iterate through results to find the first valid interactable
        foreach (var collider in results)
        {
            Interactable interactable = collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                firstInteractable = interactable;
                break;
            }
        }

        // Update interactable state
        if (firstInteractable != null && firstInteractable != CurrentInteractable)
        {
            ClearCurrentInteractable();

            CurrentInteractable = firstInteractable;
            _uiManager.Gameplay.SetInteractPrompt(firstInteractable.InteractionPrompt);
        }
        else if (firstInteractable == null)
        {
            ClearCurrentInteractable();
            _uiManager.Gameplay.HideInteractPrompt();
        }
    }

    public void SetHeldObject(PickupInteractable pickup)
    {
        CurrentInteractable = pickup;

        if (pickup != null)
            _uiManager.Gameplay.SetInteractPrompt("Drop");
        else
            _uiManager.Gameplay.HideInteractPrompt();
    }

    private void ClearCurrentInteractable()
    {
        CurrentInteractable = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;

        Gizmos.color = CurrentInteractable != null ? Color.green : Color.blue;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 capsuleStart = transform.position + Vector3.up * _height;
        Vector3 capsuleEnd = capsuleStart + cameraForward * _detectionRange;

        GizmoExtensions.DrawWireCapsule(capsuleStart, capsuleEnd, _detectionRadius);
    }
}
