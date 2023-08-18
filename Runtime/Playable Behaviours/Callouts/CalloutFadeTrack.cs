using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CalloutFadeTrack : PlayableAsset
{
    public float intensity;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CalloutFadePlayableBehaviour>.Create(graph);

        var calloutControlBehaviour = playable.GetBehaviour();

        calloutControlBehaviour.intensity = intensity;

        return playable;
    }
}
