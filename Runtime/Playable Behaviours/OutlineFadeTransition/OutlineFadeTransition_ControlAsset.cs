using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OutlineFadeTransition_ControlAsset : PlayableAsset
{
    public float intensity;
    public Color OutlineColor;
    //public ExposedReference<Material> Shader;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<OutlineFadeTransition_ControlBehaviour>.Create(graph);

        var lightControlBehaviour = playable.GetBehaviour();

        //lightControlBehaviour.ADMShader = Shader.Resolve(graph.GetResolver());
        //lightControlBehaviour.ADMShader.SetFloat("_XRay", intensity);

        lightControlBehaviour.intensity = intensity;
        lightControlBehaviour.OutlineColor = OutlineColor;

        return playable;
    }
}
