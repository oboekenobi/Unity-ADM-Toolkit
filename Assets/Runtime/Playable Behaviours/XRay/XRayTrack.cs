using ADM.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(XRayControlAsset))]
[TrackBindingType(typeof(Material))]
public class XRayTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<XRayTrackMixer>.Create(graph, inputCount);
    }
}
