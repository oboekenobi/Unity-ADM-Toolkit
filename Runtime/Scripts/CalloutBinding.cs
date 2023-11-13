
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


[ExecuteInEditMode]
public class CalloutBinding : MonoBehaviour
{
    [SerializeField]
    public PresentationSection Section;

    public string QueryName = "";
    public string CalloutText;

    private ProjectManager projectManager;

    public CalloutManager calloutContainer;

    [Tooltip("Indicates if the label should have a minimum length it can be from the label point or if it should be set dynamically through the UI Canvas position")]
    public enum TetherLengthType {ViewportSpace, WorldSpace};

    public TetherLengthType CalloutPositioning  = TetherLengthType.WorldSpace;
    
    public VisualElement m_CalloutLabel;

    public float maxOcclusionDistance;

    [Range(0,1)]
    public float minOcclusionOpacity;

    public bool OccludeCallout;
    private bool CollisionDetected;
    private bool AddedToSection;

    //[Range(0,20)]
    public float TetherLength;

    [HideInInspector]
    public float TetherLengthIntensity;

    [Range (0,1)]
    public float CalloutAngle;

    public Color Color;

    public float lineThickeness;

    [Range(0, 100)]
    public float CalloutMargin;

    private float CalloutXPosition;

    private float CalloutYPosition;





    // Start is called before the first frame update

    private void Start()
    {
        if (projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
        }

        float max = 100 - CalloutMargin;
        float min = 0 + CalloutMargin;

        float positiveXClamp = Map(min, max, 0, 0.25f, max, min, 0.5f, 0.75f, CalloutAngle);
        float positiveYClamp = Map(min, max, 0.25f, 0.5f, max, min, 0.75f, 1, CalloutAngle);

        CalloutXPosition = Mathf.Clamp(positiveXClamp, min, max);
        CalloutYPosition = Mathf.Clamp(positiveYClamp, min, max);
        UpdateLine();
    }
    private void UpdateLine()
    {
        calloutContainer = Section.CalloutCanvasDocument.rootVisualElement.Q<CalloutManager>(QueryName);

        if (calloutContainer != null)
        {
            calloutContainer.m_labelPoint = calloutContainer.Q<VisualElement>("LabelPoint");
            calloutContainer.m_label = calloutContainer.Q<VisualElement>("Label");
            calloutContainer.m_text = calloutContainer.Q<Label>("Text");

            calloutContainer.m_text.text = CalloutText;
            calloutContainer.thickness = lineThickeness;

            calloutContainer.LineColor = Color;
            calloutContainer.m_text.style.backgroundColor = Color;
            calloutContainer.m_text.style.borderBottomColor = Color;

            calloutContainer.m_text.style.borderTopColor = GetOppositeColor(Color);
            calloutContainer.m_text.style.borderLeftColor = GetOppositeColor(Color);
            calloutContainer.m_text.style.borderRightColor = GetOppositeColor(Color);
            calloutContainer.m_text.style.borderBottomColor = GetOppositeColor(Color);

            calloutContainer.m_labelPoint.style.borderTopColor = GetOppositeColor(Color);
            calloutContainer.m_labelPoint.style.borderLeftColor = GetOppositeColor(Color);
            calloutContainer.m_labelPoint.style.borderRightColor = GetOppositeColor(Color);
            calloutContainer.m_labelPoint.style.borderBottomColor = GetOppositeColor(Color);

            calloutContainer.m_labelPoint.style.backgroundColor = Color;
            calloutContainer.m_text.style.color = GetOppositeColor(Color);
            calloutContainer.LineOutlineColor = GetOppositeColor(Color);
        }
    }
#if UNITY_EDITOR
    void OnEnable()
    {
        if (!Application.isPlaying)
        {
            if (projectManager == null)
            {
                projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
            }


            
        }
        
    }


    private void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            if (AddedToSection && Section != null)
            {
                foreach (PresentationSection section in projectManager.Sections)
                {
                    if (section == Section)
                    {
                        section.CalloutBindings.Remove(this);
                    }
                }
                AddedToSection = false;
            }
        }
    }
    
    private void OnValidate()
    {

        if(!Application.isPlaying)
        {
            float max = 100 - CalloutMargin;
            float min = 0 + CalloutMargin;

             float positiveXClamp = Map(min, max, 0, 0.25f, max, min, 0.5f, 0.75f, CalloutAngle);
            float positiveYClamp = Map(min, max, 0.25f, 0.5f, max, min, 0.75f, 1, CalloutAngle);


            CalloutXPosition = Mathf.Clamp(positiveXClamp, min, max);
            CalloutYPosition = Mathf.Clamp(positiveYClamp, min, max);

            calloutContainer = Section.CalloutCanvasDocument.rootVisualElement.Q<CalloutManager>(QueryName);

            UpdateLine();
        }
        
    }
#endif
    public Color GetOppositeColor(Color inputColor)
    {
        // Calculate the opposite color by subtracting each channel from 1.0
        float oppositeR = 1.0f - inputColor.r;
        float oppositeG = 1.0f - inputColor.g;
        float oppositeB = 1.0f - inputColor.b;

        // Create and return the opposite color
        Color oppositeColor = new Color(oppositeR, oppositeG, oppositeB, inputColor.a);
        return oppositeColor;
    }

    public float Map(float from, float to, float from2, float to2, float from3, float to3, float from4, float to4, float value)
    {
        if (value == from2)
        {
            return from;
        }
        else if (value == to2)
        {
            return to;
        }
        else if(value < to2)
        {
            return (to - from) * ((value - from2) / (to2 - from2)) + from;
        }

        if (value == from4)
        {
            return from3;
        }
        else if (value == to4)
        {
            return to3;
        }
        else if(value > to2)
        {
            return (to3 - from3) * ((value - from4) / (to4 - from4)) + from3;
        }


        else
        {
            return 0;
        }
    }
// Update is called once per frame
    void Update()
    {



#if UNITY_EDITOR
        if (projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
        }

        foreach (PresentationSection section in projectManager.Sections)
        {
            if (section == Section)
            {
                if (!section.CalloutBindings.Contains(this))
                {
                    section.CalloutBindings.Add(this);
                    Debug.Log("Your binding is not listed in this section!!!!!!!!!");
                }
            }
        }

        if (!AddedToSection && Section != null)
        {
            foreach (PresentationSection section in projectManager.Sections)
            {
                if(section == Section)
                {
                    section.CalloutBindings.Add(this);
                }
            }
            AddedToSection = true;
        }
#endif
        if (projectManager.ActiveSection != null)
        {
            if(projectManager.ActiveSection.CalloutCanvasDocument.enabled == true)
            {
                if (calloutContainer == null)
                {
                    calloutContainer = projectManager.ActiveSection.CalloutCanvasDocument.rootVisualElement.Q<CalloutManager>(QueryName);
                }

                if (CalloutPositioning == TetherLengthType.WorldSpace && m_CalloutLabel == null && calloutContainer != null)
                {
                    m_CalloutLabel = calloutContainer.Q<VisualElement>("Label");
                }
            }
        }
        
        if (calloutContainer != null)
        {
            calloutContainer.MarkDirtyRepaint();

            Vector2 pointPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 flipped = new Vector2(pointPos.x, Screen.height - pointPos.y);
            Vector2 finalPointPos = new Vector2(flipped.x / projectManager.uI_Manager.uIDocument.panelSettings.scale, flipped.y / projectManager.uI_Manager.uIDocument.panelSettings.scale);

            Vector2 finalLabelPos = new Vector2();


            if (Application.isPlaying && OccludeCallout)
            {
                //Code for occlussion fading of labels
                float collisionDistance = Vector3.Distance(transform.position, ShootRayAndFindClosestPoint(transform.position));
                if (CollisionDetected)
                {
                    float clampedOcclusionDistance = Mathf.Clamp(collisionDistance, 0, maxOcclusionDistance);
                    float normalizedOcclusionDistance = 1 - (clampedOcclusionDistance / maxOcclusionDistance);
                    float clampedOcclusionOpacity = Mathf.Clamp(normalizedOcclusionDistance, minOcclusionOpacity, 1);

                    calloutContainer.style.opacity = clampedOcclusionOpacity;
                }
            }
            

            if (calloutContainer.m_labelPoint != null)
            {
                calloutContainer.m_labelPoint.style.left = finalPointPos.x;
                calloutContainer.m_labelPoint.style.top = finalPointPos.y;
            }


            if (CalloutPositioning == TetherLengthType.WorldSpace)
            {
                float distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
                Vector2 resolvedScreenSize = new Vector2(Screen.width, Screen.height) / projectManager.uI_Manager.uIDocument.panelSettings.scale;
                //replace this with a transform.forward! 

                Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(transform.position);
                Vector3 OriginalLabelPosition = Camera.main.ViewportToWorldPoint(new Vector3(CalloutXPosition/100, (100 - CalloutYPosition)/100, ViewportPosition.z));
                float screenTetherDistance = Vector3.Distance(OriginalLabelPosition, transform.position);
                float finalTetherDistance = finalTetherDistance = Mathf.Clamp(screenTetherDistance, 0, TetherLength * TetherLengthIntensity);

                Vector3 tetherDirection = (OriginalLabelPosition - transform.position).normalized;
                Vector3 maxTetherPos = transform.position + (tetherDirection * finalTetherDistance);
                Vector3 finalViewportPos = Camera.main.WorldToViewportPoint(maxTetherPos);

                Vector3 finalScreenPos = Camera.main.WorldToScreenPoint(maxTetherPos) / projectManager.uI_Manager.uIDocument.panelSettings.scale;

                finalLabelPos = new Vector2(finalScreenPos.x, resolvedScreenSize.y - finalScreenPos.y);

                m_CalloutLabel.style.left = finalLabelPos.x - (m_CalloutLabel.worldBound.width/2);
                m_CalloutLabel.style.top = finalLabelPos.y - (m_CalloutLabel.worldBound.height/2);
            }
        }

        
    }

    private Vector3 lastHitPoint = Vector3.zero; // Store the last recorded hit point

    public Vector3 ShootRayAndFindClosestPoint(Vector3 targetPosition)
    {
        Camera mainCamera = Camera.main;

        Vector3 rayOrigin = mainCamera.transform.position;
        Vector3 rayDirection = targetPosition - rayOrigin;
        float maxDistance = rayDirection.magnitude;

        rayDirection.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance))
        {
            CollisionDetected = true;
            MeshCollider meshCollider = hit.collider as MeshCollider;

            if (meshCollider != null && meshCollider.sharedMesh != null)
            {
                Vector3 hitPoint = hit.point;
                Debug.DrawLine(rayOrigin, hitPoint, Color.white);

                lastHitPoint = hitPoint; // Update the last recorded hit point
                return hitPoint;
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin, rayDirection * maxDistance, Color.black);
            CollisionDetected = false;
        }

        Debug.DrawRay(rayOrigin, rayDirection * maxDistance, Color.green);

        // If no new hit is found, return the last recorded hit point
        return lastHitPoint;
    }


    public Color deselectedColor;
    public Color selectedColor;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(0.15f, 0.15f, 0.15f));
        Gizmos.color = selectedColor;
        Gizmos.DrawSphere(Vector3.zero, ProjectManager.GetGizmoSize(new Vector3(0.1f, 0.1f, 0.1f)));

    }
#endif
}
