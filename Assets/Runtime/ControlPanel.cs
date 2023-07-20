using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class ControlPanel
{
    // Start is called before the first frame update

    public VisualElement GeneratedContainer;
    public string ControlPanelTitle;

    [SerializeField]
    public List<MenuBinding> menuBindings;
}

[Serializable]
public class MenuBinding
{
    
    [Tooltip("The selected type.")]
    public Type selectedType;

    public GameObject GameObjectReference;

    [Tooltip("The presentation section script reference.")]
    public PresentationSection SectionReference;

    [Tooltip("The minimum value for the min max slider.")]
    public float minValue;

    [Tooltip("The maximum value for the min max slider.")]
    public float maxValue;

    public bool InitialBooleanValue;

    public string ControlTitle;

    // The enum for the selected type.
    public enum Type
    {
        Booleans,
        Visbility,
        SectionSwitch,
        Sliders,
        WorldSpaceLabels,
        Markers
    }
}
