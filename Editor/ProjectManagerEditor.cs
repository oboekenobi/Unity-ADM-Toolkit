#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(ProjectManager))]
public class ProjectManagerEditor : Editor
{
    private VisualElement root { get; set; }

    public VisualTreeAsset m_UXML;

    public static SerializedObject mangerSerializedObject;

    private ControlPanelEditor controlPanelEditor;
    private SerializedProperty controlPanelProperty;

    private void OnEnable()
    {
        /*controlPanelProperty = serializedObject.FindProperty("MasterControlPanel");
        controlPanelEditor = (ControlPanelEditor)Editor.CreateEditor(controlPanelProperty.objectReferenceValue);*/

        /*controlPanelProperty = serializedObject.FindProperty("MasterControlPanel");
        if (controlPanelProperty != null)
        {
            controlPanelEditor = (ControlPanelEditor)CreateEditor(controlPanelProperty.objectReferenceValue, typeof(ControlPanelEditor));
        }*/

    }

    public override VisualElement CreateInspectorGUI()
    {
        //FindProperties();
        InitializeEditor();
        //Compose();

        m_UXML.CloneTree(root);


        /*if (controlPanelEditor != null)
        {
            root.Add(controlPanelEditor.CreateInspectorGUI());
        }
        else
        {
            Debug.LogError("ControlPanel property not found. Make sure the property name is correct.");
        }*/

        //creates foldout menu of the entire default way the script is displayed in the inspector, minus the already composed propertyfield

        var foldout = new Foldout() { viewDataKey = "ProjectManagerEditor" };
        InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
        root.Add(foldout);

        mangerSerializedObject = serializedObject;
        return root;
    }

    private void InitializeEditor()
    {
        root = new VisualElement();
    }
    private void CinemachineComposerizeEditor()
    {
        // Create editor here
    }

    private void Compose()
    {
        //root.Add(SectionsPropertyField);

        // Compose editor here
        //composes the list of sections
    }
}


#endif