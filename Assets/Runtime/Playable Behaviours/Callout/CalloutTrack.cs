using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(CalloutControlAsset))]
[TrackBindingType(typeof(CallOutLabel))]

public class CalloutTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CalloutTrackMixer>.Create(graph, inputCount);
    }
}
