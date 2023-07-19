using ADM.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using UnityEngine.SceneManagement;

public class CutOutPoint : MonoBehaviour
{
    public Transform PointTransform;
    [Range(0f, 1f)]
    public float CutOutSize;
    public Effects_Manager Transition;

    public float FinalCutOutSize;
    [Range(0f, 0.3f)]
    public float CutOutSizes;
    
    public bool CanUpdate;

    public List<Material> CutOutMats = new List<Material>();

    public PresentationSection.MaterialCutOut CutOut;
    private void Start()
    {
        Transition = Effects_Manager._instance;
    }
    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
    }

    public void InitiaiteCutOuts()
    {
     
/*        if (projectManager.ActiveSection.TransitionSettings.MaterialTransitions.CutOuts.Length >= 1)
        {
            //CutOutTarget = AveragedCutOutPos();
            Dist = (Vector3.Distance(CameraMovement.MainCamera.transform.position, CutOutTarget));
            Val = map01(Dist, minCameraDist, maxCameraDist);
        }
        if (projectManager.ActiveSection.TransitionSettings.MaterialTransitions.CutOuts.Length < 1)
        {
            //CutOutTarget = projectManager.ActiveSection.TransitionSettings.MaterialTransitions.CutOuts[0].CutOut.PointTransform.position;
            Dist = (Vector3.Distance(CameraMovement.MainCamera.transform.position, CutOutTarget));
            Val = map01(Dist, minCameraDist, maxCameraDist);
        }*/


        
        //FinalCutOutSize = CutOutSizes * Val;

    }

    public void Update()
    {
        if (CanUpdate)
        {
            //UpdateCutOut();
        }
    }

    public void UpdateCutOut()
    {
        for (int m = 0; m < CutOutMats.Count; m++)
        {
            /*float Dist = new float();
            float Val = new float();
            Vector3 CutOutTarget = new Vector3();
            CutOutTarget = PointTransform.position;*/

            Vector2 cutoutPos = InputManager.MainCamera.WorldToViewportPoint(PointTransform.position);
            CutOutMats[m].SetVector("_CutoutPos", cutoutPos);
            CutOutMats[m].SetFloat("_FalloffSize", Transition.FallOff);
            CutOutMats[m].SetFloat("_CutoutSize", FinalCutOutSize);
        }

        FinalCutOutSize = CutOutSize;
    }

    public IEnumerator CutOutTransition(float duration, float start, float target, bool Active)
    {
        float elapsedTime = 0f;
        float value = start;
        while (elapsedTime <= duration)
        {
            value = Mathf.SmoothStep(start, target, elapsedTime / duration);
            foreach (Material Mat in CutOutMats)
            {
                Vector2 cutoutPos = InputManager.MainCamera.WorldToViewportPoint(PointTransform.position);
                Mat.SetVector("_CutoutPos", cutoutPos);
                Mat.SetFloat("_CutoutSize", value);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (Material Mat in CutOutMats)
        {
            Vector2 cutoutPos = InputManager.MainCamera.WorldToViewportPoint(PointTransform.position);
            Mat.SetVector("_CutoutPos", cutoutPos);
            Mat.SetFloat("_CutoutSize", target);
        }

        if (Active)
        {
            CanUpdate = true;
        }
        if(!Active)
        {
            CanUpdate = false;
        }


    }

}
