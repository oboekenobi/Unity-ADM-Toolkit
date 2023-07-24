#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Toolbars;
using Cinemachine;
using UnityEngine.Timeline;
using ADM.UISystem;

[Overlay(typeof(SceneView), "ADM 3D Toolkit", true)]

public class VirtualCameraSwitching : ToolbarOverlay
{
    const string k_Id = "ADMoverlay";
    public static List<CinemachineVirtualCamera> Cameras = new List<CinemachineVirtualCamera>();
    public static CinemachineVirtualCamera activeCamera = null;
    public static bool PerspCamera;

    //SceneView.lastActiveSceneView.camera

    public static bool NullProject(ProjectManager manager)
    {
        if(manager.currentProject == null)
        {
            Debug.LogError("No project has been set. Set a project to continue!");

            return true;
        }
        else
        {
            return false;
        }
    }
    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement() { name = "ADM Tool Bar" };
        root.Add(new Label() { text = "ADM 3D Interactive Tools" });

        return root;
    }
    VirtualCameraSwitching() : base(
            CameraLabel.id,
            PictureLabel.id,
            InteractionCamera.id,
            BetweenCamera.id,
            DropdownExample.id
            )

    { }
}

#region CallOut Label

[EditorToolbarElement(id, typeof(SceneView))]
class CameraLabel : EditorToolbarButton//, IAccessContainerWindow
{
    // This ID is used to populate toolbar elements.

    public const string id = "ExampleToolbar/Label";



    // IAccessContainerWindow provides a way for toolbar elements to access the `EditorWindow` in which they exist.
    // Here we use `containerWindow` to focus the camera on our newly instantiated objects after creation.
    //public EditorWindow containerWindow { get; set; }

    // Because this is a VisualElement, it is appropriate to place initialization logic in the constructor.
    // In this method you can also register to any additional events as required. In this example there is a tooltip, an icon, and an action.

    public CameraLabel()
    {

        // A toolbar element can be either text, icon, or a combination of the two. Keep in mind that if a toolbar is
        // docked horizontally the text will be clipped, so usually it's a good idea to specify an icon.

        text = "Create Label";
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CreateCubeIcon.png");
        tooltip = "Create a UI Label. Dont forget to assign it in the LabelManager!!";
        clicked += OnClick;
    }
    // This method will be invoked when the `Create Cube` button is clicked.

    void OnClick()
    {
        ProjectManager manager = GameObject.FindWithTag("SceneManager").GetComponent<ProjectManager>();

        if(manager == null)
        {
            Debug.LogWarning("You cannot place a Label if you don't have a Project Manager");
            return;
        }

        if (VirtualCameraSwitching.NullProject(manager))
        {
            return;
        }

        GameObject newObj = PrefabUtility.InstantiatePrefab(manager.toolkitManager.LabelPrefab) as GameObject;
        newObj.transform.parent = GameObject.FindWithTag("CallOut Canvas").transform;

        //var newObj = GameObject.Instantiate(Resources.Load<GameObject>("PreFabs/CallOut Label"), GameObject.FindWithTag("CallOut Canvas").transform, false);

        //CallOutLabel label = GameObject.FindFirstObjectByType<CallOutLabel>();
        CallOutLabel label = newObj.GetComponentInChildren<CallOutLabel>();

        label.enabled = true;
        //newObj.name = "Empty Label";
        manager.Sections[manager.ActiveSectionIndex].CallOutGameObjects.Add(newObj);
        manager.Sections[manager.ActiveSectionIndex].CallOuts.Add(label);
        label.transition = GameObject.FindWithTag("SceneManager").GetComponent<Effects_Manager>();
        //replace active section with an int 

        // When writing editor tools don't forget to be a good citizen and implement Undo!

        Undo.RegisterCreatedObjectUndo(newObj.gameObject, "Create Label");

        //if (containerWindow is SceneView view)
        //    view.FrameSelected();

    }


    void InitializeTimeline()
    {
        
    }
}
#endregion

#region Picture label
[EditorToolbarElement(id, typeof(SceneView))]
class PictureLabel : EditorToolbarButton//, IAccessContainerWindow
{
    // This ID is used to populate toolbar elements.

    public const string id = "ExampleToolbar/VisualLabel";



    // IAccessContainerWindow provides a way for toolbar elements to access the `EditorWindow` in which they exist.
    // Here we use `containerWindow` to focus the camera on our newly instantiated objects after creation.
    //public EditorWindow containerWindow { get; set; }

    // Because this is a VisualElement, it is appropriate to place initialization logic in the constructor.
    // In this method you can also register to any additional events as required. In this example there is a tooltip, an icon, and an action.

    public PictureLabel()
    {

        // A toolbar element can be either text, icon, or a combination of the two. Keep in mind that if a toolbar is
        // docked horizontally the text will be clipped, so usually it's a good idea to specify an icon.

        text = "Create Picture Label";
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CreateCubeIcon.png");
        tooltip = "Create a UI Label. Dont forget to assign it in the LabelManager!!";
        clicked += OnClick;
    }
    // This method will be invoked when the `Create Cube` button is clicked.

    void OnClick()
    {
        ProjectManager manager = GameObject.FindWithTag("SceneManager").GetComponent<ProjectManager>();
        if(manager == null)
        {
            Debug.LogWarning("You cannot place a Picture Label if you don't have a Project Manager");
            return;
        }

        if (VirtualCameraSwitching.NullProject(manager))
        {
            return;
        }

        GameObject newObj = PrefabUtility.InstantiatePrefab(manager.toolkitManager.PictureLabelPrefab) as GameObject;

        //var newObj = GameObject.Instantiate(Resources.Load<GameObject>("PreFabs/Picture Label"), GameObject.FindWithTag("CallOut Canvas").transform, false);
        newObj.name = "Empty Picture Label";
        manager.Sections[manager.ActiveSectionIndex].CallOutGameObjects.Add(newObj);
        // When writing editor tools don't forget to be a good citizen and implement Undo!

        Undo.RegisterCreatedObjectUndo(newObj.gameObject, "Create Picture Label");

        //if (containerWindow is SceneView view)
        //    view.FrameSelected();

    }

}
#endregion

#region Camera

[EditorToolbarElement(id, typeof(SceneView))]
class InteractionCamera : EditorToolbarButton//, IAccessContainerWindow
{
    // This ID is used to populate toolbar elements.

    public const string id = "ExampleToolbar/Camera";
    // IAccessContainerWindow provides a way for toolbar elements to access the `EditorWindow` in which they exist.
    // Here we use `containerWindow` to focus the camera on our newly instantiated objects after creation.
    //public EditorWindow containerWindow { get; set; }

    // Because this is a VisualElement, it is appropriate to place initialization logic in the constructor.
    // In this method you can also register to any additional events as required. In this example there is a tooltip, an icon, and an action.
    public static int count;
    public InteractionCamera()
    {
        // A toolbar element can be either text, icon, or a combination of the two. Keep in mind that if a toolbar is
        // docked horizontally the text will be clipped, so usually it's a good idea to specify an icon.

        text = "Create Camera";
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CreateCameraIcon.png");
        tooltip = "Create an Interactive Camera. Dont forget to assign it in the Project Manager!!";
        clicked += OnClick;
    }
    // This method will be invoked when the `Create Cube` button is clicked.

    void OnClick()
    {
        ProjectManager manager = GameObject.FindFirstObjectByType<ProjectManager>();
        if(manager == null)
        {
            Debug.LogWarning("You cannot place a camera if you don't have a Project Manager");
            return;
        }

        if (VirtualCameraSwitching.NullProject(manager))
        {
            return;
        }

        

        GameObject newObj = PrefabUtility.InstantiatePrefab(manager.toolkitManager.CameraPrefab) as GameObject;
        VirtualCameraObject Vobj = newObj.GetComponent<VirtualCameraObject>();
        if (manager.CameraFolder == null)
        {
            GameObject cameraFolder = new GameObject("Cameras");
            manager.CameraFolder = cameraFolder;
        }
        newObj.transform.parent = manager.CameraFolder.transform;
        SceneVisibilityManager.instance.DisablePicking(newObj, true);
        Vobj.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
        Vobj.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;


        //var newObj = GameObject.Instantiate(Resources.Load<GameObject>("PreFabs/Default Camera"), SceneView.lastActiveSceneView.camera.transform.position, SceneView.lastActiveSceneView.camera.transform.rotation, GameObject.FindWithTag("CameraFolder").transform);
        newObj.name = "Empty Camera";
        Vobj.manager = manager;
        Vobj.section.manager = manager;
        Vobj.section.Camera = Vobj;
        Vobj.section.SectionTitle = "Empty Section";
        Vobj.enabled = true;
        manager.Sections.Add(Vobj.section);
        Vobj.section.SectionID = manager.Sections.Count -1;
        Vobj.section.Pivot = Vobj.PivotAngle;
        Vobj.section.enabled = true;


        InitializeTimeline(Vobj.section, manager);

        Vector3 pivotPostion = RaycastHitPosition();
        Debug.Log(pivotPostion);

        if (pivotPostion != Vector3.zero)
        {
            Vobj.PivotAngle.position = pivotPostion;
        }

        manager.ActiveSectionIndex = Vobj.section.SectionID;
        CinemachineVirtualCamera virtualCam = Vobj.VirtualCamera;
        VirtualCameraSwitching.Cameras.Add(virtualCam);
        count = VirtualCameraSwitching.Cameras.Count;
        DropdownExample.menu.AddItem(new GUIContent(newObj.name + count), true, () => { text = newObj.name + count;; });
        manager.EditorCameraSwitch(virtualCam);
        Selection.activeGameObject = Vobj.CameraChild;



        // When writing editor tools don't forget to be a good citizen and implement Undo!
        Undo.RegisterCreatedObjectUndo(newObj.gameObject, "Create Camera");
    }


    public void InitializeTimeline(PresentationSection section, ProjectManager manager)
    {
        InputManager camManager = GameObject.FindFirstObjectByType<InputManager>();

        string filePath = (manager.currentProject + "/Timeline Instances/Timeline Instance.playable");
        section.assetPath = AssetDatabase.GenerateUniqueAssetPath(filePath);
        //AssetDatabase.CreateAsset(camManager.TimelineTemplate, section.assetPath);

        bool copied = AssetDatabase.CopyAsset(("Packages/com.adm.adm-toolkit/Runtime/Resources/ADM Toolkit/Timeline Template.playable"), section.assetPath);

        section.TimelineInstance = (TimelineAsset)AssetDatabase.LoadAssetAtPath(section.assetPath, typeof(TimelineAsset));
        section.director.playableAsset = section.TimelineInstance;
        if (manager.Sections.Count > 1)
        {
            section.PreviousCamera = manager.Sections[section.SectionID - 1].VirtualCamera;
        }

        camManager.BindCinemachineCameras(section, camManager);
        //CameraMovement.BindTimelineResets("Label Track Asset", section, camManager.TransitionCamera);
    }

    public Vector3 RaycastHitPosition()
    {
        Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

}
#endregion

#region TweenCamera

[EditorToolbarElement(id, typeof(SceneView))]
class BetweenCamera : EditorToolbarButton//, IAccessContainerWindow
{
    // This ID is used to populate toolbar elements.

    public const string id = "ExampleToolbar/TweenCamera";
    // IAccessContainerWindow provides a way for toolbar elements to access the `EditorWindow` in which they exist.
    // Here we use `containerWindow` to focus the camera on our newly instantiated objects after creation.
    //public EditorWindow containerWindow { get; set; }

    // Because this is a VisualElement, it is appropriate to place initialization logic in the constructor.
    // In this method you can also register to any additional events as required. In this example there is a tooltip, an icon, and an action.
    public static int count;
    public BetweenCamera()
    {
        // A toolbar element can be either text, icon, or a combination of the two. Keep in mind that if a toolbar is
        // docked horizontally the text will be clipped, so usually it's a good idea to specify an icon.

        text = "Create Tween Camera";
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CreateCameraIcon.png");
        tooltip = "Create an Interactive Camera. Dont forget to assign it in the Project Manager!!";
        clicked += OnClick;
    }
    // This method will be invoked when the `Create Cube` button is clicked.

    void OnClick()
    {
        ProjectManager manager = GameObject.FindWithTag("SceneManager").GetComponent<ProjectManager>();
        if(manager == null)
        {
            Debug.LogWarning("You cannot place a Tween Camera if you dont have a Project Manager");
            return;
        }

        if (VirtualCameraSwitching.NullProject(manager))
        {
            return;
        }

        GameObject newObj = PrefabUtility.InstantiatePrefab(manager.toolkitManager.TweenCameraPrefab) as GameObject;
        newObj.transform.parent = GameObject.FindWithTag("CameraFolder").transform;
        newObj.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
        newObj.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;

        //var newObj = GameObject.Instantiate(Resources.Load<GameObject>("PreFabs/Tween Camera"), SceneView.lastActiveSceneView.camera.transform.position, SceneView.lastActiveSceneView.camera.transform.rotation, GameObject.FindWithTag("CameraFolder").transform);
        //newObj.name = "Empty Camera";
        TweenCamera Vobj = newObj.GetComponent<TweenCamera>();
        Vobj.manager = manager;
        Vobj.section = manager.ActiveSection;
        Vobj.section.TweenCameras.Add(Vobj);

        //Vobj.manager = manager;
        //Vobj.section.manager = manager;
        //Vobj.section.Camera = Vobj;
        //Vobj.section.SectionTitle = "Empty Section";
        Vobj.enabled = true;
        //manager.Sections.Add(Vobj.section);
        //Vobj.section.SectionID = manager.Sections.Count - 1;
        //InitializeTimeline(Vobj.section, manager);
        //Vobj.section.Pivot = Vobj.PivotAngle;
        //Vobj.section.enabled = true;

        Vector3 pivotPostion = RaycastHitPosition();
/*        Debug.Log(pivotPostion);
        if (pivotPostion != Vector3.zero)
        {
            Vobj.PivotAngle.position = pivotPostion;
        }*/

        //manager.ActiveSectionIndex = Vobj.section.SectionID;
        /*CinemachineVirtualCamera virtualCam = Vobj.VirtualCamera;
        VirtualCameraSwitching.Cameras.Add(virtualCam);
        count = VirtualCameraSwitching.Cameras.Count;
        DropdownExample.menu.AddItem(new GUIContent(newObj.name + count), true, () => { text = newObj.name + count; ; });
        manager.ActiveCamera = virtualCam;
        manager.EditorCameraSwitch(virtualCam);
        Selection.activeGameObject = Vobj.CameraChild;*/

        // When writing editor tools don't forget to be a good citizen and implement Undo!
        Undo.RegisterCreatedObjectUndo(newObj.gameObject, "Create Camera");
    }

    public Vector3 RaycastHitPosition()
    {
        Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

}
#endregion

#region DropDown Picker
[EditorToolbarElement(id, typeof(SceneView))]
class DropdownExample : EditorToolbarDropdownToggle
{
    public const string id = "ExampleToolbar/Dropdown";
    public static GenericMenu menu = new GenericMenu();
    public string dropChoice;
    public List<int> dropChoices = new List<int>();
    public DropdownExample()
    {
        text = "Camera Preview";
        dropdownClicked += ShowDropdown;
    }
    void ShowDropdown()
    {
        var menu = new GenericMenu();
        ProjectManager manager = GameObject.FindWithTag("SceneManager").GetComponent<ProjectManager>();
        //menu.AddItem(new GUIContent("Persp Camera"), false, () => { text = "Previewed Camera"; ProjectManager.PerspCamera = true; });
        for (int i = 0; i < manager.SceneCameras.Count; i++)
        {
            if (manager.SceneCameras.Count > 0 && manager.SceneCameras[i] != null)
            {
                VirtualCameraObject Camera = manager.SceneCameras[i];
                
                menu.AddItem(new GUIContent(manager.SceneCameras[i].CameraChild.name), false, () => { text = Camera.name; SelectCamera(Camera);});
            }

        }
        menu.ShowAsContext();
    }
    void SelectCamera(VirtualCameraObject Cam)
    {
        ProjectManager manager = GameObject.FindWithTag("SceneManager").GetComponent<ProjectManager>();
        if (ProjectManager.CanSwitchCamera)
        {
            ProjectManager.CanSwitchCamera = false;
        }
        if (manager.SceneCameras != null)
        {
            manager.EditorCameraSwitch(Cam.VirtualCamera);
        }
    }
}
#endregion

#endif