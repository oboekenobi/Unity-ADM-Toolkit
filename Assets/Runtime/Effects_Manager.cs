using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ADM.UISystem;
using Cinemachine.PostFX;
using UnityEngine.Rendering;
using System;

[ExecuteInEditMode]
public class Effects_Manager : MonoBehaviour
{
    public GameObject focusFadeObject;
    public ProjectManager projectManager;
    public static Effects_Manager _instance;
    public CanvasGroup ContrastLayer;
    public InputManager cam;
    //public Material Background;
    public Material OutlineMaterial;
    public Shader ADMShaderGraph;
    public List<Material> RayMaterials = new List<Material>();
    [Range(0, 5)]
    public float CurrentDuration = 1;
    [Range(0, 4)]
    public float CalloutDuration = 0.5f;
    public List<HighlightOutline> focusOutlineObject;
    [Range (0,4)]
    public float outlineWidth;
    public Color outlineColor;
    public float cutOutSize = 0.15f;
    public float smallCutOutSize = 0.10f;
    public float pulseIntensity = 0.8f;
    public float Xrayfade;
    public static List<HighlightOutline> Outlines = new List<HighlightOutline>();

    public GameObject Video;
    public static bool ToOutline = false;
    public static bool releaseCallOuts;
    public static bool XRayMaterialsInQue;
    public static bool AlphaFadeMaterialsInQue;

    public static List<CanvasGroupMixerBehaviour> CanvasGroupTracks = new List<CanvasGroupMixerBehaviour>();
    public static List<CanvasGroup> CanvasGroups = new List<CanvasGroup>();
    public bool ContrastLayerActive;
    public bool IsCutOut;
    public bool FromCutOut;


    public static List<Material> ActiveXRayMaterials = new List<Material>();
    public static List<CallOutLabel> ActiveCalloutLabels = new List<CallOutLabel>();
    public static List<Material> ActiveAlphaFadeObject = new List<Material>();


    // Start is called before the first frame update

#if UNITY_EDITOR
    private void OnEnable()
    {
        if(projectManager != null)
        {
            //Background.SetColor("_Top", projectManager.toolkitManager.BackgroundColor);
        }
    }

#endif
    //Write a function that will Tranisiton the color in Background to another color and back
    [SerializeField]
    public Color StartBackgroundColor;
    [SerializeField]
    public Color EndBakcgroundColor;
    public Color OriginalBackgroundColor;

    public void Awake()
    {
        projectManager = GameObject.FindFirstObjectByType<ProjectManager>();

        if(projectManager == null)
        {
            Debug.LogWarning("No ProjectManager in the scene! remember to add a project manager");
        }

        if(_instance == null)
        {
            Effects_Manager._instance = this;
        }
        
    }
    void Start()
    {
        CutOutTransition();
    }

    private void Update()
    {
        RayMaterials = ActiveXRayMaterials;
    }
    /*public void TransitionBackground(float duration, bool Active)
    {
        if (Active)
        {
            StartBackgroundColor = Background.GetColor("_Top");
            EndBakcgroundColor = projectManager.ActiveSection.TransitionSettings.BackgroundTransition.BackgroundColor;
            StartCoroutine(ColorTransition(Background, StartBackgroundColor, EndBakcgroundColor, duration, "_Top"));
        }
        else
        {
            StartCoroutine(ColorTransition(Background, EndBakcgroundColor, StartBackgroundColor, duration, "_Top"));
        }
    }*/

    private SortingGroup sortingGroup;
    public float CurrentSmoothness;

    /*public void FadeToXray(Material fadeObject, float duration)
    {
        //fadeObject.renderQueue = 3050;
        CurrentSmoothness = fadeObject.GetFloat("_Smoothness");
        StartCoroutine(XRayFadeTransition(fadeObject, 1, Xrayfade, CurrentSmoothness, 0, duration, true));
        ActiveXRayMaterials.Add(fadeObject);
        CurrentDuration = duration;
    }*/
    public void FadeFromXray()
    {
        if (ActiveXRayMaterials.Count > 0)
        {
            foreach(Material mat in ActiveXRayMaterials)
            {
                StartCoroutine(XRayFadeTransition(mat, mat.GetFloat("_XRay"), 1, 0, CurrentSmoothness, CurrentDuration, false));
            }
            XRayMaterialsInQue = false;
            ActiveXRayMaterials.Clear();
        }
    }

    public void FadeFromAlpha()
    {
        if (ActiveAlphaFadeObject.Count > 0)
        {
            foreach (Material mat in ActiveAlphaFadeObject)
            {
                StartCoroutine(FadeTransition(mat, mat.GetFloat("_Alpha"), 1, 0, CurrentSmoothness, CurrentDuration, false));
            }
            AlphaFadeMaterialsInQue = false;
            ActiveAlphaFadeObject.Clear();
        }
    }

    public void FadeFromCanvasGroup()
    {
        if (CanvasGroups.Count > 0)
        {
            foreach (CanvasGroup canvas in CanvasGroups)
            {
                StartCoroutine(CanvasGroupTransition(canvas, 1, 0, CurrentDuration, false));
            }
            foreach(CanvasGroupMixerBehaviour track in CanvasGroupTracks)
            {
                track.registered = false;
            }
            CanvasGroupTracks.Clear();
            //CanvasGroupInQue = false;
            CanvasGroups.Clear();
        }
    }

    public void CloseLabels(PresentationSection lastSection)
    {
        StartCoroutine(CloseCallouts(lastSection));
    }

    public IEnumerator CloseCallouts(PresentationSection section)
    {
        if(section.CallOuts.Count > 0)
        {
            float elapsedTime = 0f;
            float intensity = 1f;
            float TagHeight = 0;
            float TagWidth = 0;
            float duration = 0.2f;
            //float duration = CalloutDuration;
            releaseCallOuts = true;
            while(elapsedTime <= duration)
            {
                intensity = Mathf.Lerp(1, 0, elapsedTime / duration);
                foreach (CallOutLabel label in section.CallOuts)
                {
                    float halfFinal = Mathf.Clamp(intensity * 2, 0, 1f);
                    float final = ((Mathf.Clamp(intensity, 0.5f, 1f)) - 0.5f) * 2;
                    label.ChildTag.localScale = new Vector3(label.ChildTag.localScale.y * final, label.ChildTag.localScale.y, label.ChildTag.localScale.z);
                    TagWidth = (label.Fitter.glowBackground.rect.width * label.transform.localScale.y / 2);
                    TagHeight = (label.Fitter.glowBackground.rect.height * label.transform.localScale.y / 2);

                    for (int j = 0; j < label.UILines.Count; j++)
                    {
                        label.UILines[j].line.option.endRatio = halfFinal;
                        label.UILines[j].GeometyUpdateFlagUp();

                    }
                    for (int j = 0; j < label.CircleMaterials.Count; j++)
                    {
                        label.CircleMaterials[j].SetFloat("_Alpha", halfFinal);
                    }
                    label.ParentGroup.alpha = final;



                    if (label.LinePlacement == CallOutLabel.PlacementDirection.Left)
                    {
                        Vector3 Offset = Vector3.left * TagWidth;
                        Vector3 Direction = Offset + (Vector3.right * (TagWidth * final));
                        label.ChildTag.localPosition = Offset * (1 - final);
                    }

                    if (label.LinePlacement == CallOutLabel.PlacementDirection.Right)
                    {
                        Vector3 Offset = Vector3.right * TagWidth;
                        Vector3 Direction = Offset + (Vector3.left * (TagWidth * final));
                        label.ChildTag.localPosition = Offset * (1 - final);
                    }
                    if (label.LinePlacement == CallOutLabel.PlacementDirection.Bottom)
                    {
                        Vector3 Offset = Vector3.down * TagHeight;
                        Vector3 Direction = Offset + (Vector3.up * (TagHeight * final));
                        label.ChildTag.localPosition = Offset * (1 - final);
                    }
                    if (label.LinePlacement == CallOutLabel.PlacementDirection.Top)
                    {
                        Vector3 Offset = Vector3.up * TagHeight;
                        Vector3 Direction = Offset + (Vector3.down * (TagHeight * final));
                        label.ChildTag.localPosition = Offset * (1 - final);
                    }
                    if (label.LinePlacement == CallOutLabel.PlacementDirection.Middle)
                    {
                        Vector3 Offset = Vector3.down * TagHeight;
                        Vector3 Direction = Offset + (Vector3.up * (TagHeight * final));
                        label.ChildTag.localPosition = Offset * (1 - final);
                    }

                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            for (int i = 0; i < section.CallOutGameObjects.Count; i++)
            {
                section.CallOutGameObjects[i].SetActive(false);
                releaseCallOuts = false;
            }
        }
    }
    public void CutOutTransition()
    {
        FromCutOut = false;
        IsCutOut = true;
        /*if (projectManager.ActiveSection.TransitionSettings.MaterialTransitions.CutOuts.Length > 0)
        {
            foreach(PresentationSection.MaterialCutOut point in projectManager.ActiveSection.TransitionSettings.MaterialTransitions.CutOuts)
            {
                StartCoroutine(point.CutOut.CutOutTransition(projectManager.ActiveSection.MaterialTransitionDuration, 0, point.CutOut.CutOutSize, true));
            }
        }*/
    }
    public void FromCutOutTransition()
    {
        /*if (IsCutOut)
        {
            Debug.Log("IsExitingCutOut");
            foreach (PresentationSection.MaterialCutOut point in cam.PreviousPresentationSection.TransitionSettings.MaterialTransitions.CutOuts)
            {
                StartCoroutine(point.CutOut.CutOutTransition(cam.PreviousPresentationSection.MaterialTransitionDuration, point.CutOut.CutOutSize, 0, false));
            }
            IsCutOut = false;
        }*/
    }

    #region Coroutine Transitions

    #region Material Tranisitons
    public IEnumerator FadeTransition(GameObject fadeObject, float Start, float Finish, float duration, string Property)
    {
        float elapsedTime = 0f;
        Material fadeMat = fadeObject.transform.GetComponent<Renderer>().material;
        while (elapsedTime <= duration)
        {
            float fade = Mathf.Lerp(Start, Finish, elapsedTime / duration);
            fadeMat.SetFloat(Property, fade);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeMat.SetFloat(Property, Finish);
    }

    public IEnumerator CanvasGroupTransition(CanvasGroup Canvas, float Start, float Finish, float duration, bool Active)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            float fade = Mathf.Lerp(Start, Finish, elapsedTime / duration);
            Canvas.alpha = fade;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Canvas.alpha = Finish;
    }


    public IEnumerator MatFadeTransition(Material fadeMat, float Start, float Finish, float duration, string Property, bool Active)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            float fade = Mathf.Lerp(Start, Finish, elapsedTime / duration);
            fadeMat.SetFloat(Property, fade);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeMat.SetFloat(Property, Finish);

        if (!Active)
        {
            fadeMat.renderQueue = 3050;

        }
    }

    public IEnumerator ColorTransition(Material Mat, Color Start, Color Finish, float duration, string Property)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            float fadeR = Mathf.Lerp(Start.r, Finish.r, elapsedTime / duration);
            float fadeG = Mathf.Lerp(Start.g, Finish.g, elapsedTime / duration);
            float fadeB = Mathf.Lerp(Start.b, Finish.b, elapsedTime / duration);
            Mat.SetColor(Property, new Color(fadeR, fadeG, fadeB, 1));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Mat.SetColor(Property, new Color(Finish.r, Finish.g, Finish.b, 1));
    }

    public IEnumerator XRayFadeTransition(Material fadeMat, float Start, float Finish, float SmoothnessStart, float SmoothnessFinish, float duration, bool Active)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            float fade = Mathf.Lerp(Start, Finish, elapsedTime / duration);
            float SmoothnessFade = Mathf.Lerp(SmoothnessStart, SmoothnessFinish, elapsedTime / duration);
            fadeMat.SetFloat("_XRay", fade);
            fadeMat.SetFloat("_Smoothness", SmoothnessFade);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeMat.SetFloat("_XRay", Finish);
        fadeMat.SetFloat("_Smoothness", SmoothnessFinish);
        if (!Active)
        {
            //fadeMat.renderQueue = 3000;
        }
    }

    public IEnumerator FadeTransition(Material fadeMat, float Start, float Finish, float SmoothnessStart, float SmoothnessFinish, float duration, bool Active)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            float fade = Mathf.Lerp(Start, Finish, elapsedTime / duration);
            float SmoothnessFade = Mathf.Lerp(SmoothnessStart, SmoothnessFinish, elapsedTime / duration);
            fadeMat.SetFloat("_Alpha", fade);
            //fadeMat.SetFloat("_Smoothness", SmoothnessFade);
            //fadeMat.SetFloat("_SmoothnessOffset", fade);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeMat.SetFloat("_Alpha", Finish);
        //fadeMat.SetFloat("_Smoothness", SmoothnessFinish);
        if (!Active)
        {
            //fadeMat.renderQueue = 3000;
        }
    }


    
    #endregion

    #region 0-1 Transitions
    public IEnumerator ContrastTransition(float duration, float start, float target)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            ContrastLayer.alpha = Mathf.SmoothStep(start, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    public void StartVideo()
    {
        Video.SetActive(true);
    }
    public void StopVideo()
    {
        Video.SetActive(false);
    }

    
    #endregion

    #endregion

    #region cutOutSettings
/*    [SerializeField]
    public Transform CutOutTarget;*/
    //public Vector3 AveragedCutOutPos;

    [SerializeField]
    private LayerMask wallMask;
    public GameObject InteractionCamera;
    public List<Material> CutOutObjects = new List<Material>();

    [Range(0f, 0.3f)]
    public float CutOutSize;
    [Range(0f, 0.3f)]
    public float CutOutSizes;
    [Range(0f, 1f)]
    public float FallOff;

    [Range(0f, 1f)]
    public float DistFallOff;

    [Range(0f, 40f)]
    public float maxCameraDist;
    [Range(0f, 40f)]
    public float minCameraDist;
    public Vector2 DebugVector;
    public static float map01(float value, float min, float max)
    {
        //This Function takes a value and revalues it to 0-1, which would also require a clamped value after using this function
        return (value - min) * 1f / (max - min);
    }

    /*public Vector3 AveragedCutOutPos()
    {
        Vector3 Average = new Vector3();
        Vector3 Sum = new Vector3();
        for (int i = 0; i < projectManager.ActiveSection.TransitionSettings.MaterialTransitions.CutOuts.Length; i++)
        {
            if (projectManager.ActiveSection.TransitionSettings.MaterialTransitions.CutOuts[i].CutOut != null)
            {
                Sum += projectManager.ActiveSection.TransitionSettings.MaterialTransitions.CutOuts[i].CutOut.PointTransform.position;
            }
        }
        Average = (Sum) / projectManager.ActiveSection.TransitionSettings.MaterialTransitions.CutOuts.Length;
        return Average;
    }*/


    #endregion

}
