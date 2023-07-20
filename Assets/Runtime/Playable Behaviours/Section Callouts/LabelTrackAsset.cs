using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackClipType(typeof(LabelTrack))]
[TrackBindingType(typeof(PresentationSection))]
public class LabelTrackAsset : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<LabelControlBehaviour>.Create(graph, inputCount);
    }
}
