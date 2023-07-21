using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class dropShadow : MonoBehaviour {

    [HideInInspector]

    [SerializeField]
    private Camera _camera;
    public Camera Overlay;
    public int _downResFactor = 0;
    public bool Init;
    public RenderTexture cameraTexture;

    private string _globalTextureName = "_GlobalShadowTex";

    void Start()
    {
        OnEnable();
        cameraTexture = _camera.targetTexture;
    }

    void OnEnable()
    {
        GenerateRT();
    }
    private void Update()
    {
        cameraTexture.Release();
        cameraTexture.width = Screen.width; 
        cameraTexture.height = Screen.height;
    }

    

    void GenerateRT()
    {
        _camera = GetComponent<Camera>();

        if (_camera.targetTexture != null)
        {
            RenderTexture temp = _camera.targetTexture;

            _camera.targetTexture = null;
            DestroyImmediate(temp);
        }


        _camera.targetTexture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 16);
        _camera.targetTexture.filterMode = FilterMode.Bilinear;

        cameraTexture = _camera.targetTexture;

        Shader.SetGlobalTexture(_globalTextureName, _camera.targetTexture);
    }
}
