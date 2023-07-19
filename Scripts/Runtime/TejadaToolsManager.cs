using ADM.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class TejadaToolsManager : VisualElement
{

    Toggle m_roadsToggle;
    Toggle m_markerToggle;
    Toggle m_bridgeToggle;
    public UI_Manager uI_Manager;
  
    public new class UxmlFactory : UxmlFactory<TejadaToolsManager, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    public TejadaToolsManager()
    {
        uI_Manager = GameObject.FindFirstObjectByType<UI_Manager>();
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void OnGeometryChange(GeometryChangedEvent evt)
    {
        m_roadsToggle = this.Q<Toggle>("RoadsToggle");
        m_markerToggle = this.Q<Toggle>("MarkerToggle");
        m_bridgeToggle = this.Q<Toggle>("BridgeToggle");

        m_roadsToggle?.RegisterCallback<ClickEvent>(ev => ToggleRoads());
        m_markerToggle?.RegisterCallback<ClickEvent>(ev => HideLabelMarkers());
        m_bridgeToggle?.RegisterCallback<ClickEvent>(ev => ToggleBridge());
    }

    void ToggleRoads()
    {
        if (m_roadsToggle.value)
        {
            uI_Manager.DitherHighlight.SetFloat("_AlphaThreshold", 0);
        }
        if(!m_roadsToggle.value)
        {
            uI_Manager.DitherHighlight.SetFloat("_AlphaThreshold", 2);
        }
    }

    void HideLabelMarkers()
    {
        if (!m_markerToggle.value)
        {
            uI_Manager.projectManager.ActiveSection.Markers.SetActive(true);
        }
        if (m_markerToggle.value)
        {
            uI_Manager.projectManager.ActiveSection.Markers.SetActive(false);
        }
    }

    void ToggleBridge()
    {
        if (m_bridgeToggle.value)
        {
            uI_Manager.projectManager.Sections[0].ToggleObjects[0].gameObject.SetActive(true);
        }
        else
        {
            uI_Manager.projectManager.Sections[0].ToggleObjects[0].gameObject.SetActive(false);
        }
    }

   
}
