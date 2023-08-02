using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using ADM.UISystem;
using Cinemachine.PostFX;
using UnityEditor;
using UnityEngine.Playables;

[ExecuteInEditMode]
public class VirtualCameraObject : MonoBehaviour
{
    #region Dependencies
    public CinemachineVirtualCamera VirtualCamera;
    public CinemachineVolumeSettings volumeSettings;
    public PlayableDirector director;
    public PresentationSection section;
    public ProjectManager manager;
    public GameObject CameraChild;
    public Transform PivotAngle;
    public Color SelectedRayColor;
    public Color DeselectedRayColor;
    public Color RayColor;
    public bool isAdded = false;
    public bool hasBeenDestroyed;
    #endregion




    public List<CinemachineVirtualCamera> DebugList;

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (!isAdded)
        {
            manager.SceneCameras.Add(this);
            volumeSettings = CameraChild.GetComponent<CinemachineVolumeSettings>();
            isAdded = true;
        }
        if (hasBeenDestroyed)
        {
            manager.Sections.Add(section);
            hasBeenDestroyed = false;
        }
    }


#if UNITY_EDITOR
    void OnDestroy()
    {
        if(manager != null && !Application.isPlaying)
        {
            for (int i = 0; i < manager.Sections.Count; i++)
            {
                if (manager.Sections[i].sectionCamera == this)
                {
                    section = manager.Sections[i];
                    manager.Sections.RemoveAt(i);
                    Debug.Log("Section Removed");
                }
            }
            hasBeenDestroyed = true;
        }
    }
#endif

    void Update()
    {
        if (!Application.isPlaying)
        {
            if(VirtualCamera == null)
            {
                DestroyImmediate(gameObject);
            }
            if(manager != null)
            {
                if (!manager.SceneCameras.Contains(this))
                {
                    manager.SceneCameras.Add(this);
                }
            }
            
            if (CameraChild != null)
            {
                VirtualCamera.transform.LookAt(PivotAngle.position);
                Debug.DrawLine(VirtualCamera.transform.position, PivotAngle.position, RayColor);
                gameObject.name = section.SectionTitle + " Camera";
                CameraChild.name = section.SectionTitle;
                PivotAngle.gameObject.name = section.SectionTitle + " Pivot";
            }
        }
    }
#endif
}
