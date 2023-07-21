using Cinemachine.PostFX;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ADM.UISystem;

[ExecuteInEditMode]
public class TweenCamera : MonoBehaviour
{
    //This Script will be for an instatiated prefab that will have a button in the on a custome Toolbar Overlay panel
    //It will bind itself to the active PrensetationSection's director and it's timeline.
    //When it initializes it creates a cinemachine shot and bind it to the exposed virtualCamera reference 
    #region Dependencies
    public CinemachineVirtualCamera VirtualCamera;
    public CinemachineVolumeSettings volumeSettings;
    public PresentationSection section;
    public TweenSection tweenSection;
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
        /*if (!isAdded)
        {
            manager.SceneCameras.Add(this);

            gameObject.transform.parent = GameObject.FindWithTag("CameraFolder").transform;
            volumeSettings = CameraChild.GetComponent<CinemachineVolumeSettings>();
            isAdded = true;
        }
        if (hasBeenDestroyed)
        {
            manager.Sections.Add(section);
            hasBeenDestroyed = false;
        }*/
    }



    void OnDestroy()
    {
        /*if (manager != null)
        {
            for (int i = 0; i < manager.Sections.Count; i++)
            {
                if (manager.Sections[i].Camera == this)
                {
                    section = manager.Sections[i];
                    if (manager.Sections[i].CallOuts.Count > 0)
                    {
                        for (int j = 0; j < manager.Sections[i].CallOuts.Count; j++)
                        {
                            DestroyImmediate(manager.Sections[i].CallOuts[j]);
                        }
                    }
                    manager.Sections.RemoveAt(i);
                    Debug.Log("Section Removed");
                }
            }
            hasBeenDestroyed = true;
        }*/


        if(section != null)
        {
            for(int i = 0; i < section.TweenCameras.Count; i++)
            {
                if (section.TweenCameras[i] == this)
                {
                    section.TweenCameras.RemoveAt(i);
                }
            }
        }
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            /*if (VirtualCamera == null)
            {
                DestroyImmediate(gameObject);
            }
            if (manager != null)
            {
                if (!manager.SceneCameras.Contains(this))
                {
                    manager.SceneCameras.Add(this);
                }
            }*/

            if (CameraChild != null && PivotAngle != null)
            {
                VirtualCamera.transform.LookAt(PivotAngle.position);
                Debug.DrawLine(VirtualCamera.transform.position, PivotAngle.position, RayColor);
                //gameObject.name = section.SectionTitle + "Tween Camera";
                //CameraChild.name = section.SectionTitle + "Tween Camera Child";
                //PivotAngle.gameObject.name = section.SectionTitle + "Tween Pivot";
            }
        }
    }
    //Bind this virtualCamera to the active director's Cinemachine timeline track
    public void BindTweenCamera()
    {
        /*foreach (var playableAssetOutput in section.director.playableAsset.outputs)
        {
            if (playableAssetOutput.streamName == "Cinemachine Track")
            {
                var cinemachineTrack = playableAssetOutput.sourceObject as CinemachineShot;

                if (cinemachineTrack == null)
                {
                    Debug.Log("Track is null");
                }

                var CinemachineShot = cinemachineTrack.CreateClip<"p">
                var FirstCinemachineShot = cinemachineTrack.GetClips().ElementAt(1).asset as CinemachineShot;
                FirstCinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
                section.director.SetReferenceValue(FirstCinemachineShot.VirtualCamera.exposedName, section.VirtualCamera);

            }

        }
*/
        //AssetDatabase.Refresh();
    }

#endif
}
