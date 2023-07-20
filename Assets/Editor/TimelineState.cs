using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

public class TimelineState : MonoBehaviour
{
    public static readonly Type timelineWindow =
            Type.GetType("UnityEditor.Timeline.TimelineWindow, Unity.Timeline.Editor");


    public static void SetLockStatus(bool newState)
    {
        if(TimelineEditor.GetWindow() != null)
        {
            var window = TimelineEditor.GetWindow();
            PropertyInfo propertyInfo = window.GetType().GetProperty("locked");
            propertyInfo.SetValue(window, newState, null);
            window.Repaint();
        }
    }

}
