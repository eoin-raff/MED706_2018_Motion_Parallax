using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

[ExecuteInEditMode]
public class Distortion : MonoBehaviour {

    public Material processingMaterial;
    [Range(0,5)]
    public float distortionPower = 0.1f;
    [Range(-5, 10)]
    public float _ZoomScale;
    [Range(-1, 1)]
    public float _ZoomOffset;
    Camera cam;
    public void OnEnable()
    {
        cam = GetComponent<Camera>();
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (processingMaterial != null)
        {
            processingMaterial.SetFloat("_BarrelPower", distortionPower);
            processingMaterial.SetFloat("_ZoomScale", _ZoomScale);
            processingMaterial.SetFloat("_ZoomOffset", _ZoomOffset);
            Graphics.Blit(source, destination, processingMaterial);
        }
    }
}
