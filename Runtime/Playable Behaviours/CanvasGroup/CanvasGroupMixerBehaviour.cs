using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CanvasGroupMixerBehaviour : PlayableBehaviour
{
    float m_DefaultAlpha;

    float m_AssignedAlpha;

    CanvasGroup m_TrackBinding;

    public bool registered = false;
    public bool hasPlayed = false;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as CanvasGroup;

        if (m_TrackBinding == null)
            return;

        if (!Mathf.Approximately(m_TrackBinding.alpha, m_AssignedAlpha))
            m_DefaultAlpha = m_TrackBinding.alpha;

        int inputCount = playable.GetInputCount ();

        float blendedAlpha = 0f;
        float totalWeight = 0f;
        float greatestWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<CanvasGroupBehaviour> inputPlayable = (ScriptPlayable<CanvasGroupBehaviour>)playable.GetInput(i);
            CanvasGroupBehaviour input = inputPlayable.GetBehaviour ();
            
            blendedAlpha += input.alpha * inputWeight;
            totalWeight += inputWeight;

            if (inputWeight > greatestWeight)
            {
                greatestWeight = inputWeight;
            }



            if (inputWeight > 0 && inputWeight != 1)
            {
                if (!registered)
                {
                    RegisterEffect(m_TrackBinding);
                    registered = true;
                    Debug.Log("Effect Registered");
                    //MatFade_Transition.CanvasGroupInQue = true;
                }
                /*if (registered && hasPlayed)
                {
                    registered = false;
                    hasPlayed = false;
                }*/
            }
            /*if (inputWeight == 1)
            {
                hasPlayed = true;
            }*/
        }

        m_AssignedAlpha = blendedAlpha + m_DefaultAlpha * (1f - totalWeight);
        m_TrackBinding.alpha = m_AssignedAlpha;
    }


    public void RegisterEffect(CanvasGroup canvas)
    {
        Effects_Manager.CanvasGroups.Add(canvas);
        Effects_Manager.CanvasGroupTracks.Add(this);
    }
}
