using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AlphaFadeAsset : PlayableBehaviour
{
    public bool registered = false;
    public bool hasPlayed = false;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Material trackBinding = playerData as Material;
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

            if (inputWeight > 0 && inputWeight != 1)
            {
                if (!registered && !Effects_Manager.AlphaFadeMaterialsInQue)
                {
                    RegisterEffect(trackBinding);
                    registered = true;
                    Effects_Manager.AlphaFadeMaterialsInQue = true;
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
        }

        //assign the result to the bound object

        

        SetMaterialValue(trackBinding, finalIntensity);
        //trackBinding.SetFloat("_SmoothnessOffset", finalIntensity);
    }

    /*void SetMaterialValue(Material mat, float value)
    {
        foreach (Transform child in parent.transform)
        {
            Material renderer = child.gameObject.GetComponent<Material>();
            if(renderer != null)
            {
                renderer.SetFloat("_Alpha", value);
            }     
        }


    }*/

    public void RegisterEffect(Material material)
    {
        Effects_Manager.ActiveAlphaFadeObject.Add(material);
    }

    void SetMaterialValue(Material mat, float value)
    {
        mat.SetFloat("_Alpha", value);
    }
}
