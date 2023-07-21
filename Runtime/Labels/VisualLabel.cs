using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using geniikw.DataRenderer2D;

[ExecuteInEditMode]
public class VisualLabel : MonoBehaviour
{
    public RawImage VisualImage;
    public RectTransform ImageRect;
    public CanvasGroup ImageCanvasGroup;
    public Transform InterestPoint;
    public LabelPoint WorldSpacePoint;
    public Camera MainCamera;
    public ProjectManager manager;
    

    public bool HidePointer;
    public bool isFolding;
    public float Duration;

    public float StartCircleSize;
    public float StartCircleWidth;
    public float StartAngle;
    public float StartingMagnitude;
    public float LabelMagnitude;
    public float LineAngle;

    public RectTransform focalCircle;
    public Material focalCircleMaterial;
    public Sprite focalCircleMask;
    public Shader focalCricleShader;

    public Vector3[] CircleRTCorners;
    public Vector3[] ImageRTCorners;
    public Vector3 TagPoint;
    public Vector3[] PerpendicularPoints = new Vector3[2];
    public List<UILine> LineRenderers;

    public bool Initialized = false;
    public bool isAdded = true;

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (!Initialized)
        {
            manager = GameObject.FindWithTag("SceneManager").GetComponent<ProjectManager>();
            gameObject.transform.parent = GameObject.FindWithTag("CallOut Canvas").transform;
            CircleRenderer();
            CreateLineRenderers();
            CircleRTCorners = new Vector3[4];
            ImageRTCorners = new Vector3[4];
            manager.Sections[manager.ActiveSectionIndex].VisualLabels.Add(this);
            Initialized = true;
        }
        
    }

    private void OnDestroy()
    {
        if(manager != null)
        {
            foreach (PresentationSection section in manager.Sections)
            {
                for (int i = 0; i < section.CallOutGameObjects.Count; i++)
                {
                    if (section.CallOutGameObjects[i] == gameObject)
                    {
                        manager.Sections[manager.ActiveSectionIndex].CallOutGameObjects.RemoveAt(i);
                    }
                }
            }
            DestroyImmediate(InterestPoint.gameObject);
        }
    }
#endif

    // Start is called before the first frame update
    void Awake()
    {
        StartCircleSize = focalCircle.localScale.x;
        MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR

#endif
        if (!Application.isPlaying)
        {
            Vector3 screenSpacePos = MainCamera.WorldToScreenPoint(InterestPoint.position);
            Vector3 CirclePos = new Vector3(screenSpacePos.x, screenSpacePos.y, 0);
            focalCircle.transform.localPosition = CirclePos;
            CalculateCircumference();
            GetImagePoints();
            Drawlines();
            UpdateLinePlacements();
            UpdatedLerpValues();
        }
        else if(!manager.StaticPresentation)
        {
            Vector3 screenSpacePos = MainCamera.WorldToScreenPoint(InterestPoint.position);
            Vector3 CirclePos = new Vector3(screenSpacePos.x, screenSpacePos.y, 0);
            focalCircle.transform.localPosition = CirclePos;
            CalculateCircumference();
            GetImagePoints();
            Drawlines();
            UpdateLinePlacements();
            UpdatedLerpValues();
        }

        if (Application.isPlaying)
        {
            //LockCircleSize();
        }
    }

    public Vector3 D;
    //public TextMeshProUGUI debugNumber;
    public void CalculateCircumference()
    {
        //Get The Corners of the Cicle's RectTransform and convert them into screenspace points
        focalCircle.GetWorldCorners(CircleRTCorners);
        float CircleRadius = (focalCircle.rect.width / 2) * focalCircle.localScale.x;
        
        Vector3 DirectionToTag = (ImageRect.localPosition - focalCircle.localPosition).normalized;
        Vector3 dir = new Vector3(Mathf.Abs(DirectionToTag.x), DirectionToTag.y, Mathf.Abs(DirectionToTag.z));
        Vector3 PerpindicularDirection = new Vector3(DirectionToTag.y, -DirectionToTag.x, 0);
        D = DirectionToTag;

        PerpendicularPoints[0] = focalCircle.localPosition + (PerpindicularDirection * CircleRadius);
        PerpendicularPoints[1] = focalCircle.localPosition - (PerpindicularDirection * CircleRadius);
        TagPoint = focalCircle.position + (DirectionToTag * CircleRadius);
    }
    public void GetImagePoints()
    {
        ImageRect.GetWorldCorners(ImageRTCorners);
    }

    private static float UnwrapAngle(float angle)
    {
        //This Function Takes any eulerAngle in an Atan over 360 degrees and clamps it to a value within 360 degrees 
        if (angle >= 0)
            return angle;
        angle = -angle % 360;
        return 360 - angle;
    }

    public void UpdatedLerpValues()
    {
        if (Application.isPlaying)
        {

        }
        if (!Application.isPlaying)
        {
            StartCircleSize = focalCircle.transform.localScale.x;
        }
    }
    public float GetLineAngle(Vector3 StartPoint, Vector3 EndPoint)
    {
        Vector3 Dir = StartPoint - EndPoint;
        float LineAngle = Vector2.Angle(Dir, transform.up);
        Vector2 Cross = new Vector2(Dir.y, -Dir.x);
        if(Cross.y < 0)
        {
            LineAngle = -LineAngle;
        }
        return LineAngle;
    }

    public void UpdateLinePlacements()
    {

        float LineAngle1Corner1 = GetLineAngle(LineRenderers[1].line.points[0].position, MainCamera.WorldToScreenPoint(ImageRTCorners[1]));
        float LineAngle1Corner2 = GetLineAngle(LineRenderers[1].line.points[0].position, MainCamera.WorldToScreenPoint(ImageRTCorners[2]));
        float LineAngle1Corner3 = GetLineAngle(LineRenderers[1].line.points[0].position, MainCamera.WorldToScreenPoint(ImageRTCorners[3]));

        float LineAngle2Corner0 = GetLineAngle(LineRenderers[0].line.points[0].position, MainCamera.WorldToScreenPoint(ImageRTCorners[0]));
        float LineAngle2Corner1 = GetLineAngle(LineRenderers[0].line.points[0].position, MainCamera.WorldToScreenPoint(ImageRTCorners[1]));
        float LineAngle2Corner2 = GetLineAngle(LineRenderers[0].line.points[0].position, MainCamera.WorldToScreenPoint(ImageRTCorners[2]));
        float LineAngle2Corner3 = GetLineAngle(LineRenderers[0].line.points[0].position, MainCamera.WorldToScreenPoint(ImageRTCorners[3]));


        //debugNumber.text = LineAngle2Corner3.ToString();

        //For Line Renderer One
        if(LineAngle1Corner1 < -90 && LineAngle1Corner1 > -180)
        {
            LineRenderers[1].line.points[1].position = MainCamera.WorldToScreenPoint(ImageRTCorners[0]);
        }
        if (LineAngle1Corner1 > 90 && LineAngle1Corner1 < 180)
        {
            LineRenderers[1].line.points[1].position = MainCamera.WorldToScreenPoint(ImageRTCorners[1]);
        }
        if (LineAngle1Corner1 < 90 && LineAngle1Corner1 > 0 || LineAngle1Corner2 > 0 && LineAngle1Corner2 < 90)
        {
            LineRenderers[1].line.points[1].position = MainCamera.WorldToScreenPoint(ImageRTCorners[2]);
        }
        if (LineAngle1Corner3 < 0 && LineAngle1Corner3 > -90)
        {
            LineRenderers[1].line.points[1].position = MainCamera.WorldToScreenPoint(ImageRTCorners[3]);
        }

        //For Line Renderer Two

        if(LineAngle2Corner3 > 90 && LineAngle2Corner3 < 180)
        {
            LineRenderers[0].line.points[1].position = MainCamera.WorldToScreenPoint(ImageRTCorners[3]);
        }
        if(LineAngle2Corner3 < -90 && LineAngle2Corner3 > -180)
        {
            LineRenderers[0].line.points[1].position = MainCamera.WorldToScreenPoint(ImageRTCorners[2]);
        }
        if (LineAngle2Corner3 > 0 && LineAngle2Corner3 < 90)
        {
            LineRenderers[0].line.points[1].position = MainCamera.WorldToScreenPoint(ImageRTCorners[0]);
        }

        if(LineAngle2Corner0 < 0 && LineAngle2Corner0 > -90)
        {
            LineRenderers[0].line.points[1].position = MainCamera.WorldToScreenPoint(ImageRTCorners[1]);
        }

        if (LineAngle2Corner2 < -90 && LineAngle2Corner2 > -180)
        {
            LineRenderers[0].line.points[1].position = MainCamera.WorldToScreenPoint(ImageRTCorners[2]);
        }
    }
    public void Drawlines()
    {
        if(LineRenderers.Count >= 2)
        {
            LineRenderers[0].line.points[0].position = PerpendicularPoints[0];
            LineRenderers[1].line.points[0].position = PerpendicularPoints[1];
            LineRenderers[0].GeometyUpdateFlagUp();
            LineRenderers[1].GeometyUpdateFlagUp();
        }
        Debug.DrawLine(focalCircle.transform.position, InterestPoint.position, Color.gray);
    }

    public void LockCircleSize()
    {
        float UpdatedCameraDistance = Vector3.Distance(MainCamera.transform.position, InterestPoint.position);
        //float DistanceChange = CameraMovement.StartCameraDistance - UpdatedCameraDistance;
        //float SizeScale = StartCircleSize + DistanceChange;
        //focalCircle.localScale = new Vector3(SizeScale, SizeScale, SizeScale);
    }

    public void CreateLineRenderers()
    {
        for(int i = 0; i < PerpendicularPoints.Length; i++)
        {
            /*GameObject Line = new GameObject();
            Line.transform.parent = gameObject.transform;
            UILine line =  Line.AddComponent<UILine>();
            line.line.points.Add(new Point());
            line.color = Color.black;
            line.line.option.color = new Gradient();
            line.line.option.endRatio = 1;
            line.line.option.DivideLength = 1;
            line.line.option.DivideAngle = 10;*/
            //UILine.CreateLine();
            GameObject line = Instantiate(Resources.Load<GameObject>("PreFabs/line"));
            line.transform.parent = gameObject.transform;
            RectTransform lineRect = line.GetComponent<RectTransform>();
            lineRect.anchorMax = new Vector2(0, 0);
            lineRect.anchorMin = new Vector2(0, 0);
            lineRect.localScale = Vector3.one;
            lineRect.localEulerAngles = Vector3.zero;
            UILine Line = line.GetComponent<UILine>();
            LineRenderers.Add(Line);
        }
    }
    public void CircleRenderer()
    {
        GameObject circle = Instantiate(Resources.Load<GameObject>("PreFabs/Circle"));
        focalCircle = circle.GetComponent<RectTransform>();
        circle.transform.parent = gameObject.transform;
        focalCircle.anchorMax = new Vector2(0,0);
        focalCircle.anchorMin = new Vector2(0,0);
        focalCircle.localScale = new Vector3(200, 500, 0);
        circle.transform.localScale = Vector3.one;
        circle.transform.localEulerAngles = Vector3.zero;
    }

    #region Transition Functions

    public IEnumerator TagAnimation(float Start, float End, float StartRadius, float EndRadius)
    {
        isFolding = true;
        float elapsedTime = 0f;
        Material circle = focalCircle.GetComponent<Image>().material;
        while (elapsedTime <= Duration)
        {
            LineRenderers[0].line.option.endRatio = Mathf.Lerp(Start, End, elapsedTime/Duration);
            LineRenderers[1].line.option.endRatio = Mathf.Lerp(Start, End, elapsedTime / Duration);
            if (manager.StaticPresentation)
            {
                LineRenderers[0].GeometyUpdateFlagUp();
                LineRenderers[1].GeometyUpdateFlagUp();
            }
            ImageCanvasGroup.alpha = Mathf.Lerp(Start, End, elapsedTime / Duration);
            circle.SetFloat("_Radius", Mathf.Lerp(StartRadius, EndRadius, elapsedTime/Duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        LineRenderers[0].line.option.endRatio = End;
        LineRenderers[1].line.option.endRatio = End;
        ImageCanvasGroup.alpha =End;

        isFolding = false;
    }
    
    public void OpenLabel()
    {
        StartCoroutine(TagAnimation(0, 1, 0, 0.047f));
    }

    public void CloseLabel()
    {
        StartCoroutine(TagAnimation(1, 0, 0.047f, 0));
    }
    #endregion
}
