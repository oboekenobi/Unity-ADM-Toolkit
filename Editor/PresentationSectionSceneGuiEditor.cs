#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class PresentationSectionSceneGUI : Editor
{
    private PresentationSection targetSection;
    private ProjectManager manager;
    public void SetTarget(PresentationSection section)
    {
        targetSection = section;
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (!Application.isPlaying)
        {
            PresentationSection camera = targetSection;
            if (camera == null)
            {
                return;
            }

            if(manager == null)
            {
                manager = GameObject.FindFirstObjectByType<ProjectManager>();
            }

            if (manager.SceneCameras != null)
            {
                manager.uI_Manager.PreviousPresentationSection = manager.ActiveSection;
                manager.CanSwitchEditorCamera = true;
                manager.EditorCameraSwitch(camera.sectionCamera.VirtualCamera);
            }
            if (manager.ActiveSection != manager.LastActiveSection)
            {
                manager.inputManager.SetDefaultCinemachineCamera();

                //manager.ActiveSection.director.time = manager.ActiveSection.director.duration;
                manager.ActiveSection.director.time = 0;
                manager.ActiveSection.director.RebuildGraph();
                manager.ActiveSection.director.Play();
                manager.ActiveSection.director.playableGraph.GetRootPlayable(0);

                Debug.Log("Camera Selected");
                //lock the Timeline and release it if the current selected camera is different
                TimelineState.SetLockStatus(false);
                TimelineState.SetLockStatus(true);
            }
            else
            {

            }
        }
    }
}


#endif