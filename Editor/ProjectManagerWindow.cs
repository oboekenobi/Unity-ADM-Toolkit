using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ProjectManagerWindow : EditorWindow
{
    Editor embeddedInspector;
    ProjectManager currentSection;

    [MenuItem("Window/Project Manager")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ProjectManagerWindow));
    }

    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ProjectManagerWindow window = (ProjectManagerWindow)EditorWindow.GetWindow(typeof(ProjectManagerWindow));
    }

    void OnGUI()
    {

       
    }

    //private VisualElement root { get; set; }
    public VisualTreeAsset m_UXML;

    public void CreateGUI()
    {
        CreateInsepctor();
    }

    VisualElement root { get; set; }
    [SerializeField]
    VisualElement UXMLRoot;
    void CreateInsepctor()
    {
        currentSection = FindFirstObjectByType<ProjectManager>();
        //currentSection = FindFirstObjectOfType<ProjectManager>().GetComponent<ProjectManager>();
        if (currentSection != null)
        {
            RecycleInspector(currentSection);
        }

        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Packages/ADM Toolkit/UI/UXML Icons/CustomManager.uxml");

        UXMLRoot = visualTree.Instantiate();

        rootVisualElement.Add(UXMLRoot);
        
        if (embeddedInspector != null)
        {
            root.Bind(new SerializedObject(embeddedInspector.target));
        }
    }

    void RecycleInspector(Object target)
    {
        if (embeddedInspector != null) DestroyImmediate(embeddedInspector);
        if (UXMLRoot != null) UXMLRoot.Clear();
        embeddedInspector = Editor.CreateEditor(target);
        //CreateInsepctor();
    }


    void OnDisable()
    {
        if (embeddedInspector != null) DestroyImmediate(embeddedInspector);
    }
}
