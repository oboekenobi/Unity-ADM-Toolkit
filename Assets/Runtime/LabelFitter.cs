using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LabelFitter : MonoBehaviour
{

    // Start is called before the first frame update
    public ContentSizeFitter fitter;
    public HorizontalLayoutGroup horizontalLayoutGroup;
    public RectTransform Background;
    public RectTransform glowBackground;
    public Image BackgroundImage;
    public Image glowBackgroundImage;

    /*[Range(1, 100)]
    public int TopPadding;
    [Range(1, 100)]
    public int SidePadding;*/
    [Range(1, 100)]
    public float Padding;
    [Range(0, 4)]
    public float GlowPadding;

    void Start()
    {
        
    }
#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            //Debug.Log(glowBackground.rect.width);
            glowBackgroundImage.pixelsPerUnitMultiplier = BackgroundImage.pixelsPerUnitMultiplier;

            horizontalLayoutGroup.padding.left = -(int)Padding;
            horizontalLayoutGroup.padding.right = -(int)Padding;
            glowBackground.offsetMax = new Vector2(Padding * GlowPadding, Padding * GlowPadding);
            glowBackground.offsetMin = new Vector2(-Padding * GlowPadding, -Padding * GlowPadding);

            glowBackground.anchorMax = Vector2.one;
            glowBackground.anchorMin = Vector2.zero;
            Background.sizeDelta = new Vector2(Background.sizeDelta.x, (30 + Padding));
        }
    }
#endif
}
