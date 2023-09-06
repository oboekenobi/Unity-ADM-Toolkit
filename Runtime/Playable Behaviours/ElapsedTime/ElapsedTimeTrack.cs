using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Playables;

public class ElapsedTimeTrack : PlayableAsset
{
    public float Time;
    public string TimerBinding;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ElapsedTimePlayableBehaviour>.Create(graph);

        var elapsedTimeControlBehaviour = playable.GetBehaviour();

        elapsedTimeControlBehaviour.Time = Time;
        elapsedTimeControlBehaviour.TimerBinding = TimerBinding;

        return playable;
    }
}
