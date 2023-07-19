using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using itk.simple;

public class OutlineFadeTransition_ControlBehaviour : PlayableBehaviour
{
    public float intensity = 1f;
    public Color OutlineColor;
    public List<GameObject> OutlineObjects;
}
