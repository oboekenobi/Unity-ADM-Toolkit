using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


[ExecuteInEditMode]
public class CalloutBinding : MonoBehaviour
{

    public string QueryName = "";

    public ProjectManager projectManager;

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
}
