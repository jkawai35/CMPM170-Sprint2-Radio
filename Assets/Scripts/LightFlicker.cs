using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    public Light2D lightObject;
    public float dimmingRate;
    private float dimmingDirection = 1;
    public float maxIntensity;
    public float minIntensity;
    public float setIntensity;

    private void Start()
    {
        lightObject.intensity = setIntensity;
    }

    private void Update()
    {
        lightObject.intensity += dimmingRate * dimmingDirection * Time.deltaTime;
        if (lightObject.intensity > maxIntensity || lightObject.intensity < minIntensity) {
            dimmingDirection *= -1;
        }
    }
}