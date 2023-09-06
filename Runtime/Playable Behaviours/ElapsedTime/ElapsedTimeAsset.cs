using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class ElapsedTimeAsset : PlayableBehaviour
{
    public ProjectManager projectManager;
    public Label Timer;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        float finalTime = 0f;

        int inputCount = playable.GetInputCount(); //get the number of all clips on this track

        if (projectManager == null)
        {
            projectManager = GameObject.FindFirstObjectByType<ProjectManager>();
        }

        float time = info.deltaTime;

        for (int i = 0; i < inputCount; i++)
        {

            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<ElapsedTimePlayableBehaviour> inputPlayable = (ScriptPlayable<ElapsedTimePlayableBehaviour>)playable.GetInput(i);
            ElapsedTimePlayableBehaviour input = inputPlayable.GetBehaviour();
            PlayableDirector director = playable.GetGraph().GetResolver() as PlayableDirector;

            try
            {
                Timer = projectManager.uI_Manager.uIDocument.rootVisualElement.Q<Label>(input.TimerBinding);
            }
            catch
            {
                Debug.LogWarning("The timer on " + projectManager.ActiveSection.SectionTitle + " cannot find the visual element");
            }

            Timer.text = ConvertToClockTime((float)director.time, input.Time);
            /*if (Timer == null)
            {
                try
                {
                    projectManager.uI_Manager.uIDocument.rootVisualElement.Q<Label>(input.TimerBinding);
                    Timer.text = finalTime.ToString();
                }
                catch
                {
                    Debug.LogWarning("The timer on " + projectManager.ActiveSection.SectionTitle + " cannot find the visual element");
                }

            }*/


            //finalTime += input.Time * inputWeight;



            /*if (Timer != null)
            {
                Timer.text = finalTime.ToString();
            }*/

        }
    }
    public string ConvertToClockTime(float seconds, float offset)
    {
        float time = seconds + offset;  
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int remainingSeconds = Mathf.FloorToInt(time % 60);

        string clockTime = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, remainingSeconds);
        return clockTime;
    }
}
