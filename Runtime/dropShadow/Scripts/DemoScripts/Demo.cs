using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour {

    public Material dropShadowMaterial;

    public Slider distanceSlider;
    public Slider alphaSlider;
    public Slider offsetXSlider;
    public Slider offsetYSlider;
    public Toggle longShadowToggle;
    public Slider longShadowAngleSlider;
//    public Slider qualitySlider;

	
	// Update is called once per frame
	void Update () 
    {
        dropShadowMaterial.SetFloat("_distance",distanceSlider.value);
        dropShadowMaterial.SetColor("_color",new Color(0f,0f,0f,alphaSlider.value));
        dropShadowMaterial.SetFloat("_offsetX",offsetXSlider.value);
        dropShadowMaterial.SetFloat("_offsetY",offsetYSlider.value);

        float i = (longShadowToggle.isOn)?1f:0f;
        dropShadowMaterial.SetFloat("_longShadow",i);

        dropShadowMaterial.SetFloat("_longShadowAngle",longShadowAngleSlider.value);

        longShadowAngleSlider.interactable = longShadowToggle.isOn;

//        dropShadowMaterial.SetInt("_quality",(int)qualitySlider.value);
        	
	}
}
