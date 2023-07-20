using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;


public class TransitionResetPlayable : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        GameObject trackBinding = playerData as GameObject;
        float finalIntensity = 0f;


        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount(); //get the number of all clips on this track

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<AlphaPlayableBehaviour> inputPlayable = (ScriptPlayable<AlphaPlayableBehaviour>)playable.GetInput(i);
            AlphaPlayableBehaviour input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.

            finalIntensity += input.intensity * inputWeight;
        }

        //assign the result to the bound object

        //SetMaterialValue(trackBinding, finalIntensity);

    }
}
