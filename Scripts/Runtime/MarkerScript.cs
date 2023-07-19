using UnityEngine;
using UnityEngine.UIElements;
using ADM.UISystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class MarkerScript : MonoBehaviour
{
    [Tooltip("The visual tree asset to use for the marker.")]
    public VisualTreeAsset markerVisualTreeAsset;

    [Tooltip("The distance from the edge of the game window at which the marker will start to fade.")]
    public float fadeThreshold = 50f;

    [Tooltip("The text to display on the marker label.")]
    public string markerLabelText;

    public VisualElement Layout;

    public VisualElement Marker;

    private VisualElement GameWindow;

    public bool staticMarker;
   
    public VisualElement exhibitMarker;

    public UIDocument uIDocument;

    private Camera mainCamera;

    private UI_Manager uI_Manager;

    public List<GameObject> Outlines = new List<GameObject>();

    public Color OutlineColor;

    public PresentationSection targetSection;

    public VisualElement Bounds;

    public void Awake()
    {

        uI_Manager = GameObject.FindObjectOfType<UI_Manager>();
        // Get the main camera
        mainCamera = Camera.main;
        uIDocument = GameObject.FindObjectOfType<UIDocument>();
        // Get the game window
        GameWindow = uIDocument.rootVisualElement.Q<VisualElement>("GameWindow");
        // Create the marker
        exhibitMarker = markerVisualTreeAsset.CloneTree();
        exhibitMarker.Q<Label>("MarkerLayout").text = markerLabelText;

        Layout = uIDocument.rootVisualElement.Q<VisualElement>("MarkerLayout");
        exhibitMarker.style.position = Position.Absolute; // Set position to absolute
        exhibitMarker.style.flexShrink = 0;
        Marker = exhibitMarker.Q<VisualElement>("MarkerLayout");
        Layout.Add(exhibitMarker);
        Marker.style.display = DisplayStyle.None;
        Bounds = uIDocument.rootVisualElement.Q<VisualElement>("Main");
        //Layout.Add(exhibitMarker);
        if (staticMarker)
        {
            // Get the screen position of the game object
            screenPosition = (mainCamera.WorldToScreenPoint(transform.transform.position)) / uIDocument.panelSettings.scale;
            //exhibitMarker.worldBound.position.Set(screenPosition.x, screenPosition.y);
            // Set the position of the exhibit marker
            exhibitMarker.style.left = screenPosition.x;
            exhibitMarker.style.top = (Bounds.worldBound.yMax - screenPosition.y); // Invert the y position
        }
        // Set the text of the marker label

        if (Outlines.Count > 0)
        {
            exhibitMarker?.RegisterCallback<MouseEnterEvent>(ev => OutlineObjects());
            exhibitMarker?.RegisterCallback<MouseLeaveEvent>(ev => UnOutlineObjects());
        }
        if(HighlightObject != null)
        {
            exhibitMarker?.RegisterCallback<MouseEnterEvent>(ev => ShowHighlight());
            exhibitMarker?.RegisterCallback<MouseLeaveEvent>(ev => unShowHighlight());
        }
        if (targetSection != null)
        {
            exhibitMarker?.RegisterCallback<ClickEvent>(ev => uI_Manager.SetPresentationSection(targetSection));
        }
        else
        {
            Debug.LogWarning("Exhibit Marker " + markerLabelText + " does not have a reference to a section");
        }
        
    }

    private Vector3 screenPosition;
    private void Update()
    {
        if (!staticMarker)
        {
            // Get the screen position of the game object
            screenPosition = (mainCamera.WorldToScreenPoint(transform.transform.position)) / uIDocument.panelSettings.scale;
            //exhibitMarker.worldBound.position.Set(screenPosition.x, screenPosition.y);
            // Set the position of the exhibit marker
            exhibitMarker.style.left = screenPosition.x;
            exhibitMarker.style.top = (Bounds.worldBound.yMax - screenPosition.y); // Invert the y position
        }
    }

    private void LateUpdate()
    {
        /*// Calculate the distance from the edge of the game window, reverse it because UI Documents position start from top left.
        float distanceFromEdge = Mathf.Min(GameWindow.worldBound.xMax - exhibitMarker.worldBound.center.x, GameWindow.worldBound.xMax - (GameWindow.worldBound.xMax - exhibitMarker.worldBound.center.x), GameWindow.worldBound.yMax - exhibitMarker.worldBound.center.y, GameWindow.worldBound.yMax - (GameWindow.worldBound.yMax - exhibitMarker.worldBound.center.y));
        // Calculate the opacity of the exhibit marker based on the distance from the edge and the fade threshold
        float opacity = ExtensionMethods.Remap(distanceFromEdge, 0, fadeThreshold, 0, 1);


        // Set the opacity of the exhibit marker

        exhibitMarker.style.opacity = Mathf.Clamp01(opacity);*/
    }

    public void OutlineObjects()
    {
        if(Outlines.Count > 0)
        {
            Debug.Log("Highlgiht");
            foreach (GameObject go in Outlines)
            {
                if (go == null)
                    return;
                go.layer = LayerMask.NameToLayer("Outline");
            }
            Shader.SetGlobalColor("OutlineColor", OutlineColor);
            StartCoroutine(showOutline(0, 1, 0.1f));
        }
    }
    public void UnOutlineObjects()
    {
        if(Outlines.Count > 0)
        {
            Debug.Log("UnHighlgiht");
            foreach (GameObject go in Outlines)
            {
                if (go == null)
                    return;
                go.layer = LayerMask.NameToLayer("Default");
            }
            StartCoroutine(showOutline(1, 0, 0.1f));
        }
    }

    public IEnumerator showOutline(float start, float end, float duration)
    {
        float elapsedTime = 0f;
        float targetValue = start;
        while (elapsedTime < duration)
        {
            targetValue = Mathf.Lerp(start, end, elapsedTime/duration);
            Shader.SetGlobalFloat("Thickness", (0.02f * targetValue));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetValue = end;
        Shader.SetGlobalFloat("Thickness", (0.02f * targetValue));
    }

    public void ShowHighlight()
    {
        HighlightObject.layer = LayerMask.NameToLayer("UI");
        ProjectManager.ContrastLayer.alpha = 1;
        //StartCoroutine(ContrastFade());
    }

    public void unShowHighlight()
    {
        StopCoroutine(ContrastFade());
        ProjectManager.ContrastLayer.alpha = 0;
        HighlightObject.layer = LayerMask.NameToLayer("Default");
        //StartCoroutine(StopContrastFade());
    }

    public GameObject HighlightObject;
    public IEnumerator ContrastFade()
    {
        float start = ProjectManager.ContrastLayer.alpha;
        float end = 1;
        float duration = 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime <= end)
        {
            ProjectManager.ContrastLayer.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public IEnumerator StopContrastFade()
    {
        float start = ProjectManager.ContrastLayer.alpha;
        float end = 0;
        float duration = 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime <= end)
        {
            ProjectManager.ContrastLayer.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("End Reached");
        ProjectManager.ContrastLayer.alpha = 0;
        HighlightObject.layer = LayerMask.NameToLayer("Default");
    }
}