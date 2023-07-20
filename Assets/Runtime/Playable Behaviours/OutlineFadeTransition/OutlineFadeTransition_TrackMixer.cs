using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OutlineFadeTransition_TrackMixer : PlayableBehaviour
{
    public bool registered = false;
    public bool hasPlayed = false;
    GameObject trackBinding;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        trackBinding = playerData as GameObject;
        float finalIntensity = 0f;
        Color finalColor = Color.black;

        /*if (!trackBinding)
            return;*/

        int inputCount = playable.GetInputCount(); //get the number of all clips on this track

     
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<OutlineFadeTransition_ControlBehaviour> inputPlayable = (ScriptPlayable<OutlineFadeTransition_ControlBehaviour>)playable.GetInput(i);
            OutlineFadeTransition_ControlBehaviour input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.
            
            finalIntensity += input.intensity * inputWeight;
            finalColor += input.OutlineColor * inputWeight;
        }

        //assign the result to the bound object


        if(trackBinding != null)
        {
            if (finalIntensity > 0 && !registered)
            {
                RegisterEffect(trackBinding);
                Debug.Log("Active");
                registered = true;
                //RegisterEffect(trackBinding);
            }
            if (finalIntensity > 0)
            {
                Shader.SetGlobalFloat("Thickness", (0.014f * finalIntensity));
                Shader.SetGlobalColor("OutlineColor", finalColor);
            }
            if (finalIntensity == 0 && registered)
            {
                UnRegisterEffect(trackBinding);
                Debug.Log("Inactive");
                registered = false;
            }
        }
    }

    public void RegisterEffect(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("Outline");
        //MatFade_Transition.ActiveXRayMaterials.Add(material);
    }

    public void UnRegisterEffect(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("Default");
        //MatFade_Transition.ActiveXRayMaterials.Add(material);
    }
}
