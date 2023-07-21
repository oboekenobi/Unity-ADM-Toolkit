using ADM.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ADMUIMenuBinding : MonoBehaviour
{
    public List<ButtonBindings> Bindings = new List<ButtonBindings>();
    public string MenuBinding;
    public VisualElement uIRoot;
    public VisualElement MenuElement;
    public CanvasGroup ContrastLayer;
    public Transform Slicer;
    public float VerticalSlicerMin;
    public float VerticalSlicerMax;
    public float HorizontalSlicerMax;
    public float HorizontalSlicerMin;
    private Slider HorizontalSlider;
    private Slider VerticalSlider;
    public List<Material> FadeMaterials = new List<Material>();

    public Menu MenuType;
    public enum Menu {ContrastFade, DICOMViewer };
    public bool ContrastFade;
    [Serializable]
    public struct ButtonBindings
    {
        public string ButtonBinding;
        public GameObject HighlightObject;
        public List<Material> HiddenMaterials;
        public List<Material> RevealMaterials;
        public List<GameObject> HiddenDitherMasks;
        public Color outlineColor;
        public float outlineThickeness;
    }

    public void Awake()
    {
        Shader.SetGlobalFloat("_DitherAlpha", 0f);
    }
    public void BindAllButtons()
    {
        if(MenuType == Menu.ContrastFade)
        {
            foreach (var b in Bindings)
            {
                MenuElement.Q<VisualElement>(b.ButtonBinding).RegisterCallback<MouseEnterEvent>(ev => ShowHighlight(b));
                MenuElement.Q<VisualElement>(b.ButtonBinding).RegisterCallback<MouseLeaveEvent>(ev => unShowHighlight(b));
            }
        }
        if(MenuType == Menu.DICOMViewer)
        {
            VerticalSlider = MenuElement.Q<Slider>("VerticalDICOMSlider");
            HorizontalSlider = MenuElement.Q<Slider>("HorizontalDICOMSlider");
            VerticalSlider.RegisterValueChangedCallback(ev => VerticalDICOMSlide(VerticalSlider.value));
            HorizontalSlider.RegisterValueChangedCallback(ev => HorizontalDICOMSlide(HorizontalSlider.value));
        }
    }
    public void ShowHighlight(ButtonBindings binding)
    {
        //binding.HighlightObject.SetActive(true);
        binding.HighlightObject.layer = LayerMask.NameToLayer("UI");
        Shader.SetGlobalFloat("_DitherAlpha" , 1f); 
        Shader.SetGlobalFloat("Thickness", binding.outlineThickeness);
        Shader.SetGlobalColor("OutlineColor", binding.outlineColor);

        if (binding.HiddenMaterials != null)
        {
            foreach(Material mat in binding.HiddenMaterials)
            {
                mat.SetFloat("_Alpha", 0.5f);
                mat.SetFloat("_XRay", 0.1f);
                mat.SetFloat("_Intensity", 0.6f);
            }
            //Shader.SetGlobalFloat("_DitherAlpha", 0f);
        }
        if (binding.RevealMaterials != null)
        {
            foreach (Material mat in binding.RevealMaterials)
            {

                mat.SetFloat("_Alpha", 1f);
                mat.SetFloat("_XRay", 1);
            }
        }
        if(binding.HiddenDitherMasks != null)
        {
            foreach(GameObject go in binding.HiddenDitherMasks)
            {
                //go.SetActive(false);
                go.layer = LayerMask.NameToLayer("Default");
            }
        }
        //StartCoroutine(ContrastFade());
    }

    public void unShowHighlight(ButtonBindings binding)
    {
        binding.HighlightObject.layer = LayerMask.NameToLayer("DitherMask");
        Shader.SetGlobalFloat("Thickness", 0);
        Shader.SetGlobalFloat("_DitherAlpha", 0f);
        if (binding.HiddenMaterials != null)
        {
            foreach (Material mat in binding.HiddenMaterials)
            {

                mat.SetFloat("_Alpha", 1f);
                mat.SetFloat("_XRay", 0.6f);
                mat.SetFloat("_Intensity", 1f);

            }
        }
        if (binding.RevealMaterials != null)
        {
            foreach (Material mat in binding.RevealMaterials)
            {

                mat.SetFloat("_Alpha", 1f);
                mat.SetFloat("_XRay", 0.6f);
                mat.SetFloat("_Intensity", 1f);
            }
        }
        if (binding.HiddenDitherMasks != null)
        {
            foreach (GameObject go in binding.HiddenDitherMasks)
            {
                go.layer = LayerMask.NameToLayer("DitherMask");
                
            }
        }
    }

    public void VerticalDICOMSlide(float SliderValue)
    {
        Slicer.position = new Vector3(Slicer.position.x, VerticalSlicerMax + (VerticalSlicerMin * SliderValue), Slicer.position.z);
        Debug.Log("Slider Sliding");
    }

    public void HorizontalDICOMSlide(float SliderValue)
    {
        Slicer.position = new Vector3(HorizontalSlicerMin + (HorizontalSlicerMax - (HorizontalSlicerMin * SliderValue)), Slicer.position.y, Slicer.position.z);
    }
}
