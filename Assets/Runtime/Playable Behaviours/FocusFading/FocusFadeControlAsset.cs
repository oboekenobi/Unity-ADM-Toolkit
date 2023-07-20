using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FocusFadeControlAsset : PlayableAsset, ITimelineClipAsset
{
    public float intensity;
    public bool Extrapolation;
    public ClipCaps clipCaps
    {
        get
        {
            return ClipCaps.ClipIn | ClipCaps.Extrapolation | ClipCaps.Looping | ClipCaps.Blending;
        }
    }
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<FocusFadeControlBehaviour>.Create(graph);

        var lightControlBehaviour = playable.GetBehaviour();

        lightControlBehaviour.intensity = intensity;

        return playable;
    }
}
