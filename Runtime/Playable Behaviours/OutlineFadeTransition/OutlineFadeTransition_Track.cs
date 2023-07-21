using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackClipType(typeof(OutlineFadeTransition_ControlAsset))]
[TrackBindingType(typeof(GameObject))]
public class OutlineFadeTransition_Track : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<OutlineFadeTransition_TrackMixer>.Create(graph, inputCount);
    }
}
