using UnityEngine;

public class PlayerPaint : MonoBehaviour
{
    [SerializeField] private float _radius = 1;
    [SerializeField] private float _strength = 1;
    [SerializeField] private float _hardness = 1;

    private Camera _mainCamera;

    private int _ignoreLayers;

    private void OnEnable()
    {
        _mainCamera = Camera.main;
        _ignoreLayers = LayerMask.GetMask("Triggers")
                        | LayerMask.GetMask("AudioZone")
                        | LayerMask.GetMask("Interactable")
                        | LayerMask.GetMask("HeldObject");
    }

    public void Paint()
    {
        Draw(Color.green, _radius);
    }

    public void Erase()
    {
        Draw(new Color(0, 0, 0, 0), _radius * 1.5f);
    }

    private void Draw(Color color, float radius)
    {
        Vector3 position = Input.mousePosition;
        Ray ray = _mainCamera.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 7.0f, ~_ignoreLayers))
        {
            Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.red);
            Paintable paintable = hit.collider.GetComponent<Paintable>();

            if (paintable)
                paintable.Paint(hit.point, radius, _strength, _hardness, color);
        }
    }
}
