using ADM.UISystem;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public class FocusFadeTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float finalIntensity = 0f;
        
        int inputCount = playable.GetInputCount(); //get the number of all clips on this track

        //Debug.Log(playable);


        
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            
            ScriptPlayable<FocusFadeControlBehaviour> inputPlayable = (ScriptPlayable<FocusFadeControlBehaviour>)playable.GetInput(i);
            FocusFadeControlBehaviour input = inputPlayable.GetBehaviour();
            
            finalIntensity += input.intensity * inputWeight;
        }
        Shader.SetGlobalFloat("_AlphaOffset", finalIntensity);

    }
/*
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // Execute your starting logic here, calling into a singleton for example
        Debug.Log("Clip started!");
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // Only execute in Play mode

        var duration = playable.GetDuration();
        var time = playable.GetTime();
        var count = time + info.deltaTime;

        if ((info.effectivePlayState == PlayState.Paused && count > duration) || Mathf.Approximately((float)time, (float)duration))
        {
            // Execute your finishing logic here:
            Debug.Log("Clip done!");
        }
        return;

    }*/
}
