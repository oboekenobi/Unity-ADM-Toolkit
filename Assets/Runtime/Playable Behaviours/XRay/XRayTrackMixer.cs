using ADM.UISystem;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class XRayTrackMixer : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.

    public bool registered = false;
    public bool hasPlayed = false;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Material trackBinding = playerData as Material;
        float finalIntensity = 0f;
        Color finalColor = Color.black;

        if (!trackBinding)
            return;
        
        int inputCount = playable.GetInputCount(); //get the number of all clips on this track

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<XRayControlBehaviour> inputPlayable = (ScriptPlayable<XRayControlBehaviour>)playable.GetInput(i);
            XRayControlBehaviour input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.
            finalIntensity += input.intensity * inputWeight;
            if(inputWeight > 0 && inputWeight != 1)
            {
                if (!registered && !Effects_Manager.XRayMaterialsInQue)
                {
                    RegisterEffect(trackBinding);
                    registered = true;
                    Effects_Manager.XRayMaterialsInQue = true;
                }
                if (registered && hasPlayed)
                {
                    registered = false;
                    hasPlayed = false;
                }
            }
            if (inputWeight == 1)
            {
                hasPlayed = true;
            }
            //finalColor += input.color * inputWeight;
        }

        //assign the result to the bound object
        trackBinding.SetFloat("_XRay", finalIntensity);
    }

    public void RegisterEffect(Material material)
    {
        Effects_Manager.ActiveXRayMaterials.Add(material);
    }
}
