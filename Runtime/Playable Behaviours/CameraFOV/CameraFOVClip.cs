using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraFOVClip : PlayableAsset, ITimelineClipAsset
{
    public CameraFOVBehaviour template = new CameraFOVBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CameraFOVBehaviour>.Create (graph, template);
        return playable;
    }
}
