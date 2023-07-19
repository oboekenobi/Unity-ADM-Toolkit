using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LabelTrack : PlayableAsset
{
    public float intensity;
    //public ExposedReference<Material> Shader;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<LabelBehaviour>.Create(graph);

        var lightControlBehaviour = playable.GetBehaviour();

        //lightControlBehaviour.ADMShader = Shader.Resolve(graph.GetResolver());
        //lightControlBehaviour.ADMShader.SetFloat("_XRay", intensity);

        lightControlBehaviour.intensity = intensity;

        return playable;
    }
}
