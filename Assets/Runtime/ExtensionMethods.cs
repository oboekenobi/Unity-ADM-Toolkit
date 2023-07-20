using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ADM.UISystem;

[RequireComponent(typeof(Camera))]
public static class ExtensionMethods 
{
    public static float Remap(this float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }
    
    // Function to move camera to a UIElement position in "pixels" units
    public static void MoveCameraToUIPosition(this Camera camera, VisualElement uiElement, float zOffset)
    {
        // Get the world position of the UIElement
        Vector3 uiPosition = uiElement.LocalToWorld(new Vector3(uiElement.layout.x, -uiElement.layout.y, 0f));
        
        // Convert the UIElement position from pixels to world units
        uiPosition = Camera.main.ScreenToWorldPoint(new Vector3(uiPosition.x, uiPosition.y, Camera.main.nearClipPlane));
        
        // Calculate the offset position for the camera
        Vector3 offsetPosition = new Vector3(uiPosition.x, uiPosition.y, uiPosition.z + zOffset);
        
        // Move the camera to the offset position in local space
        camera.transform.localPosition = camera.transform.parent.InverseTransformPoint(offsetPosition);
    }
}