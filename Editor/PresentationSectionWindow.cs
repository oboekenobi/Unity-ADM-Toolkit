#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using ADM.UISystem;



public class PresentationSectionWindow : EditorWindow
{
    Editor embeddedInspector;
    public PresentationSection currentSection;
    public PresentationSection lastSection;
    public bool canSelect;

    [MenuItem("Window/Section Inspector")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PresentationSectionWindow));
    }

    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PresentationSectionWindow window = (PresentationSectionWindow)EditorWindow.GetWindow(typeof(PresentationSectionWindow));
    }

    void OnGUI()
    {
        GameObject sel = Selection.activeGameObject;
        Debug.Log("SSS");

        if (sel != null)
        {
            currentSection = sel.GetComponent<PresentationSection>();
            
            if(currentSection is PresentationSection)
            {
                if (!canSelect)
                {
                    lastSection = currentSection;
                    canSelect = true;
                }
                else if (lastSection != currentSection)
                {
                    RecycleInspector(currentSection);

                    canSelect = false;
                }
            }
        }
    }

    //private VisualElement root { get; set; }
    public VisualTreeAsset m_UXML;

    public void CreateGUI()
    {
        
        
    }

    VisualElement root { get; set; }
    VisualElement UXMLRoot;
    void CreateInsepctor()
    {
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/CustomSectionEditor.uxml");

        UXMLRoot = visualTree.Instantiate();
        
        rootVisualElement.Add(UXMLRoot);
        
        if(embeddedInspector != null)
        {
            root.Bind(new SerializedObject(embeddedInspector.target));
        }
    }

    void RecycleInspector(Object target)
    {
        if (embeddedInspector != null) DestroyImmediate(embeddedInspector);
        if (UXMLRoot != null) UXMLRoot.Clear();
        embeddedInspector = Editor.CreateEditor(target);
        CreateInsepctor();
    }


    void OnDisable()
    {
        if (embeddedInspector != null) DestroyImmediate(embeddedInspector);
    }

}
#endif
