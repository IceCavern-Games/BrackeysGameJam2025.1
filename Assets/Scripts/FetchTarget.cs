using UnityEngine;

public class FetchTarget : MonoBehaviour
{
    [SerializeField] private FetchTask _gameTask;

    private Collider _collider;
    private MeshRenderer _meshRenderer;

    private void OnEnable()
    {
        if (_gameTask != null)
            _gameTask.FetchTarget = this;

        _collider = GetComponent<Collider>();
        _meshRenderer = GetComponent<MeshRenderer>();

        SetEnabled(false);
    }

    public void SetEnabled(bool isEnabled)
    {
        _meshRenderer.enabled = isEnabled;
    }

    public bool IsPositionInside(Vector3 position)
    {
        return _collider.bounds.Contains(position);
    }
}
