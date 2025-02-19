using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class Paintable : MonoBehaviour
{
    [Inject] private PaintTextureManager _paintTextureManager;
    
    [SerializeField] private Shader _paintableShader;
    
    private static readonly int _prepareUVId = Shader.PropertyToID("_PrepareUV");
    private static readonly int _textureId = Shader.PropertyToID("_MaskTex");
    private static readonly int _positionId = Shader.PropertyToID("_PainterPosition");
    private static readonly int _radiusId = Shader.PropertyToID("_Radius");
    private static readonly int _hardnessId = Shader.PropertyToID("_Hardness");
    private static readonly int _strengthId = Shader.PropertyToID("_Strength");
    private static readonly int _colorId = Shader.PropertyToID("_PainterColor");
    
    private Material _paintDrawMaterial;
    private Material _paintableMaterial;
    private CommandBuffer _commandBuffer;

    private PaintTextureManager.PaintTexture _paintTexture;
    private bool _hasPaintTexture = false;

    private Renderer _renderer;
    private UniqueObject _uniqueObject;
    
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _uniqueObject = GetComponent<UniqueObject>();
        
        _paintDrawMaterial = _renderer.materials[1];
        _paintableMaterial = new Material(_paintableShader);
        

        if (_paintTextureManager.GetTexture(_uniqueObject.Id, out _paintTexture))
        {
            _commandBuffer = new CommandBuffer();
            _hasPaintTexture = true;
            _paintDrawMaterial.SetTexture("_MaskTexture", _paintTexture.Support);
        }
        
    }

    public void Paint(Vector3 position, float radius, float hardness, float strength, Color color)
    {
        if (!_hasPaintTexture)
            InitializePaintTexture();
        
        _paintableMaterial.SetFloat(_prepareUVId, 0);
        _paintableMaterial.SetVector(_positionId, position);
        _paintableMaterial.SetFloat(_radiusId, radius);
        _paintableMaterial.SetFloat(_hardnessId, hardness);
        _paintableMaterial.SetFloat(_strengthId, strength);
        _paintableMaterial.SetColor(_colorId, color);
        _paintableMaterial.SetTexture(_textureId, _paintTexture.Support);
        
        _commandBuffer.SetRenderTarget(_paintTexture.Mask);
        _commandBuffer.DrawRenderer(_renderer, _paintableMaterial, 0);
        
        _commandBuffer.SetRenderTarget(_paintTexture.Support);
        _commandBuffer.Blit(_paintTexture.Mask, _paintTexture.Support);
        
        Graphics.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear();
    }

    private void InitializePaintTexture()
    {
        _paintTextureManager.CreateTexture(_uniqueObject.Id, out _paintTexture);
        
        _paintableMaterial.SetFloat(_prepareUVId, 1);
        _commandBuffer = new CommandBuffer();
        _commandBuffer.name = $"Paintable Command Buffer: {gameObject.name}";
        _commandBuffer.SetRenderTarget(_paintTexture.Mask);
        _commandBuffer.DrawRenderer(_renderer, _paintableMaterial, 0);
        _commandBuffer.SetRenderTarget(_paintTexture.Support);
        _commandBuffer.Blit(_paintTexture.Mask, _paintTexture.Support);
        Graphics.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear();

        _paintDrawMaterial.SetTexture("_MaskTexture", _paintTexture.Support);
        _hasPaintTexture = true;
    }
}
