using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using ADM.UISystem;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine.PostFX;
using UnityEngine.UIElements;
using static ADM.UISystem.PopupManager;
using TMPro;
using System.Reflection;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class PresentationSection : MonoBehaviour
{
    public Transform TestLinePoint;
    public CalloutManager line;
    public CalloutManager testLineDrawer;
    VisualElement m_lineDrawer;
    
    public UIDocument CalloutCanvasDocument;
    

    [HideInInspector]
    public ProjectManager manager;
    public UI_Manager uI_Manager;

    [HideInInspector]
    public Transform Pivot;
    public PlayableDirector director;
    public TimelineAsset TimelineInstance;
    public VisualTreeAsset CalloutCanvasInstance;
    public string calloutAssetPath;
    public string timelineAssetPath;

    public bool playableInit;
    public bool SubSection;
    public bool hideTimeline;
    public bool showCompass;
    public CinemachineVirtualCamera PreviousCamera;
    public CinemachineVirtualCamera VirtualCamera;
    public CinemachineVolumeSettings VolumeSettings;
    public CanvasGroup WorldCallouts;

    public int MouseSensitivityMultiplier = 1;
    public float LabelEditorValue;
    public float ZoomSpeedOffset = 0;
    [Tooltip("Include a pop up for Images")]
    public ExhibitPopup PopupWindowContent;
    public ADMUIMenuBinding CustomMenu;

    public VisualElement GeneratedPopup;

    public List<GameObject> ToggleObjects;
    public List<MarkerScript> SectionMarkers = new List<MarkerScript>();
    public List<CalloutBinding> CalloutBindings = new List<CalloutBinding>();
    [Tooltip ("Include the names of visual elements in the root UI Document to reveal once this section is active")]
    public List<String> VisualElementsToShow = new List<String>();

    [SerializeField]
    public enum ControlPanleOverrideType
    {
        ReplaceControls, AddControls
    }

    public enum CameraManipulator
    {
        Tethered, POV, Static
    }

    public ControlPanleOverrideType OverrideType = ControlPanleOverrideType.ReplaceControls;
    public PlayableDirector TimelineOverride;
    public CameraManipulator CameraManipulatorType = CameraManipulator.Tethered;
    public ControlPanel ControlPanelOverride;
    public List<VisualElement> GeneratedControls = new List<VisualElement>();

    public GameObject BindedObject { get; set; }

    public void Awake()
    {


        if (Application.isPlaying)
        {

            if(PopupWindowContent != null)
            {
                GenerateContent();
            }

            if (referenceTitle == "")
            {
                Debug.LogError(("The reference title of ") + this.ToString() + " is empty");
            }
            uI_Manager = GameObject.FindFirstObjectByType<UI_Manager>();

            if (this == manager.Sections[0] && PopupWindowContent != null)
            {
                InitPopup();
            }
            if (CustomMenu != null)
            {
                //CustomMenu.ContrastLayer = ProjectManager.ContrastLayer;
                CustomMenu.MenuElement = uI_Manager.uIDocument.rootVisualElement.Q<VisualElement>(CustomMenu.MenuBinding);
                CustomMenu.uIRoot = uI_Manager.uIDocument.rootVisualElement;
                CustomMenu.BindAllButtons();
            }
        }
    }
    private void Start()
    {
        if (!Application.isPlaying)
            return;
        if (ControlPanelOverride.menuBindings.Count > 0)
        {
            if (OverrideType == ControlPanleOverrideType.ReplaceControls)
            {
                ControlPanelOverride.GeneratedContainer = uI_Manager.GenerateControlPanel(ControlPanelOverride);
            }
            if (OverrideType == ControlPanleOverrideType.AddControls)
            {
                uI_Manager.GenerateControls(ControlPanelOverride, this);
            }
        }
    }

    public void Update()
    {

    }

#if UNITY_EDITOR
    static MethodInfo setIconEnabled;
    static MethodInfo SetIconEnabled => setIconEnabled = setIconEnabled ??
    Assembly.GetAssembly(typeof(Editor))
    ?.GetType("UnityEditor.AnnotationUtility")
    ?.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);
    public static void SetGizmoIconEnabled(Type type, bool on)
    {
        if (SetIconEnabled == null) return;
        const int MONO_BEHAVIOR_CLASS_ID = 114; // https://docs.unity3d.com/Manual/ClassIDReference.html
        SetIconEnabled.Invoke(null, new object[] { MONO_BEHAVIOR_CLASS_ID, type.Name, on ? 1 : 0 });
    }

    public float GizmoCameraScale;
    private PivotGizmo pivotGizmo;
    private void OnDrawGizmos()
    {
        if (pivotGizmo == null)
        {
            pivotGizmo = Pivot.gameObject.GetComponent<PivotGizmo>();
        }
        //SetGizmoIconEnabled(Camera ,false);
        GizmoUtility.SetGizmoEnabled(typeof(Camera), false);
        GizmoUtility.SetGizmoEnabled(typeof(CinemachineBrain), false);
        Camera c = Camera.main;
        Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
        /*Gizmos.DrawFrustum(Vector3.zero, c.fieldOfView, 1, 0.5f, c.aspect);

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.5f,0.5f,1f));*/
        //SceneView.lastActiveSceneView.drawGizmos = true;


        float CameraSize = 0.2f;
        Vector3 CameraSizeVector = new Vector3(CameraSize, CameraSize, CameraSize);
        GizmoCameraScale = ProjectManager.GetGizmoSize(CameraSizeVector);

        if (manager.ActiveSection == this)
        {
            Gizmos.color = Color.cyan;
            pivotGizmo.selectedColor = Color.green;
        }
        else
        {
            pivotGizmo.selectedColor = Color.white;
        }
        GameObject MeshGO = Resources.Load<GameObject>("Gizmos/Camera Mesh");
        Gizmos.DrawMesh(MeshGO.GetComponent<MeshFilter>().sharedMesh, 0, Vector3.zero, Quaternion.identity, CameraSizeVector * GizmoCameraScale);


        
    }





    


    
    private void OnValidate()
    {
   
    }

    private void OnDestroy()
    {

        if (!Application.isPlaying)
        {
            if (manager != null)
            {
                for (int i = 0; i < manager.Sections.Count; i++)
                {
                    if (manager.Sections[i] == this)
                    {
                        manager.Sections.RemoveAt(i);
                        Debug.Log("Section Removed");
                    }
                }

            }
            if(gameObject.scene.isLoaded)
            {
                AssetDatabase.DeleteAsset(timelineAssetPath);
                AssetDatabase.DeleteAsset(calloutAssetPath);
            }
        }
        
    }
#endif

    public string SectionTitle;
    public string referenceTitle;

    [Space(10)]
    [Header("Transition Durations")]
    [Space(10)]
    
    public float CallOutTransitionDuration = 0.5f;
    public float CameraTransitionDuration = 2f;
    public float MaterialTransitionDuration = 1f;

    public Label CallOutSettings;
    public Transition TransitionSettings;

    [Space(10)]
    [Header ("Slide out Settings")]
    [Space(10)]
    
    public bool AutoSizeSlides;
    public VirtualCameraObject sectionCamera;
    [Space(10)]

    public List<TextMeshPro> WorldspaceLabels = new List<TextMeshPro>();

    public List<TweenCamera> TweenCameras = new List<TweenCamera>();

    public int SectionID;
    
    public float FadeDelay;

    [Serializable]
    public class LabelSettings
    {
        public Color LabelColor = new Vector4(1, 1, 1, 1);
        [Range(0f, 1f)]
        public float LabelAlpha = 1f;
        public float TransitionDuration = 0.5f;
        public bool HiddenPointers = false;
        [Range(0f, 1f)]
        public float ShutterSize = 1f;
    }
    [Serializable]
    public struct Label
    {
        public LabelSettings settings;
        public CinemachineVirtualCamera TargetCamera;
    }


    [Serializable]
    public struct Transition
    {
        public CanvasGroup canvasGroup;
        public BackgroundColorTransition BackgroundTransition;
    }

    [Serializable]
    public struct BackgroundColorTransition
    {
        public bool EnableTransition;
        public Color BackgroundColor;
        public float GradientPower;
    }

    [Serializable]
    public struct MaterialCutOut
    {
        public CutOutPoint CutOut;
        public Material[] CutOutMaterial;
    }

    [SerializeField]
    public List<GeneratedContent> generatedContents = new List<GeneratedContent>();
    public List<string> ExhibitNames = new List<string>();
    public void GenerateContent()
    {
        var ParentContainer = new VisualElement();
        //ParentContainer
        /*ParentContainer.style.width = new Length(100, LengthUnit.Percent);
        ParentContainer.style.height = new Length(100, LengthUnit.Percent);*/
        for (int i = 0; i < PopupWindowContent.slides.Length; i++)
        {
            //initialize data for the content as visualelements
            var Container = new GeneratedContent();

            List<VisualElement> childElements = new List<VisualElement>();
            var baseElement = new VisualElement();
            /*baseElement.style.width = new Length(100, LengthUnit.Percent);
            baseElement.style.height = new Length(100, LengthUnit.Percent);*/
            baseElement.transform.position = Vector3.zero;

            List<ContentSection> Sections = new List<ContentSection>();
            for (int j = 0; j < PopupWindowContent.slides[i].Images.Length; j++)
            {
                var Section = new ContentSection();
                var element = new VisualElement();
                /*element.style.width = new Length(100, LengthUnit.Percent);
                element.style.height = new Length(100, LengthUnit.Percent);*/
                element.style.backgroundImage = PopupWindowContent.slides[i].Images[j].texture;
                element.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                baseElement.Add(element);
                childElements.Add(element);
                Section.content = element;
                Section.content.AddToClassList("unselectedPopupContent");
                Sections.Add(Section);
            }
            ExhibitNames.Add(PopupWindowContent.slides[i].ExhibitName);
            baseElement.AddToClassList("unselectedPopupContent");
            //Add all the generated variables to the GeneratedContent Struct
            Container.Container = baseElement;
            Container.ChildContent = Sections;
            if(PopupWindowContent.slides[i].ExhibitName != null)
            {
                Container.Title = PopupWindowContent.slides[i].ExhibitName;
            }
            else
            {
                Debug.LogWarning(PopupWindowContent.ToString() + " is missing a title!") ;
            }
            generatedContents.Add(Container);
            ParentContainer.Add(baseElement);
            
            Debug.Log("ContentGenerated");
        }
        ParentContainer.AddToClassList("unselectedPopupContent");
        GeneratedPopup = ParentContainer;
    }
    public void InitPopup()
    {
        PopupManager.generatedContents = generatedContents;
        PopupManager.ExhibitNames = ExhibitNames;
    }
}
