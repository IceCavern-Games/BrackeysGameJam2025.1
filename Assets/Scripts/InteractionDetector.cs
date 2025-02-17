using Reflex.Attributes;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [Inject] private readonly UIManager _uiManager;

    public Interactable CurrentInteractable { get; private set; }

    [SerializeField] private float _detectionRange = 1.0f;
    [SerializeField] private float _detectionRadius = 0.5f;
    [SerializeField] private float _height = 1.0f;
    [SerializeField] private LayerMask _detectionLayer;

    public void DetectInteractable()
    {
        if (Camera.main == null) return;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 detectionOrigin = transform.position + Vector3.up * _height + cameraForward * _detectionRange;

        Collider[] results = Physics.OverlapSphere(detectionOrigin, _detectionRadius, _detectionLayer);

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

    private void ClearCurrentInteractable()
    {
        CurrentInteractable = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;

        Gizmos.color = CurrentInteractable != null ? Color.green : Color.blue;

        // Use the camera's forward direction instead of the player's forward direction
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 center = transform.position + Vector3.up * _height + cameraForward * _detectionRange;

        Gizmos.DrawWireSphere(center, _detectionRadius);
    }

}
