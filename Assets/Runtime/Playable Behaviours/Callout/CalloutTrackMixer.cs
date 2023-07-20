using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class CalloutTrackMixer : PlayableBehaviour
{
    public bool registered;
    public bool hasPlayed;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        CallOutLabel trackBinding = playerData as CallOutLabel;
        float finalIntensity = 0f;
        float TagWidth = 0f;
        float TagHeight = 0f;

        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount(); //get the number of all clips on this track

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<CalloutControlBehaviour> inputPlayable = (ScriptPlayable<CalloutControlBehaviour>)playable.GetInput(i);
            CalloutControlBehaviour input = inputPlayable.GetBehaviour();

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
            // Use the above variables to process each frame of this playable.

            finalIntensity += input.intensity * inputWeight;
        }

        //assign the result to the bound object
        float halfFinal = Mathf.Clamp(finalIntensity * 2, 0, 1f);
        float final = ((Mathf.Clamp(finalIntensity, 0.5f, 1f)) - 0.5f) * 2;
        trackBinding.ChildTag.localScale = new Vector3(trackBinding.ChildTag.localScale.y * final, trackBinding.ChildTag.localScale.y, trackBinding.ChildTag.localScale.z);

        TagWidth = (trackBinding.Fitter.glowBackground.rect.width * trackBinding.transform.localScale.y / 2);
        TagHeight = (trackBinding.Fitter.glowBackground.rect.height * trackBinding.transform.localScale.y / 2);

        for (int j = 0; j < trackBinding.UILines.Count; j++)
        {
            trackBinding.UILines[j].line.option.endRatio = halfFinal;
            //trackBinding.UILines[j].GeometyUpdateFlagUp();
        }

        for (int j = 0; j < trackBinding.CircleMaterials.Count; j++)
        {
            trackBinding.CircleMaterials[j].SetFloat("_Alpha", halfFinal);
        }
        trackBinding.ParentGroup.alpha = final;


        //float TagWidth = label.TagWidth;
        if (trackBinding.LinePlacement == CallOutLabel.PlacementDirection.Left)
        {
            Vector3 Offset = Vector3.left * TagWidth;
            trackBinding.ChildTag.localPosition = Offset * (1 - final);
        }

        if (trackBinding.LinePlacement == CallOutLabel.PlacementDirection.Right)
        {
            Vector3 Offset = Vector3.right * TagWidth;
            trackBinding.ChildTag.localPosition = Offset * (1 - final);
        }
        if (trackBinding.LinePlacement == CallOutLabel.PlacementDirection.Bottom)
        {
            Vector3 Offset = Vector3.down * TagHeight;
            trackBinding.ChildTag.localPosition = Offset * (1 - final);
        }
        if (trackBinding.LinePlacement == CallOutLabel.PlacementDirection.Top)
        {
            Vector3 Offset = Vector3.up * TagHeight;
            trackBinding.ChildTag.localPosition = Offset * (1 - final);
        }
        if (trackBinding.LinePlacement == CallOutLabel.PlacementDirection.Middle)
        {
            Vector3 Offset = Vector3.down * TagHeight;
            trackBinding.ChildTag.localPosition = Offset * (1 - final);
        }

    }

    public void RegisterEffect(CallOutLabel label)
    {
        label.LabelParent.SetActive(true);
    }
}
