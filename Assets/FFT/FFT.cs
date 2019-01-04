using System;
using UnityEngine;

public class FFT : MonoBehaviour
{
    public ComputeShader _shader;
    private FFTComputeHelper _helper;

    public Texture _source;
    private RenderTexture _finalForwardMagnitude;
    private RenderTexture _finalForwardPhase;
    private RenderTexture _finalInverse;

    public Renderer _sourceRenderer;
    public Renderer _finalForwardMagnitudeRenderer;
    public Renderer _finalForwardPhaseRenderer;
    public Renderer _finalInverseRenderer;

    #region Unity Methods
    private void Awake()
    {
        if (!IsInputValid())
        {
            return;
        }

        InitRenderTextures();
        InitRenderers();

        _helper = new FFTComputeHelper(_shader);
    }

    private void Start()
    {
        _helper.Init(_source.width, _source.height);
    }

    private void Update()
    {
        _helper.Load(_source);
        _helper.RecenterData();

        _helper.Forward();
        _helper.GetMagnitudeSpectrumScaled(_finalForwardMagnitude);
        _helper.GetPhaseAngle(_finalForwardPhase);

        _helper.Inverse();
        _helper.GetMagnitudeSpectrum(_finalInverse);
    }

    private void OnDestroy()
    {
        _helper.Release();
        DestroyRenderTextures();
    }
    #endregion

    #region Utility Methods
    static RenderTexture CreateRenderTexture(int width, int height)
    {
        RenderTexture tex = new RenderTexture(width, height, 24);
        tex.useMipMap = false;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.enableRandomWrite = true;
        tex.Create();
        return tex;
    }

    private bool IsInputValid()
    {
        if (_shader == null)
        {
            return false;
        }

        if (FFTComputeHelper.RoundUpPowerOf2((uint)_source.width) != (uint)_source.width ||
            FFTComputeHelper.RoundUpPowerOf2((uint)_source.height) != (uint)_source.height)
        {
            return false;
        }

        if (_sourceRenderer == null ||
            _finalForwardMagnitudeRenderer == null ||
            _finalInverseRenderer == null)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region Init Methods
    private void InitRenderers()
    {
        _sourceRenderer.material.mainTexture = _source;
        _finalForwardMagnitudeRenderer.material.mainTexture = _finalForwardMagnitude;
        _finalForwardPhaseRenderer.material.mainTexture = _finalForwardPhase;
        _finalInverseRenderer.material.mainTexture = _finalInverse;
    }

    private void InitRenderTextures()
    {
        _finalForwardMagnitude = CreateRenderTexture(_source.width, _source.height);
        _finalForwardPhase = CreateRenderTexture(_source.width, _source.height);
        _finalInverse = CreateRenderTexture(_source.width, _source.height);
    }
    #endregion

    #region Clean Up Methods
    private void DestroyRenderTextures()
    {
        _finalForwardMagnitude.Release();
        _finalInverse.Release();
    }
    #endregion
}
