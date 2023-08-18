using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class CalloutFadeAsset : PlayableBehaviour
{
    public ProjectManager projectManager;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float finalIntensity = 0f;

        int inputCount = playable.GetInputCount(); //get the number of all clips on this track

        for (int i = 0; i < inputCount; i++)
        {
            if(projectManager == null)
            {
                projectManager = GameObject.FindFirstObjectByType<ProjectManager>();
            }

            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<CalloutFadePlayableBehaviour> inputPlayable = (ScriptPlayable<CalloutFadePlayableBehaviour>)playable.GetInput(i);
            CalloutFadePlayableBehaviour input = inputPlayable.GetBehaviour();
            PlayableDirector director = playable.GetGraph().GetResolver() as PlayableDirector;

            if (director != projectManager.ActiveSection.director)
                return;

            // Use the above variables to process each frame of this playable.


            finalIntensity += input.intensity * inputWeight;

            VisualElement root = projectManager.ActiveSection.CalloutCanvasDocument.rootVisualElement;

            root.style.opacity = finalIntensity;
            
        }
    }
}
