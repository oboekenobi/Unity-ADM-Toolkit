using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

[TrackColor(0.04125132f, 0.5064747f, 0.9716981f)]
[TrackClipType(typeof(CameraFOVClip))]
[TrackBindingType(typeof(Camera))]
public class CameraFOVTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CameraFOVMixerBehaviour>.Create (graph, inputCount);
    }

    // Please note this assumes only one component of type Camera on the same gameobject.
    public override void GatherProperties (PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        Camera trackBinding = director.GetGenericBinding(this) as Camera;
        if (trackBinding == null)
            return;

        // These field names are procedurally generated estimations based on the associated property names.
        // If any of the names are incorrect you will get a DrivenPropertyManager error saying it has failed to register the name.
        // In this case you will need to find the correct backing field name.
        // The suggested way of finding the field name is to:
        // 1. Make sure your scene is serialized to text.
        // 2. Search the text for the track binding component type.
        // 3. Look through the field names until you see one that looks correct.
        driver.AddFromName<Camera>(trackBinding.gameObject, "m_FieldOfView");
#endif
        base.GatherProperties (director, driver);
    }
}
