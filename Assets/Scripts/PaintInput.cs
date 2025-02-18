using System;
using UnityEngine;

public class PaintInput : MonoBehaviour
{
    [SerializeField] private float _radius = 1;
    [SerializeField] private float _strength = 1;
    [SerializeField] private float _hardness = 1;

    private Camera _mainCamera;
    
    private void OnEnable()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        bool clicked = Input.GetMouseButton(0);

        if (clicked)
        {
            Vector3 position = Input.mousePosition;
            Ray ray = _mainCamera.ScreenPointToRay(position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.red);
                Paintable paintable = hit.collider.GetComponent<Paintable>();

                if (paintable)
                {
                    paintable.Paint(hit.point, _radius, _strength, _hardness, Color.green);
                }
            }
        }
    }
}
