using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CanvasGroupClip : PlayableAsset, ITimelineClipAsset
{
    public CanvasGroupBehaviour template = new CanvasGroupBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CanvasGroupBehaviour>.Create (graph, template);
        return playable;
    }
}
