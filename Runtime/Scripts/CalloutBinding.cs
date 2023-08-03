
using UnityEngine;
using UnityEngine.UIElements;


[ExecuteInEditMode]
public class CalloutBinding : MonoBehaviour
{

    public string QueryName = "";

    private ProjectManager projectManager;

    private CalloutManager calloutContainer;

    // Start is called before the first frame update
    void OnEnable()
    {

        if (projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (projectManager == null)
        {
            projectManager = GameObject.FindAnyObjectByType<ProjectManager>();
        }

        if(projectManager.ActiveSection != null)
        {
            if(projectManager.ActiveSection.CalloutCanvasDocument.enabled == true)
            {
                calloutContainer = projectManager.ActiveSection.CalloutCanvasDocument.rootVisualElement.Q<CalloutManager>(QueryName);
            }
        }

        if (calloutContainer != null)
        {
            calloutContainer.MarkDirtyRepaint();

            Vector2 pointPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 flipped = new Vector2(pointPos.x, Screen.height - pointPos.y);
            Vector2 finalPos = new Vector2(flipped.x / projectManager.uI_Manager.uIDocument.panelSettings.scale, flipped.y / projectManager.uI_Manager.uIDocument.panelSettings.scale);

            if(calloutContainer.m_labelPoint != null)
            {
                calloutContainer.m_labelPoint.style.left = finalPos.x;
                calloutContainer.m_labelPoint.style.top = finalPos.y;
            }
            
        }
    }

    public Color deselectedColor;
    public Color selectedColor;
    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(0.15f, 0.15f, 0.15f));
        Gizmos.color = selectedColor;
        Gizmos.DrawSphere(Vector3.zero, ProjectManager.GetGizmoSize(new Vector3(0.1f, 0.1f, 0.1f)));

    }
}
