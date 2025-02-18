using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PickupInteractable : Interactable
{
    private Rigidbody _rigidbody;
    private Transform _holdPosition;
    private bool _isHeld = false;

    private readonly float _correctionForce = 100.0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void LateUpdate()
    {
        if (_isHeld && _holdPosition != null)
        {
            var force = _holdPosition.position - transform.position;

            _rigidbody.linearVelocity = force.normalized * _rigidbody.linearVelocity.magnitude;
            _rigidbody.AddForce(force * _correctionForce);
            _rigidbody.linearVelocity *= Mathf.Min(1.0f, force.magnitude / 2);
        }
    }

    public override void Interact(GameObject interactor)
    {
        var pickup = interactor.GetComponent<PlayerPickup>();

        if (_isHeld)
            pickup.DropObject();
        else
            pickup.PickUpObject(this);
    }

    public void Drop()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");

        _holdPosition = null;
        _rigidbody.useGravity = true;
        _rigidbody.constraints = RigidbodyConstraints.None;
        _isHeld = false;
    }

    public void PickUp(Transform holdPosition)
    {
        gameObject.layer = LayerMask.NameToLayer("HeldObject");

        _holdPosition = holdPosition;
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _isHeld = true;
    }
}
