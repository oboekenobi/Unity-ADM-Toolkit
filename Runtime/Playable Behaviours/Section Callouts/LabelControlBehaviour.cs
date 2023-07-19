using ADM.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LabelControlBehaviour : PlayableBehaviour
{
    public bool registered;
    public bool hasPlayed;

  /*  float TagWidth = 0;
    float TagHeight = 0;*/
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        float finalIntensity = 0f;
        PresentationSection trackBinding = playerData as PresentationSection;
        float TagWidth = 0f;
        float TagHeight = 0f;

        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount(); //get the number of all clips on this track

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<LabelBehaviour> inputPlayable = (ScriptPlayable<LabelBehaviour>)playable.GetInput(i);
            LabelBehaviour input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.

            if (inputWeight > 0 && inputWeight != 1)
            {
                if (!registered)
                {
                    RegisterEffect(trackBinding);
                    registered = true;
                }
                else if (hasPlayed)
                {
                    registered = false;
                    hasPlayed = false;
                }
            }
            if (inputWeight == 1)
            {
                hasPlayed = true;
            }

            finalIntensity += input.intensity * inputWeight;
        }

        if (!Effects_Manager.releaseCallOuts)
        {
            foreach (CallOutLabel label in trackBinding.CallOuts)
            {
                float halfFinal = Mathf.Clamp(finalIntensity * 2, 0, 1f);
                float final = ((Mathf.Clamp(finalIntensity, 0.5f, 1f)) - 0.5f) * 2;
                label.ChildTag.localScale = new Vector3(label.ChildTag.localScale.y * final, label.ChildTag.localScale.y, label.ChildTag.localScale.z);

                TagWidth = (label.Fitter.glowBackground.rect.width * label.transform.localScale.y / 2);
                TagHeight = (label.Fitter.glowBackground.rect.height * label.transform.localScale.y / 2);

                for (int j = 0; j < label.UILines.Count; j++)
                {
                    label.UILines[j].line.option.endRatio = halfFinal;
                    label.UILines[j].GeometyUpdateFlagUp();
                }

                for(int j =0; j < label.CircleMaterials.Count; j++)
                {
                    label.CircleMaterials[j].SetFloat("_Alpha", halfFinal);
                }
                label.ParentGroup.alpha = final;


                //float TagWidth = label.TagWidth;
                if (label.LinePlacement == CallOutLabel.PlacementDirection.Left)
                {
                    Vector3 Offset = Vector3.left * TagWidth;
                    label.ChildTag.localPosition = Offset * (1-final);
                }

                if (label.LinePlacement == CallOutLabel.PlacementDirection.Right)
                {
                    Vector3 Offset = Vector3.right * TagWidth;
                    label.ChildTag.localPosition = Offset * (1 - final);
                }
                if (label.LinePlacement == CallOutLabel.PlacementDirection.Bottom)
                {
                    Vector3 Offset = Vector3.down * TagHeight;
                    label.ChildTag.localPosition = Offset * (1 - final);
                }
                if (label.LinePlacement == CallOutLabel.PlacementDirection.Top)
                {
                    Vector3 Offset = Vector3.up * TagHeight;
                    label.ChildTag.localPosition = Offset * (1 - final);
                }
                if (label.LinePlacement == CallOutLabel.PlacementDirection.Middle)
                {
                    Vector3 Offset = Vector3.down * TagHeight;
                    label.ChildTag.localPosition = Offset * (1 - final);
                }

            }
        }
        

    }
    public void RegisterEffect(PresentationSection section)
    {
        for (int i = 0; i < section.CallOutGameObjects.Count; i++)
        {
            section.CallOutGameObjects[i].SetActive(true);
        }
    }
}
