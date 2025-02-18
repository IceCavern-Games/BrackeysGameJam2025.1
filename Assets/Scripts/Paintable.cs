using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class Paintable : MonoBehaviour
{
    [SerializeField] private Shader _paintableShader;
    [SerializeField] private Material _preview;
    
    private static readonly int _prepareUVId = Shader.PropertyToID("_PrepareUV");
    private static readonly int _textureId = Shader.PropertyToID("_MaskTex");
    private static readonly int _positionId = Shader.PropertyToID("_PainterPosition");
    private static readonly int _radiusId = Shader.PropertyToID("_Radius");
    private static readonly int _hardnessId = Shader.PropertyToID("_Hardness");
    private static readonly int _strengthId = Shader.PropertyToID("_Strength");
    private static readonly int _colorId = Shader.PropertyToID("_PainterColor");
    
    private Material _baseMaterial;
    private Material _paintableMaterial;
    private Camera _mainCamera;
    private CommandBuffer _commandBuffer;
    private Texture _baseTexture;

    private RenderTexture _mask;
    private RenderTexture _support;
    
    
    private void Start()
    {
        _mainCamera = Camera.main;
        _baseMaterial = GetComponent<Renderer>().material;
        _baseTexture = _baseMaterial.mainTexture;

        int width = _baseTexture ? _baseTexture.width : 1024;
        int height = _baseTexture ? _baseTexture.height : 1024;
        
        _mask = new RenderTexture(width, height, 0, RenderTextureFormat.Default);
        _support = new RenderTexture(width, height, 0, RenderTextureFormat.Default);

        _commandBuffer = new CommandBuffer();
        _paintableMaterial = new Material(_paintableShader);
        _paintableMaterial.SetFloat(_prepareUVId, 1);
        
        _commandBuffer.name = $"Paintable Command Buffer: {gameObject.name}";
        _commandBuffer.SetRenderTarget(_mask);
        _commandBuffer.DrawRenderer(GetComponent<Renderer>(), _paintableMaterial, 0);
        _commandBuffer.SetRenderTarget(_support);
        _commandBuffer.Blit(_mask, _support);
        Graphics.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear();
        
        _preview.SetTexture("_BaseMap", _support);
    }

    public void Paint(Vector3 position, float radius, float hardness, float strength, Color color)
    {
        _paintableMaterial.SetFloat(_prepareUVId, 0);
        _paintableMaterial.SetVector(_positionId, position);
        _paintableMaterial.SetFloat(_radiusId, radius);
        _paintableMaterial.SetFloat(_hardnessId, hardness);
        _paintableMaterial.SetFloat(_strengthId, strength);
        _paintableMaterial.SetColor(_colorId, color);
        _paintableMaterial.SetTexture(_textureId, _support);
        
        _commandBuffer.SetRenderTarget(_mask);
        _commandBuffer.DrawRenderer(GetComponent<Renderer>(), _paintableMaterial, 0);
        
        _commandBuffer.SetRenderTarget(_support);
        _commandBuffer.Blit(_mask, _support);
        
        Graphics.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear();
    }
}
