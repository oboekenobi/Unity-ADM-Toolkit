using UnityEngine;
using System.Collections.Generic;
using ADM.UISystem;
using UnityEditor;

public class DrawWithMouse : MonoBehaviour
{
    // Variables for line width and color
    [Tooltip("Width of the line")]
    public float lineWidth = 0.1f;
    [Tooltip("Color of the line")]
    public Color lineColor = Color.white;
    [Range(0,10)]
    public float CameraDistance;
    public GameObject lineStroke;
    public RectTransform drawRect;
    public GameObject strokePrefab;
    //public GameObject drawGo;


    // Boolean to check if left mouse button is down
    private bool isMouseDown = false;

    // List to store all instantiated line renderers
    public List<GameObject> storedLineRenderers = new List<GameObject>();

    // Public variable to store the current line renderer
    public LineRenderer CurrentLineRenderer { get; private set; }

    // Camera to render the line renderer
    [Tooltip("Camera to render the line renderer")]
    public Camera lineCamera;

    // Render texture to store the line renderer
    public RenderTexture lineRenderTexture;

    // Start is called before the first frame update
    void Start()
    {

        // Create a new render texture
        //lineRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        //lineRenderTexture.Create();

        // Set the target texture of the line camera to the render texture
        //lineCamera.targetTexture = lineRenderTexture;
    }
    public void InitializeLine()
    {
        //GameObject newLine = PrefabUtility.InstantiatePrefab(lineStroke);
        GameObject newLine = GameObject.Instantiate(lineStroke, Vector3.zero, Quaternion.identity);
        newLine.transform.SetParent(transform);
        newLine.layer = LayerMask.NameToLayer("Laser");
        LineRenderer newLineRenderer = newLine.GetComponent<LineRenderer>();
        newLineRenderer.startWidth = lineWidth;
        newLineRenderer.endWidth = lineWidth;
        newLineRenderer.material.color = lineColor;
        CurrentLineRenderer = newLineRenderer;
        storedLineRenderers.Add(newLine);

        lineRenderTexture.Release();
        lineRenderTexture.width = Screen.width;
        lineRenderTexture.height = Screen.height;
        //drawGo.SetActive(true);

        /*        // Set the width of the line renderer based on screen resolution
                float lineWidth = (Screen.height / 1080f) * 0.1f; // Change 1080 to your desired reference screen height
                newLineRenderer.startWidth = lineWidth;
                newLineRenderer.endWidth = lineWidth;*/
    }
    // Update is called once per frame

    void Update()
    {
       
        // Check if left mouse button is down
        if (Input.GetMouseButtonDown(0) && UI_Manager.DrawingMode)
        {
            if (!isMouseDown)
            {

                InitializeLine();
            }
            // Set boolean to true
            isMouseDown = true;
            // Instantiate a new line renderer
            //GameObject newLine = new GameObject("Line");


            // Set the position of the first point in the line renderer
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = lineCamera.nearClipPlane + CameraDistance;
            Vector3 worldPosition = lineCamera.ScreenToWorldPoint(mousePosition);
            //Vector3 worldPosition = WorldToScreenSpace();
            worldPosition = new Vector3(worldPosition.x, worldPosition.y, lineCamera.nearClipPlane + CameraDistance); // Set the z position to be in front of the camera
            CurrentLineRenderer.positionCount++;
            CurrentLineRenderer.SetPosition(0, worldPosition);


            // Scale down the line renderer to fit the screen space
            float distance = Vector3.Distance(lineCamera.transform.position, CurrentLineRenderer.transform.position);
            float scale = distance / 10f;
            CurrentLineRenderer.transform.localScale = new Vector3(scale, scale, scale);
        }

        // Check if left mouse button is up
        if (Input.GetMouseButtonUp(0) && UI_Manager.DrawingMode)
        {
            // Set boolean to false
            isMouseDown = false;

            // Set the current line renderer to null
            CurrentLineRenderer = null;
        }

        // Check if left mouse button is down and current line renderer is not null
        if (isMouseDown && CurrentLineRenderer != null && UI_Manager.DrawingMode)
        {
            // Add a new point to the current line renderer at the mouse position
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = lineCamera.nearClipPlane + CameraDistance;
            Vector3 worldPosition = lineCamera.ScreenToWorldPoint(mousePosition);
            //Vector3 worldPosition = WorldToScreenSpace();
            worldPosition = new Vector3(worldPosition.x, worldPosition.y, lineCamera.nearClipPlane + CameraDistance); // Set the z position to be in front of the camera
            CurrentLineRenderer.positionCount++;
            CurrentLineRenderer.SetPosition(CurrentLineRenderer.positionCount - 1, worldPosition);
        }

        // Render the line renderer to the render texture
        //lineCamera.Render();

        // Set the render texture as the material of the line renderer
        //lineRenderer.material.mainTexture = lineRenderTexture;
    }

    public void ErasedDrawing()
    {
        Debug.Log("Drawing Destroyed");
        UI_Manager.DrawingMode = false;
        // Loop through all stored line renderers and destroy them
        foreach (GameObject storedLineRenderer in storedLineRenderers)
        {
            Destroy(storedLineRenderer.gameObject);
        }
        //drawGo.SetActive(false);
        lineRenderTexture.Release();
        lineRenderTexture.width = Screen.width;
        //lineRenderTexture.antiAliasing = 2;
        lineRenderTexture.height = Screen.height;;
        // Clear the list of stored line renderers
        storedLineRenderers.Clear();
    }

    public Vector3 WorldToScreenSpace()
    {
        Vector3 screenPoint = lineCamera.WorldToScreenPoint(Input.mousePosition);
        //screenPoint.z = 0;

        Vector2 screenPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawRect, Input.mousePosition, lineCamera, out screenPos))
        {
            return screenPos;
        }

        return screenPoint;
    }
}