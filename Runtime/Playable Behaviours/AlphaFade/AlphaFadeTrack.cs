using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AlphaFadeTrack : PlayableAsset
{
    public float intensity;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<AlphaPlayableBehaviour>.Create(graph);

        var lightControlBehaviour = playable.GetBehaviour();

        lightControlBehaviour.intensity = intensity;

        return playable;
    }
}
