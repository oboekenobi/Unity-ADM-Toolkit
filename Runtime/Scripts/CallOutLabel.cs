using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using geniikw.DataRenderer2D;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

[ExecuteInEditMode]
public class CallOutLabel : MonoBehaviour
{
    #region Dependencies
    [Space(10)]
    [Header("Dependencies")]
    [Space(10)]
    [HideInInspector]
    public GameObject LabelCanvas;
    public RectTransform LabelCanvasRect;
    public Effects_Manager transition;
    public GameObject LabelParent;
    public Image LabelImage;
    public CanvasGroup ParentGroup;
    public LabelFitter Fitter;
    public RectTransform Area;
    //[HideInInspector]
    public Camera MainCamera;
    [HideInInspector]
    public ProjectManager manager;
    [HideInInspector]
    public RectTransform Tag;
    public RectTransform ChildTag;
    public RectTransform TagBackground;
    public GameObject TagRect;
    public GameObject HighlightObject;

    public GameObject LabelPointPrefab;
    public GameObject CirclePrefab;
    public GameObject LinePrefab;
    public TextMeshProUGUI labelText;
    [Tooltip("Updates the positioning of the lines, recommended if Callout is non-Static")]
    public bool UpdatePositions = true;
    #endregion

    #region Settings Values
    [Space(10)]
    [Header("Label Text")]
    [Space(10)]
    public string LabelText;

    [Space(10)]
    [Header("Line Settings")]
    [Space(10)]

    public bool staticLinePlacement;
    [Tooltip("Change the style of the label's line")]
    public CalloutLineStyle LineStyle = CalloutLineStyle.Linear;
    [HideInInspector]
    public float TransitionDuration = 0.5f;
    //[HideInInspector]
    public PlacementDirection LinePlacement;
    [Space (10)]
    [Header("Stepped Line Settings")]
    [Space(10)]
    public bool AddStep;
    [Range(-300, 0)]
    public float LineStepTrail = 0.5f;
    public bool NegativeDirection;
    [Space(10)]
    [Header("Angular Line Settings")]
    [Space(10)]
    [Range(0, 1f)]
    public float LineAngleValue = 0;

    public List<float> CircleSizes;
    #endregion

    #region Highlighter Settings
    [Space(10)]
    [Header("Highlighting Options")]
    [Space(10)]
    public bool PointAdded;
    public bool StepPointAdded;
    [HideInInspector]
    public bool Init;
    [HideInInspector]
    public bool HighlightInit;
    #endregion

    #region Lists & Arrays
    public enum CalloutLineStyle { Linear, Stepped, Angled};
    public enum PlacementDirection {Left, Right, Bottom, Top, Middle};
    
    //[HideInInspector]
    public List<GameObject> Lines;
    //[HideInInspector]
    public List<UILine> UILines;
    //[HideInInspector]
    public List<RectTransform> Circles;
    //[HideInInspector]
    public List<Material> CircleMaterials;
    //[HideInInspector]
    public List<Transform> InterestPoints = new List<Transform>();
    [HideInInspector]
    public Vector3[] TagCorners = new Vector3[4];
    #endregion


#if UNITY_EDITOR
    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            manager = GameObject.FindFirstObjectByType<ProjectManager>();
            if (!Init)
            {
                Tag = gameObject.GetComponent<RectTransform>();
                LabelCanvas = GameObject.FindWithTag("CallOut Canvas");
                MainCamera = Camera.main;
                Vector3 pointPos = MainCamera.ScreenToWorldPoint(new Vector3(600, 400, 5));
                GameObject labelPoint = PrefabUtility.InstantiatePrefab(LabelPointPrefab) as GameObject;
                labelPoint.transform.position = pointPos;
                LabelPoint point = labelPoint.GetComponent<LabelPoint>();
                point.label = this;
                Init = true;
            }
            LabelCanvasRect = LabelCanvas.GetComponent<RectTransform>();
        }
    }

    private void OnDestroy()
    {
        /*if (manager != null && manager.Sections.Count > 0)
        {
            for (int i = 0; i < manager.Sections[manager.ActiveSectionIndex].CallOutGameObjects.Count; i++)
            {
                if (manager.Sections[manager.ActiveSectionIndex].CallOutGameObjects[i] == LabelParent)
                {
                    manager.Sections[manager.ActiveSectionIndex].CallOutGameObjects.RemoveAt(i);
                    manager.Sections[manager.ActiveSectionIndex].CallOuts.RemoveAt(i);
                }
            }
            if (LabelParent != null)
            {
                manager.GarbageCollection.Add(LabelParent);
            }
        }*/
    }

    private void OnValidate()
    {
        for(int i = 0; i < Lines.Count; i++)
        {
            if (Lines[i] == null)
            {
                Lines.RemoveAt(i);
            }
        }
        for (int i = 0; i < Circles.Count; i++)
        {
            if (Circles[i] == null)
            {
                Circles.RemoveAt(i);
            }
        }

        if(labelText != null)
        {
            labelText.text = LabelText;
            if(LabelText == "")
            {
                labelText.text = "Enter Text";
            }
        }
    }
#endif
    void Awake()
    {
        if (Application.isPlaying)
        {
            //InitializeLabelPositions();
            ProjectManager.LabelHighlited = false;
        }
        if (UpdatePositions)
        {
            lineEnd = new Vector3[Lines.Count];
        }
    }

    void Start()
    {
        /*if (Application.isPlaying)
        {
            for (int i = 0; i < Circles.Count; i++)
            {
                CircleSizes.Add(Circles[i].localScale.x);
            }
        }*/

        TransitionDuration = manager.ActiveSection.CallOutTransitionDuration;
    }

    /// <summary>
    /// used to determine the Callout's placement direction since Callouts may have more than one line pointing to multiple positions
    /// </summary>
    /// <returns>the average position of all the starting points of the lines</returns>
    public Vector3 LineDirectionAverage()
    {
        Vector3 Average = new Vector3();
        Vector3 Sum = new Vector3();
        for (int i = 0; i < Circles.Count; i++)
        {
            if (Circles[i] != null)
            {
                Sum += Circles[i].localPosition;
            }
        }
        Average = (Sum + Tag.localPosition) / (Circles.Count + 1);
        return Average;
    }
    
    public void UpdateLineCount()
    {
        if (InterestPoints.Count > 0 && InterestPoints.Count > Lines.Count)
        {
            //Initialize LineRenderer Prefabs

            //GameObject Line = PrefabUtility.InstantiatePrefab(LinePrefab) as GameObject;
            GameObject Line = new GameObject();
            Line.transform.parent = LabelParent.transform;
            Lines.Add(Line);
            UILine line = Line.GetComponent<UILine>();
            if (PointAdded)
            {
                float width = UILines[0].line.points[0].width;
                line.line.points.Add(new Point(Vector3.zero, Vector3.zero, Vector3.zero, width));
                if (StepPointAdded)
                {
                    line.line.points.Add(new Point(Vector3.zero, Vector3.zero, Vector3.zero, width));
                }
            }
            UILines.Add(line);

            //Initialize CircleRenderer Prefabs
            //GameObject Circle = PrefabUtility.InstantiatePrefab(CirclePrefab) as GameObject;
            GameObject Circle = new GameObject();
            Circle.transform.parent = (LabelParent.transform);
            Circles.Add(Circle.GetComponent<RectTransform>());
        }

        if (!PointAdded && LineStyle != CalloutLineStyle.Linear)
        {
            float width = UILines[0].line.points[0].width;
            foreach (UILine line in UILines)
            {
                line.line.points.Add(new Point(Vector3.zero, Vector3.zero, Vector3.zero, width));
                PointAdded = true;
                if (AddStep && LineStyle == CalloutLineStyle.Stepped && !StepPointAdded)
                {
                    line.line.points.Add(new Point(Vector3.zero, Vector3.zero, Vector3.zero, width));
                    StepPointAdded = true;
                }
            }
        }
        if (PointAdded && !AddStep && LineStyle == CalloutLineStyle.Stepped)
        {
            foreach (UILine line in UILines)
            {
                if (StepPointAdded)
                {
                    line.line.points.RemoveAt(2);
                }
            }
            StepPointAdded = false;
        }
        if (PointAdded && AddStep && LineStyle == CalloutLineStyle.Stepped)
        {
            float width = new float();

            if(InterestPoints.Count > 0)
            {
                width = UILines[0].line.points[0].width;
            }

            foreach (UILine line in UILines)
            {
                if (!StepPointAdded && line != null)
                {
                    line.line.points.Add(new Point(Vector3.zero, Vector3.zero, Vector3.zero, width));
                }
            }
            StepPointAdded = true;
        }
        if (PointAdded && LineStyle == CalloutLineStyle.Linear)
        {
            foreach (UILine line in UILines)
            {
                line.line.points.RemoveAt(1);

                if (AddStep && StepPointAdded)
                {
                    line.line.points.RemoveAt(2);
                }
            }
            PointAdded = false;
            StepPointAdded = false;
        }

    }

    public Vector3[] lineEnd;

    //private float AveragedAngle;

    public void UpdateLinePositions()
    {
        //Get Position of Corners of Tag
        TagBackground.GetWorldCorners(TagCorners);
        if (!Application.isPlaying)
        {
            lineEnd = new Vector3[Lines.Count];
        }
        if (UILines.Count == InterestPoints.Count && UILines.Count == Circles.Count)
        {
            //AveragedAngle = GetLineAngle(Lines, LineDirectionAverage());

            for (int i = 0; i < Lines.Count; i++)
            {
                if (Circles[i] != null)
                {
                    //Update Circle Renderer Position
                    Vector3 ScreenPos = WorldToScreenSpace(InterestPoints[i].position, LabelCanvasRect);
                    Circles[i].localPosition = new Vector3(ScreenPos.x, ScreenPos.y, 0);

                    float CircleRadius = (Circles[i].rect.width / 2) * Circles[i].localScale.x;
                    float LineAngle = GetLineAngle(Circles[i].localPosition, Tag.localPosition);
                    float AveragedAngle = GetLineAngle(Tag.localPosition, LineDirectionAverage());
                    Vector3 lineStart = new Vector3();
                    //Vector3 lineEnd = new Vector3();
                    
                    if (LineStyle == CalloutLineStyle.Linear)
                    {
                        Vector3 dir = (UILines[i].line.points[1].position - Circles[i].localPosition).normalized;
                        lineStart = Circles[i].localPosition + (dir * CircleRadius);
                        if (!Application.isPlaying || UpdatePositions)
                        {
                            #region Linear Line Placement 

                            if (LineAngle > 135 && LineAngle < 157.5f)
                            {
                                //0
                                lineEnd[i] = WorldToScreenSpace(TagCorners[0], LabelCanvasRect);
                            }
                            
                            if (LineAngle > 45 && LineAngle < 67.5)
                            {
                                //1
                                lineEnd[i] = WorldToScreenSpace(TagCorners[1], LabelCanvasRect);
                            }

                            if (LineAngle < -45 && LineAngle > -67.5f)
                            {
                                //2
                                lineEnd[i] = WorldToScreenSpace(TagCorners[2], LabelCanvasRect);
                            }

                            if (LineAngle < -135 && LineAngle > -157.5f)
                            {
                                //3
                                lineEnd[i] = WorldToScreenSpace(TagCorners[3], LabelCanvasRect);
                            }

                            if (LineAngle > 157.5 && LineAngle < 180 || LineAngle < -157.5f && LineAngle > -180)
                            {
                                //Bottom
                                lineEnd[i] = WorldToScreenSpace((TagCorners[0] + (TagCorners[3] - TagCorners[0]) / 2), LabelCanvasRect);
                                LinePlacement = PlacementDirection.Bottom;
                            }

                            if (LineAngle < 45f && LineAngle > -45f)
                            {
                                //Top
                                lineEnd[i] = WorldToScreenSpace((TagCorners[1] + (TagCorners[2] - TagCorners[1]) / 2), LabelCanvasRect);
                                LinePlacement = PlacementDirection.Top;
                            }

                            if (LineAngle > 67.5f && LineAngle < 135)
                            {
                                //left
                                lineEnd[i] = WorldToScreenSpace((TagCorners[0] + (TagCorners[1] - TagCorners[0]) / 2),LabelCanvasRect);
                                LinePlacement = PlacementDirection.Left;
                            }

                            if (LineAngle < -67.5f && LineAngle > -135)
                            {
                                //right
                                lineEnd[i] = WorldToScreenSpace((TagCorners[2] + (TagCorners[3] - TagCorners[2]) / 2),LabelCanvasRect);
                                LinePlacement = PlacementDirection.Right;
                            }

                            #endregion
                        }
                    }
                    if (LineStyle == CalloutLineStyle.Angled)
                    {
                        #region Angled Line Placement
                        Vector3 dir = (UILines[i].line.points[1].position - Circles[i].localPosition).normalized;
                        lineStart = Circles[i].localPosition + (dir * CircleRadius);
                        if (!Application.isPlaying || UpdatePositions)
                        {
                            if (LineAngle > 0 && LineAngle < 180)
                            {
                                lineEnd[i] = WorldToScreenSpace(TagCorners[0] + (TagCorners[1] - TagCorners[0]) / 2, LabelCanvasRect);
                            }
                            if (LineAngle < 0 && LineAngle > -180)
                            {
                                lineEnd[i] = WorldToScreenSpace(TagCorners[2] + (TagCorners[3] - TagCorners[2]) / 2, LabelCanvasRect);
                            }
                        }
                        #endregion
                    }
                    if (LineStyle == CalloutLineStyle.Stepped)
                    {
                        #region Stepped Line Placement
                        if (Lines.Count == 1 || Lines.Count > 1 && !AddStep)
                        {
                            if (!NegativeDirection)
                            {
                                Vector3 dir = (UILines[i].line.points[1].position - Circles[i].localPosition).normalized;
                                lineStart = Circles[i].localPosition + (dir * CircleRadius);

                                if (!Application.isPlaying || UpdatePositions)
                                {
                                    if (LineAngle >= 0 && LineAngle < 180)
                                    {
                                        //left
                                        lineEnd[i] = WorldToScreenSpace((TagCorners[0] + (TagCorners[1] - TagCorners[0]) / 2),LabelCanvasRect);
                                        LinePlacement = PlacementDirection.Left;
                                    }
                                    if (LineAngle < 0 && LineAngle >= -180)
                                    {
                                        //right
                                        lineEnd[i] = WorldToScreenSpace((TagCorners[2] + (TagCorners[3] - TagCorners[2]) / 2),LabelCanvasRect);
                                        LinePlacement = PlacementDirection.Right;
                                    }
                                    
                                    if (AddStep && StepPointAdded)
                                    {
                                        Vector3 StepPos = lineEnd[i] + (dir * LineStepTrail);

                                        foreach (UILine line in UILines)
                                        {
                                            line.line.points[2].position = StepPos;
                                        }
                                    }
                                    if (AddStep && StepPointAdded)
                                    {
                                        Vector3 StepPos = lineEnd[i] + (dir * LineStepTrail);

                                        foreach (UILine line in UILines)
                                        {
                                            line.line.points[2].position = StepPos;
                                        }
                                    }
                                }
                                
                            }
                            if (NegativeDirection)
                            {
                                Vector3 dir = (UILines[i].line.points[1].position - Circles[i].localPosition).normalized;
                                lineStart = Circles[i].localPosition + (dir * CircleRadius);

                                if (!Application.isPlaying || UpdatePositions)
                                {
                                    if (LineAngle > 90 && LineAngle < 180 || LineAngle >= -180 && LineAngle < -90)
                                    {
                                        //bottom
                                        lineEnd[i] = WorldToScreenSpace((TagCorners[0] + (TagCorners[3] - TagCorners[0]) / 2), LabelCanvasRect);
                                        LinePlacement = PlacementDirection.Bottom;
                                    }
                                    if (LineAngle >= 0 && LineAngle < 90 || LineAngle < 0 && LineAngle > -90)
                                    {
                                        //top
                                        lineEnd[i] = WorldToScreenSpace((TagCorners[1] + (TagCorners[2] - TagCorners[1]) / 2), LabelCanvasRect);
                                        LinePlacement = PlacementDirection.Top;
                                    }
                                    if (AddStep && StepPointAdded)
                                    {
                                        Vector3 StepPos = lineEnd[i] + (dir * LineStepTrail);

                                        foreach (UILine line in UILines)
                                        {
                                            line.line.points[2].position = StepPos;
                                        }
                                    }
                                }
                                
                            }
                        }

                        if (Lines.Count > 1 && StepPointAdded)
                        {
                            Vector3 dir = (UILines[i].line.points[1].position - Circles[i].localPosition).normalized;
                            lineStart = Circles[i].localPosition + (dir * CircleRadius);
                            
                            if (!Application.isPlaying || UpdatePositions)
                            {
                                if (AveragedAngle >= 0 && AveragedAngle < 45 || AveragedAngle < 0 && AveragedAngle > -45)
                                {
                                    //Bottom
      
                                    lineEnd[i] = WorldToScreenSpace((TagCorners[0] + (TagCorners[3] - TagCorners[0]) / 2), LabelCanvasRect);
                                    LinePlacement = PlacementDirection.Bottom;
                                }
                                if (AveragedAngle > 45 && AveragedAngle < 135)
                                {
                                    //Right
                                    lineEnd[i] = WorldToScreenSpace((TagCorners[2] + (TagCorners[3] - TagCorners[2]) / 2), LabelCanvasRect);
                                    LinePlacement = PlacementDirection.Right;
                                }
                                if (AveragedAngle < -45 && AveragedAngle > -135)
                                {
                                    //Left
                                    lineEnd[i] = WorldToScreenSpace((TagCorners[0] + (TagCorners[1] - TagCorners[0]) / 2), LabelCanvasRect);
                                    LinePlacement = PlacementDirection.Left;
                                }
                                if (AveragedAngle > 135 && AveragedAngle < 180 || AveragedAngle < -135 && AveragedAngle >= -180)
                                {
                                    //Top
                                    lineEnd[i] = WorldToScreenSpace((TagCorners[1] + (TagCorners[2] - TagCorners[1]) / 2), LabelCanvasRect);
                                    LinePlacement = PlacementDirection.Top;
                                }
                                if (AddStep && StepPointAdded)
                                {
                                    Vector3 StepPos = lineEnd[i] + (dir * LineStepTrail);
                                    foreach (UILine line in UILines)
                                    {
                                        line.line.points[2].position = StepPos;
                                    }
                                }
                            }
                        }
                        #endregion
                    }

                    if (!UpdatePositions)
                    {
                        #region Averaged Line Placement

                        if (AveragedAngle > 0 && AveragedAngle < 45 || AveragedAngle < 0 && AveragedAngle > -45)
                        {
                            //Bottom
                            LinePlacement = PlacementDirection.Bottom;
                        }
                        if (AveragedAngle > 45 && AveragedAngle < 135)
                        {
                            //Right
                            LinePlacement = PlacementDirection.Right;
                        }
                        if (AveragedAngle < -45 && AveragedAngle > -135)
                        {
                            //Left
                            LinePlacement = PlacementDirection.Left;
                        }
                        if (AveragedAngle > 135 && AveragedAngle < 180 || AveragedAngle < -135 && AveragedAngle > -180)
                        {
                            //Top
                            LinePlacement = PlacementDirection.Top;
                        }

                        #endregion
                    }

                    UILines[i].GeometyUpdateFlagUp();
                    UILines[i].line.points[0].position = lineStart;
                    //UILines[i].line.points.Last().position = lineEnd;
                    UILines[i].line.points.Last().position = lineEnd[i];
                }
                
            }
        }
    }

    public float GetLineAngle(Vector3 StartPoint, Vector3 EndPoint)
    {
        Vector3 Dir = StartPoint - EndPoint;
        float LineAngle = Vector2.Angle(Dir, transform.up);
        Vector2 Cross = new Vector2(Dir.y, -Dir.x);
        if (Cross.y < 0)
        {
            LineAngle = -LineAngle;
        }
        return LineAngle;
    }

    /// <summary>
    /// Updates the size of a circle sprite shape at the beginning of a line relative to the camera's zoom
    /// </summary>
    public void LockCircleSize()
    {
        for(int i = 0; i < Circles.Count ; i++)
        {
            /*float UpdatedCameraDistance = Vector3.Distance(MainCamera.transform.position, InterestPoints[i].position);
            float DistanceChange = CameraMovement.StartCameraDistance - UpdatedCameraDistance;
            float SizeScale = CircleSizes[i] + DistanceChange;
            Circles[i].localScale = new Vector3(SizeScale, SizeScale, SizeScale);*/
        }
    }

    public void SteppedLineUpdate()
    {
    
        if (PointAdded && LineStyle == CalloutLineStyle.Stepped)
        {
           for(int i = 0; i < UILines.Count; i++)
           {
                if (Circles[i] != null)
                {
                    //Vector3 MidPoint = new Vector3();
                    if (NegativeDirection)
                    {

                        if (AddStep)
                        {
                            UILines[i].line.points[1].position = new Vector3(Circles[i].localPosition.x, UILines[i].line.points[2].position.y, 0);
                        }
                        else
                        {
                            UILines[i].line.points[1].position = new Vector3(Tag.localPosition.x, Circles[i].localPosition.y, 0);
                        }
                    }
                    else
                    {
                        if (AddStep)
                        {
                            UILines[i].line.points[1].position = new Vector3(Circles[i].localPosition.x, UILines[i].line.points[2].position.y, 0);
                        }
                        else
                        {
                            UILines[i].line.points[1].position = new Vector3(Circles[i].localPosition.x, Tag.localPosition.y, 0);
                        }
                    }
                }
                
            }
        }
    }

    public void AngledLineUdpdate()
    {
        if (LineStyle == CalloutLineStyle.Angled && PointAdded)
        {
            float trailVal = MainCamera.ViewportToScreenPoint(new Vector3(LineAngleValue, LineAngleValue, LineAngleValue)).x;
            foreach(UILine line in UILines)
            {
                Vector3 MidPoint = new Vector3(trailVal, Tag.localPosition.y, 0);

                line.line.points[1].position = MidPoint;
            }
        }

    }

    public GameObject highlightedObject;
    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (InterestPoints.Count > 0)
            {
                for (int i = 0; i < Circles.Count; i++)
                {
                    if (Circles[i] != null)
                    {
                        Debug.DrawLine(Circles[i].position, InterestPoints[i].position, Color.gray);
                    }
                }
            }
            UpdateLineCount();
            //InitHightlightObj();
            UpdateLinePositions();
            SteppedLineUpdate();
            AngledLineUdpdate();
            
        }
#endif

        

        if (!manager.StaticPresentation && Application.isPlaying)
        {
            UpdateLinePositions();
            SteppedLineUpdate();
            AngledLineUdpdate();
        }
    }
   /* void FixedUpdate()
    {
        if(MainCamera != null)
        {
            if (!manager.StaticPresentation && Application.isPlaying)
            {
                UpdateLinePositions();
                SteppedLineUpdate();
                AngledLineUdpdate();
            }
        }
    }*/
    public bool HasOpened;
    #region Transition Functions
    public IEnumerator LineTransition(float Start, float End)
    {
        float elapsedTime = 0;
        while(elapsedTime <= (TransitionDuration/2))
        {
            for (int i = 0; i < UILines.Count; i++)
            {
               UILines[i].line.option.endRatio = Mathf.SmoothStep(Start, End, elapsedTime/(TransitionDuration/2));
               if (manager.StaticPresentation)
               {
                  UILines[i].GeometyUpdateFlagUp();
               }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < UILines.Count; i++)
        {
            UILines[i].line.option.endRatio = End;
            UILines[i].GeometyUpdateFlagUp();
        }
    }

    public void InitializeLabelPositions()
    {
        TagHeight = (Vector3.Distance(WorldToScreenSpace(TagCorners[0], LabelCanvasRect), WorldToScreenSpace(TagCorners[1], LabelCanvasRect)) / 2);
        TagWidth = (Vector3.Distance(WorldToScreenSpace(TagCorners[1], LabelCanvasRect), WorldToScreenSpace(TagCorners[2], LabelCanvasRect)) / 2);

        /*if (LinePlacement == PlacementDirection.Left)
        {
            Tag.localScale = new Vector3(0, Tag.localScale.y, Tag.localScale.z);
            Tag.localPosition = Tag.localPosition + (Vector3.left * TagWidth);
        }

        if (LinePlacement == PlacementDirection.Right)
        {
            Tag.localPosition = Tag.localPosition + (Vector3.right * TagWidth);
            Tag.localScale = new Vector3(0, Tag.localScale.y, Tag.localScale.z);
        }
        if (LinePlacement == PlacementDirection.Bottom)
        {
            Tag.localPosition = Tag.localPosition + (Vector3.down * TagHeight);
            Tag.localScale = new Vector3(Tag.localScale.x, 0, Tag.localScale.z);
        }
        if (LinePlacement == PlacementDirection.Top)
        {
            Tag.localPosition = Tag.localPosition + (Vector3.up * TagHeight);
            Tag.localScale = new Vector3(Tag.localScale.x, 0, Tag.localScale.z);
        }
        if (LinePlacement == PlacementDirection.Middle)
        {
            Tag.localScale = new Vector3(0, Tag.localScale.y, Tag.localScale.z);
        }*/

        if (LinePlacement == PlacementDirection.Left)
        {
            ChildTag.localScale = new Vector3(0, 1, 1);
            ChildTag.localPosition = (Vector3.left * TagWidth);
        }

        if (LinePlacement == PlacementDirection.Right)
        {
            ChildTag.localPosition = Tag.localPosition + (Vector3.right * TagWidth);
            ChildTag.localScale = new Vector3(0, 1, 1);
        }
        if (LinePlacement == PlacementDirection.Bottom)
        {
            ChildTag.localPosition = (Vector3.down * TagHeight);
            ChildTag.localScale = new Vector3(0, 1, 1);
        }
        if (LinePlacement == PlacementDirection.Top)
        {
            ChildTag.localPosition = (Vector3.up * TagHeight);
            ChildTag.localScale = new Vector3(0, 1, 1);
        }
        if (LinePlacement == PlacementDirection.Middle)
        {
            ChildTag.localScale = new Vector3(0, 1, 1);
        }

    }
    public IEnumerator LabelTransition(float TagStart, float TagEnd, float LineStart, float LineEnd, bool Open)
    {
        if (Open)
        {
            yield return LineTransition(LineStart, LineEnd);
        }

        if (LinePlacement == PlacementDirection.Left)
        {
            float elapsedTime = 0;
            Vector3 StartScale = new Vector3();
            Vector3 StartPos = new Vector3();
            Vector3 TargetScale = new Vector3();
            Vector3 TargetPos = new Vector3();
            if (Open)
            {
                StartPos = Tag.localPosition;
                StartScale = new Vector3(TagStart, TagEnd, TagEnd);
                TargetScale = new Vector3(TagEnd, TagEnd, TagEnd);
                TargetPos = (StartPos + (Vector3.right * TagWidth));
            }
            else
            {
                StartPos = Tag.localPosition;
                StartScale = new Vector3(TagStart, TagStart, TagStart);
                TargetScale = new Vector3(TagEnd, TagStart, TagStart);
                TargetPos = (StartPos + (Vector3.left * TagWidth));
            }
            Tag.localScale = StartScale;
            Tag.localPosition = StartPos;
            while (elapsedTime <= (TransitionDuration / 2))
            {
                Tag.localPosition = Vector3.Lerp(StartPos, TargetPos, elapsedTime / (TransitionDuration / 2));
                Tag.localScale = Vector3.Lerp(StartScale, TargetScale, elapsedTime / (TransitionDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Tag.localScale = TargetScale;
            Tag.localPosition = TargetPos;
        }
        if (LinePlacement == PlacementDirection.Right)
        {
            float elapsedTime = 0;
            Vector3 StartScale = new Vector3();
            Vector3 StartPos = new Vector3();
            Vector3 TargetScale = new Vector3();
            Vector3 TargetPos = new Vector3();
            if (Open)
            {
                StartPos = Tag.localPosition;
                StartScale = new Vector3(TagStart, TagEnd, TagEnd);
                TargetScale = new Vector3(TagEnd, TagEnd, TagEnd);
                TargetPos = (StartPos + (Vector3.left * TagWidth));
            }
            else
            {
                StartPos = Tag.localPosition;
                TargetPos = (StartPos + (Vector3.right * TagWidth));
                StartScale = new Vector3(TagStart, TagStart, TagStart);
                TargetScale = new Vector3(TagEnd, TagStart, TagStart);
            }
            Tag.localScale = StartScale;
            Tag.localPosition = StartPos;
            while (elapsedTime <= (TransitionDuration / 2))
            {
                Tag.localPosition = Vector3.Lerp(StartPos, TargetPos, elapsedTime / (TransitionDuration / 2));
                Tag.localScale = Vector3.Lerp(StartScale, TargetScale, elapsedTime / (TransitionDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Tag.localScale = TargetScale;
            Tag.localPosition = TargetPos;
        }
        if (LinePlacement == PlacementDirection.Bottom)
        {
            float elapsedTime = 0;
            Vector3 StartScale = new Vector3();
            Vector3 StartPos = new Vector3();
            Vector3 TargetScale = new Vector3();
            Vector3 TargetPos = new Vector3();
            if (Open)
            {
/*                if (!HasOpened)
                {
                    StartPos = Tag.localPosition + (Vector3.down * TagHeight);
                    HasOpened = true;
                }
                else
                {
                    StartPos = Tag.localPosition;
                }*/
                StartPos = Tag.localPosition;
                TargetPos = StartPos + (Vector3.up * TagHeight);
                TargetScale = new Vector3(TagEnd, TagEnd, TagEnd);
                StartScale = new Vector3(TagEnd, TagStart, TagEnd);
            }
            else
            {
                StartScale = new Vector3(TagStart, TagStart, TagStart);
                TargetScale = new Vector3(TagStart, TagEnd, TagStart);
                StartPos = Tag.localPosition;
                TargetPos = Tag.localPosition + (Vector3.down * TagHeight);
            }
            Tag.localScale = StartScale;
            Tag.localPosition = StartPos;
            while (elapsedTime <= (TransitionDuration / 2))
            {
                Tag.localPosition = Vector3.Lerp(StartPos, TargetPos, elapsedTime / (TransitionDuration / 2));
                Tag.localScale = Vector3.Lerp(StartScale, TargetScale, elapsedTime / (TransitionDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Tag.localScale = TargetScale;
            Tag.localPosition = TargetPos;
        }
        if (LinePlacement == PlacementDirection.Top)
        {
            float elapsedTime = 0;
            Vector3 StartScale = new Vector3();
            Vector3 StartPos = new Vector3();
            Vector3 TargetScale = new Vector3();
            Vector3 TargetPos = new Vector3();
            if (Open)
            {
/*                if (!HasOpened)
                {
                    StartPos = Tag.localPosition + (Vector3.up * TagHeight);
                    HasOpened = true;
                }
                else
                {
                    StartPos = Tag.localPosition;
                }*/
                StartPos = Tag.localPosition;
                TargetPos = StartPos + (Vector3.down * TagHeight);
                TargetScale = new Vector3(TagEnd, TagEnd, TagEnd);
                StartScale = new Vector3(TagEnd, TagStart, TagEnd);
            }
            else
            {
                StartPos = Tag.localPosition;
                TargetPos = StartPos + (Vector3.up * TagHeight);
                StartScale = new Vector3(TagStart, TagStart, TagStart);
                TargetScale = new Vector3(TagStart, TagEnd, TagStart);
            }
            Tag.localScale = StartScale;
            Tag.localPosition = StartPos;
            while (elapsedTime <= (TransitionDuration / 2))
            {
                Tag.localPosition = Vector3.Lerp(StartPos, TargetPos, elapsedTime / (TransitionDuration / 2));
                Tag.localScale = Vector3.Lerp(StartScale, TargetScale, elapsedTime / (TransitionDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Tag.localScale = TargetScale;
            Tag.localPosition = TargetPos;
        }
        if (LinePlacement == PlacementDirection.Middle)
        {
            float elapsedTime = 0;
            Vector3 StartScale = new Vector3();
            Vector3 TargetScale = new Vector3();
            if (Open)
            {
                TargetScale = new Vector3(TagEnd, TagEnd, TagEnd);
                StartScale = new Vector3(TagEnd, TagStart, TagEnd);
            }
            else
            {
                StartScale = new Vector3(TagStart, TagStart, TagStart);
                TargetScale = new Vector3(TagStart, TagEnd, TagStart);
            }
            Tag.localScale = StartScale;
            while (elapsedTime <= (TransitionDuration / 2))
            {
                Tag.localScale = Vector3.Lerp(StartScale, TargetScale, elapsedTime / (TransitionDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Tag.localScale = TargetScale;
        }

        if (!Open)
        {
            StartCoroutine(LineTransition(LineStart, LineEnd));
        }
    }

    public float TagWidth;
    public float TagHeight;
    public void OpenLabel()
    {

        StartCoroutine(LabelTransition(0, Tag.localScale.z, 0, 1, true));
    }
    public void CloseLabel()
    {
        TagHeight = (Vector3.Distance(WorldToScreenSpace(TagCorners[0], LabelCanvasRect), WorldToScreenSpace(TagCorners[1], LabelCanvasRect)) / 2);
        TagWidth = (Vector3.Distance(WorldToScreenSpace(TagCorners[1], LabelCanvasRect), WorldToScreenSpace(TagCorners[2], LabelCanvasRect)) / 2);
        StartCoroutine(LabelTransition(Tag.localScale.z, 0, 1, 0, false));
    }

    public float HighlightDuration = 0.3f;
    public IEnumerator TextHighlight(float start, float end, float pixelStart, float pixelEnd, float paddingStart, float paddingEnd, bool Active) 
    {
        float elapsedTime = 0f;
        while(elapsedTime <= HighlightDuration)
        {
            Fitter.Padding = Mathf.SmoothStep(paddingStart, paddingEnd, elapsedTime/HighlightDuration);
            LabelImage.pixelsPerUnitMultiplier = Mathf.SmoothStep(pixelStart, pixelEnd, elapsedTime/HighlightDuration);
            foreach(CallOutLabel label in manager.ActiveSection.CallOuts)
            {
                if(label != this)
                {
                    label.ParentGroup.alpha = Mathf.SmoothStep(start, end, elapsedTime / HighlightDuration);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (Active)
        {
            canClose = true;
            ProjectManager.LabelHighlited = true;
        }
        if (!Active)
        {
            hasInit = false;
            ProjectManager.LabelHighlited = false;
        }
        Fitter.Padding = paddingEnd;
        LabelImage.pixelsPerUnitMultiplier = pixelEnd;
        foreach (CallOutLabel label in manager.ActiveSection.CallOuts)
        {
            if(label != this)
            {
                label.ParentGroup.alpha = end;
            }
        }
    }
    public bool isNotInLabel;
    public bool canClose;
    public bool hasInit;
    #endregion
    public Vector3 WorldToScreenSpace(Vector3 worldPos, RectTransform nu)
    {
        Vector3 screenPoint = MainCamera.WorldToScreenPoint(worldPos);
        screenPoint.z = 0;

        Vector2 screenPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(LabelCanvasRect, screenPoint, MainCamera, out screenPos))
        {
            return screenPos;
            
        }

        return screenPoint;

        //return (RectTransformUtility.ScreenPointToLocalPointInRectangle(LabelCanvasRect, screenPoint, MainCamera, out screenPos)
    }

}
