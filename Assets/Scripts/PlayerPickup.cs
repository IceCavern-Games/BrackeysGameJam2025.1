using Reflex.Attributes;
using UnityEngine;

[RequireComponent(typeof(InteractionDetector))]
public class PlayerPickup : MonoBehaviour
{
    public PickupInteractable HeldObject { get; private set; }

    [Inject] private readonly AudioManager _audioManager;

    [SerializeField] private Transform _holdPosition;
    [SerializeField] private AudioClip _pickupSfx;

    private InteractionDetector _interactionDetector;
    private Vector3 _initialLocalPosition;

    private void Awake()
    {
        _interactionDetector = GetComponent<InteractionDetector>();
    }

    private void Start()
    {
        _initialLocalPosition = Camera.main.transform.InverseTransformPoint(_holdPosition.position);
    }

    private void Update()
    {
        if (Camera.main == null) return;

        // Ensure HoldPosition rotates with the camera
        _holdPosition.SetPositionAndRotation(Camera.main.transform.TransformPoint(_initialLocalPosition), Camera.main.transform.rotation);
    }

    public void PickUpObject(PickupInteractable pickup)
    {
        if (HeldObject != null)
            return;

        HeldObject = pickup;
        HeldObject.PickUp(_holdPosition);
        _audioManager.PlaySound(_pickupSfx);
        _interactionDetector.SetHeldObject(pickup);
    }

    public void DropObject()
    {
        if (HeldObject == null)
            return;

        HeldObject.Drop();
        HeldObject = null;
        _interactionDetector.SetHeldObject(null);
    }
}
