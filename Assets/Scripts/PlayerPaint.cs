using System;
using UnityEngine;

public class PlayerPaint : MonoBehaviour
{
    [SerializeField] private float _radius = 1;
    [SerializeField] private float _strength = 1;
    [SerializeField] private float _hardness = 1;

    private Camera _mainCamera;
    
    private void OnEnable()
    {
        _mainCamera = Camera.main;
    }

    public void Paint()
    {
        Draw(Color.green);
    }

    public void Erase()
    {
        Draw(new Color(0, 0, 0, 0));
    }

    private void Draw(Color color)
    {
        Vector3 position = Input.mousePosition;
        Ray ray = _mainCamera.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5.0f))
        {
            Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.red);
            Paintable paintable = hit.collider.GetComponent<Paintable>();

            if (paintable)
            {
                paintable.Paint(hit.point, _radius, _strength, _hardness, color);
            }
        }
    }
}
