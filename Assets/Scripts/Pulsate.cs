using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate : MonoBehaviour
{
    private Material _material;
    private Light _light;
    private Color _baseColor;
    private bool _isGlowing = false;
    private float _glowLevel = 0;

    public float Max = 4;
    public float Min = 0;
    public float RateUp = 2f;
    public float RateDn = 2f;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _light = GetComponentInChildren<Light>();
        _baseColor = _material.color;
    }

    // Update is called once per frame
    void Update()
    {
        SetIsGlowing();
        SetColor();
        _light.intensity = _glowLevel;
    }

    private void SetIsGlowing()
    {
        if (_isGlowing)
        {
            if (_glowLevel >= Max)
            {
                _isGlowing = false;
            }
        }
        else
        {
            if (_glowLevel <= Min)
            {
                _isGlowing = true;
            }
        }
    }

    private void SetColor()
    {
        if (_isGlowing)
        {
            _glowLevel += RateUp * Time.deltaTime;
        }
        else
        {
            _glowLevel -= RateDn * Time.deltaTime;
        }

        _material.SetColor("_EmissionColor", _baseColor * _glowLevel);
    }
}
