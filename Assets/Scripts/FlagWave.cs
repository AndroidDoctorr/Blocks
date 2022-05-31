using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagWave : MonoBehaviour
{
    public GameObject[] Pieces;
    public float Phase;
    public float Frequency;
    public float Amplitude;

    void Start()
    {
        
    }

    void Update()
    {
        int i = 0;
        foreach (GameObject piece in Pieces)
        {
            piece.transform.localRotation = GetRotation(i++);
        }
    }
    private Quaternion GetRotation(int i)
    {
        float yRot = GetWaveAngle(i);
        Vector3 euler = new Vector3(0, yRot, 0);
        return Quaternion.Euler(euler);
    }
    private float GetWaveAngle(int i)
    {
        float A = i * 0.1f * Amplitude;
        float theta = -Frequency * (Time.time + i * Phase);
        return A * Mathf.Sin(theta);
    }
}
