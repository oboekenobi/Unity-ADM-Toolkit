using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

[CreateAssetMenu(fileName = "Presentation Slide")]
public class ExhibitPopup : ScriptableObject
{
    public float slideOutValue;
    public PresentationSlide[] slides;
}
[Serializable]
public class PresentationSlide
{
    public Sprite[] Images;
    public string ImageTitle;
    public string ExhibitName;
}
