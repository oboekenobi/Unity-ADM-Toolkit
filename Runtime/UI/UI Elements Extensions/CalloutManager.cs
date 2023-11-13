
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public class CalloutManager : VisualElement
{
    private Vector3 startPos, endPos;
    public float thickness;
    VisualElement m_DrawCanvas;
    public ProjectManager projectManager;
    public Camera mainCamera;
    public VisualElement m_label;
    public VisualElement m_labelPoint;
    public Label m_text;
    public Painter2D Line;
    public Painter2D LineOutline;
    public PresentationSection section;
    public GameObject labelPoint;
    public UnityEngine.Color LineColor;
    public UnityEngine.Color LineOutlineColor;

    public bool Initialized;

    public void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        if (projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
            projectManager.ActiveSection.testLineDrawer = this;
        }
        m_labelPoint = this.Q<VisualElement>("LabelPoint");
        m_label = this.Q<VisualElement>("Label");
        m_text = this.Q<Label>("Text");

        DrawLine(LineColor, thickness, mgc);
    }

    public void DrawLine(UnityEngine.Color color, float thickeness, MeshGenerationContext mgc)
    {
        var paint2D = mgc.painter2D;

        //Line = mgc.painter2D;
        //LineOutline = mgc.painter2D;

        paint2D.strokeColor = LineColor;

        paint2D.lineJoin = LineJoin.Round;

        paint2D.lineCap = LineCap.Round;

        paint2D.lineWidth = thickeness;

        paint2D.BeginPath();

        Vector2 startPos = new Vector2();




        //paint2D.MoveTo(startPos);

        paint2D.MoveTo(Vector2.zero);
        if (projectManager != null)
        {
            if (labelPoint != null)
            {

                Vector2 screenPos = Camera.main.WorldToScreenPoint(labelPoint.transform.position);
                float scale = projectManager.uI_Manager.uIDocument.panelSettings.scale;
                Vector2 LinePos = new Vector2(screenPos.x / scale, ((Screen.height - screenPos.y) / scale));

                m_labelPoint.style.left = LinePos.x;
                m_labelPoint.style.top = LinePos.y;

            }
            if (m_labelPoint != null)
            {
                startPos = new Vector2(m_labelPoint.resolvedStyle.left, m_labelPoint.resolvedStyle.top);
            }

            paint2D.MoveTo(startPos);
        }

        if (m_label != null)
        {
            Vector2 pointPos = new Vector2(m_labelPoint.resolvedStyle.left, m_labelPoint.resolvedStyle.top);

            Vector2 labelPos = new Vector2(m_label.resolvedStyle.left, m_label.resolvedStyle.top);

            Vector2 direction = (labelPos - pointPos).normalized;

            float angle = Mathf.Atan2(direction.x, direction.y);
            float rotation = angle * (Mathf.Rad2Deg);
            //float rotation = Vector2.Angle(pointPos, labelPos);

            //m_text.text = rotation.ToString();

            Vector2 endpoint = new Vector2();

            //Middle Right
            if (rotation <= -90 && rotation >= -135 || rotation <= -45 && rotation >= -90)
            {
                endpoint = new Vector2(m_label.resolvedStyle.left + m_label.resolvedStyle.width, (m_label.resolvedStyle.top + (m_label.resolvedStyle.height / 2)));
            }

            //Middle Top
            if (rotation <= 0 && rotation >= -45 || rotation >= 0 && rotation <= 45)
            {
                endpoint = new Vector2(m_label.resolvedStyle.left + (m_label.resolvedStyle.width / 2), (m_label.resolvedStyle.top));
            }

            //Middle Left
            if (rotation >= 45 && rotation <= 135)
            {
                endpoint = new Vector2(m_label.resolvedStyle.left, (m_label.resolvedStyle.top + (m_label.resolvedStyle.height / 2)));
            }

            //Middle Bottom
            if (rotation <= -135 && rotation >= -180 || rotation <= 180 && rotation >= 135)
            {
                endpoint = new Vector2(m_label.resolvedStyle.left + (m_label.resolvedStyle.width / 2), (m_label.resolvedStyle.top + m_label.resolvedStyle.height / 2));
            }
            paint2D.LineTo(endpoint);
        }
        else
        {
            //Debug.Log("Label Null!");
        }
        paint2D.ClosePath();
        paint2D.Stroke();
    }
    
    public CalloutManager()
    {
        if (projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
        }
        //Add a transform into the scene to point to 
        /*if (!Initialized)
        {

            if (this.enabledInHierarchy)
            {
                Debug.Log("Callout Init");
                GameObject point = Resources.Load<GameObject>("PreFabs/Label Point");
                labelPoint = GameObject.Instantiate(point, Vector3.zero, Quaternion.identity);
                Initialized = true;
            }
            else
            {
                Debug.Log("Failed");
            }
        }*/

        this.StretchToParentSize();
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        if (projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
            projectManager.ActiveSection.testLineDrawer = this;
        }
        //m_label = this.Q<VisualElement>("testLabel");

        generateVisualContent += OnGenerateVisualContent;
    }

    public new class UxmlFactory : UxmlFactory<CalloutManager, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        /*public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }
        UxmlColorAttributeDescription lineColor =
            new UxmlColorAttributeDescription { name = "Line Color") };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            Debug.Log("Init!!");
            base.Init(ve, bag, cc);

            var inputField = ve as CalloutManager;

            inputField.LineColor = lineColor.GetValueFromBag(bag, cc);


        }*/
    }


    public void OnGeometryChange(GeometryChangedEvent evt)
    {

        

        if (projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
        }
        //generateVisualContent += OnGenerateVisualContent;

        //this.RegisterCallback<>


    }

    public void Mouse(PointerMoveEvent evt)
    {
        m_labelPoint = this.Q<VisualElement>("LabelPoint");
        Debug.Log("repaint");
        this.MarkDirtyRepaint();


        /*Vector2 screenPos = Camera.main.WorldToScreenPoint(projectManager.ActiveSection.TestLinePoint.position);
        float scale = projectManager.uI_Manager.uIDocument.panelSettings.scale;
        Vector2 LinePos = new Vector2(screenPos.x / scale, ((Screen.height - screenPos.y) / scale));
        m_labelPoint.style.left = LinePos.x;
        m_labelPoint.style.top = LinePos.y;*/
    }

}