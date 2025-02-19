using System;
using System.Collections.Generic;
using UnityEngine;

public class PaintTextureManager : MonoBehaviour
{
    public struct PaintTexture
    {
        public RenderTexture Mask;
        public RenderTexture Support;
    }
    
    private const int TEXTURE_SIZE = 512;

    private Dictionary<string, PaintTexture> _textureMap;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        _textureMap = new Dictionary<string, PaintTexture>();
    }

    /// <summary>
    /// Gets a texture from the game's texture map
    /// </summary>
    /// <param name="id">Key in the map</param>
    /// <param name="texture"></param>
    /// <returns>Returns true if texture is already in the map, false otherwise</returns>
    public bool GetOrCreateTexture(string id, out PaintTexture texture)
    {
        if (!_textureMap.ContainsKey(id))
        {
            texture = CreateTexture();
            _textureMap.Add(id, texture);
            return false;
        }

        texture = _textureMap[id];
        return true;
    }

    private PaintTexture CreateTexture()
    {
        PaintTexture paintTexture = new()
        {
            Mask = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0, RenderTextureFormat.Default),
            Support = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0, RenderTextureFormat.Default),
        };
        
        return paintTexture;
    }
}
