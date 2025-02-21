using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HRProximityTrigger : MonoBehaviour
{
    [SerializeField] private ShadingRenderPassFeature _renderPassFeature;
    [SerializeField] private float _radius = 15;
    [SerializeField] private float _maxValueRadius = 5.0f;
    
    private GameObject _player;
    private Vector2 _positionXZ;
    
    private void OnEnable()
    {
        _player = FindFirstObjectByType<FirstPersonController>().gameObject;
        _positionXZ = new Vector2(transform.position.x, transform.position.z);
    }

    private void Update()
    {
        Vector2 playerPosXZ = new Vector2(_player.transform.position.x, _player.transform.position.z);
        float distance = Vector2.Distance(_positionXZ, playerPosXZ);
        
        if (distance > _radius)
            return;

        float t = Mathf.Clamp01(1 - (distance - _maxValueRadius) / (_radius - _maxValueRadius));
        Debug.Log(t);
        
        Color lerpedColor = Color.Lerp(Color.white, Color.red, t);
        _renderPassFeature._settings.color = lerpedColor;
    }

    private void OnDestroy()
    {
        _renderPassFeature._settings.color = Color.white;
    }
}
