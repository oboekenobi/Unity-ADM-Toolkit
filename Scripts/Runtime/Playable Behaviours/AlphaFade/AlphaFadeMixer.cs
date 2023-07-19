using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(AlphaFadeTrack))]
[TrackBindingType(typeof(Material))]
public class AlphaFadeMixer : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<AlphaFadeAsset>.Create(graph, inputCount);
    }
}
