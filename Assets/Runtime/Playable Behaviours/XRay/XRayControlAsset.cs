using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Playables;
public class XRayControlAsset : PlayableAsset
{
    public float intensity;
    //public ExposedReference<Material> Shader;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<XRayControlBehaviour>.Create(graph);
        
        var lightControlBehaviour = playable.GetBehaviour();
        
        lightControlBehaviour.intensity = intensity;

        return playable;
    }
}
