using System;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Interactable : MonoBehaviour
{
    public string InteractionPrompt = "Interact";
    [SerializeField] protected Material _outlineMaterial;

    protected Renderer _renderer;
    
    public void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (_renderer)
        {
            int numMaterials = _renderer.sharedMaterials.Length;
            var materials = _renderer.materials;
            Array.Resize(ref materials, numMaterials + 1);
            materials[numMaterials] = _outlineMaterial;
            _renderer.materials = materials;
        }
    }

    public virtual void Interact(GameObject interactor) { }
}
