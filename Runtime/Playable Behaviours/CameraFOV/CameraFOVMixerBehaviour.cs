using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CameraFOVMixerBehaviour : PlayableBehaviour
{
    float m_DefaultFieldOfView;

    float m_AssignedFieldOfView;

    Camera m_TrackBinding;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Camera;

        if (m_TrackBinding == null)
            return;

        if (!Mathf.Approximately(m_TrackBinding.fieldOfView, m_AssignedFieldOfView))
            m_DefaultFieldOfView = m_TrackBinding.fieldOfView;

        int inputCount = playable.GetInputCount ();

        float blendedFieldOfView = 0f;
        float totalWeight = 0f;
        float greatestWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<CameraFOVBehaviour> inputPlayable = (ScriptPlayable<CameraFOVBehaviour>)playable.GetInput(i);
            CameraFOVBehaviour input = inputPlayable.GetBehaviour ();
            
            blendedFieldOfView += input.fieldOfView * inputWeight;
            totalWeight += inputWeight;

            if (inputWeight > greatestWeight)
            {
                greatestWeight = inputWeight;
            }
        }

        m_AssignedFieldOfView = blendedFieldOfView + m_DefaultFieldOfView * (1f - totalWeight);
        m_TrackBinding.fieldOfView = m_AssignedFieldOfView;
    }
}
