using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeAway : MonoBehaviour
{
    private TMP_Text _text;
    public float FadeRate;

    void Start()
    {
        _text = GetComponent<TMP_Text>();
    }
    void Update()
    {
        float r = _text.color.r;
        float g = _text.color.g;
        float b = _text.color.g;
        float a = _text.color.a;
        a -= FadeRate;

        if (a <= 0)
            Disappear();
        else
        {
            Color newColor = new Color(r, g, b, a -= FadeRate);
            _text.color = newColor;
        }
    }
    private void Disappear()
    {
        _text.color = Color.white;
        gameObject.SetActive(false);
    }
}
