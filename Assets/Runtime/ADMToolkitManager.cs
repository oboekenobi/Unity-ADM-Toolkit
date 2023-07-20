using ADM.UISystem;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ADMToolkitManager : MonoBehaviour
{
    #region Editor Variable Checkers
    void SlideNullChecker()
    {

    }

    #endregion

    [SerializeField]
    public Color BackgroundColor;

    public Material BackgroundMaterial;

    [HideInInspector]
    public List<GameObject> GarbageCollection;
   

    public static CanvasGroup ContrastLayer;

    public UI_Manager UI;

    public GameObject WaterMark;

    public GameObject CameraPrefab;
    public GameObject TweenCameraPrefab;
    public GameObject LabelPrefab;
    public GameObject PictureLabelPrefab;

    public static CanvasGroup CallOutCanvas;

    #region Static Variables
    public static ProjectManager _instance;
    public static bool CanSwitchCamera;
    public static bool PerspCamera;
    public static bool LabelHighlited;
    public static bool previewCameraFramed;
    //public static 
    #endregion


    [HideInInspector]
    public CinemachineBrain Brain;
    public Transform MainCameraTransform;
    public InputManager CinemahcineManager;

    public string currentProject;

#if UNITY_EDITOR
    private void OnEnable()
    {

    }
#endif
    private void Awake()
    {

        if (Application.isPlaying)
        {
            Brain = GameObject.FindWithTag("Camera Manager").GetComponent<CinemachineBrain>();
            CallOutCanvas = GameObject.FindWithTag("CallOut Canvas").GetComponent<CanvasGroup>();
            ContrastLayer = GameObject.FindWithTag("Contrast Layer").GetComponent<CanvasGroup>();
        }



        OrderSections();



    }

    public void OrderSections()
    {
        for (int i = 0; i < Sections.Count; i++)
        {
            Sections[i].SectionID = i;
        }
    }

    [Space(10)]
    [Header("Section Management")]
    [Space(10)]
    public List<PresentationSection> Sections = new List<PresentationSection>();

    private bool canLoadToggles;
    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (GarbageCollection.Count > 0)
            {
                foreach (GameObject go in GarbageCollection)
                {
                    DestroyImmediate(go);
                    GarbageCollection.Clear();
                }
            }
        }

        SlideNullChecker();
#endif

    }



}
