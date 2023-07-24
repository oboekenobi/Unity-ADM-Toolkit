using ADM.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class LineDrawer : VisualElement
{
    private Vector3 startPos, endPos;
    private float thickness;
    VisualElement m_DrawCanvas;
    public ProjectManager projectManager;
    public Camera mainCamera;
    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var paint2D = mgc.painter2D;

        paint2D.fillColor = new Color(255,255,255,255);
        paint2D.lineJoin = LineJoin.Round;
        paint2D.lineCap = LineCap.Round;
        paint2D.lineWidth = 10.0f;


        paint2D.BeginPath();
        paint2D.MoveTo(new Vector2(100, 100));

        if(projectManager != null)
        {
            if(projectManager.ActiveSection.TestLinePoint != null)
            {
                Vector2 LinePos = Camera.main.WorldToScreenPoint(projectManager.ActiveSection.TestLinePoint.position);
                paint2D.LineTo(LinePos);
            }
            Debug.Log("Partner!");
        }
        //paint2D.LineTo(new Vector2(1000, 1000));
        //paint2D.LineTo(p2);
        //paint2D.LineTo(p3);
        paint2D.ClosePath();
        paint2D.Stroke();
    }
    public LineDrawer()
    {
        //generateVisualContent += OnGenerateVisualContent;
        this.RegisterCallback<MouseMoveEvent>(OnGeometryChange);
        if (projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
        }
    }

    public new class UxmlFactory : UxmlFactory<LineDrawer, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    
    public void OnGeometryChange(MouseMoveEvent evt)
    {
        if(projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
        }

        Debug.Log("Howdy");
        generateVisualContent += OnGenerateVisualContent;
    }

}

/*public class CanvasManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Color lineColor = Color.black;
    void Start()
    {
        var doc = GetComponent<UIDocument>();
        doc.rootVisualElement.Add(new LineDrawer(new Vector2(20, 50), new Vector2(50, 50), 10));
    }
}*/
