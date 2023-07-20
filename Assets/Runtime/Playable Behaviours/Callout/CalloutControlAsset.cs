using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CalloutControlAsset : PlayableAsset
{
    public float intensity;
    //public ExposedReference<Material> Shader;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CalloutControlBehaviour>.Create(graph);

        var lightControlBehaviour = playable.GetBehaviour();

        lightControlBehaviour.intensity = intensity;

        return playable;
    }
}
