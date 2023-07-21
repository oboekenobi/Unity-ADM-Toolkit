using UnityEngine;
using TMPro;
using ADM.UISystem;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class DistanceMeasure : MonoBehaviour
{
    [Tooltip("The first point of the line.")]
    public Transform startPoint;
    [Tooltip("The second point of the line.")]
    public Transform endPoint;
    [Tooltip("The color of the line.")]
    public Color lineColor = Color.white;
    [Tooltip("The font size of the text label.")]
    public int fontSize = 24;

    public LineRenderer lineRenderer;
    public List<LineRenderer> CapLineRenders = new List<LineRenderer>();
    public float lineWidth;
    public TextMeshPro textLabel;
    public Transform LabelParent;
    private Camera mainCamera;

    private void Start()
    {
        // Create the line renderer component
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        // Create the text label component
        textLabel.fontSize = fontSize;
        textLabel.alignment = TextAlignmentOptions.Center;
        
        
    }

    private void Update()
    {
#if UNITY_EDITOR
        // Set the positions of the line renderer
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);

        // Calculate the distance between the two points
        float distance = Vector3.Distance(startPoint.position, endPoint.position);

        // Set the text of the text label
        textLabel.text = distance.ToString("F2");

        // Set the position of the text label to the center of the line
        LabelParent.transform.position = (startPoint.position + endPoint.position) / 2f;

        // Rotate the label parent to align with the line
        Vector3 lookPosx = (LabelParent.transform.position - startPoint.position);
        //Vector3 lookPosx = LabelParent.transform.eulerAngles;
        lookPosx.y = 0;
        Quaternion rotationx = Quaternion.LookRotation(lookPosx);
        //Quaternion rotationx = LabelParent.transform.rotation * startPoint.rotation;

        // Set the LabelParent x angle to face the scene camera
        Vector3 lookPos = SceneView.lastActiveSceneView.camera.transform.position - LabelParent.transform.position;
        //Vector3 lookPos = SceneView.lastActiveSceneView.camera.transform.eulerAngles;
        lookPos.z = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        //Quaternion rotation = SceneView.lastActiveSceneView.camera.transform.rotation * LabelParent.transform.rotation;

        var f = new Vector3(SceneView.lastActiveSceneView.camera.transform.eulerAngles.x, 0, 0);
        var c = new Vector3(0, rotationx.eulerAngles.y + 90, rotationx.eulerAngles.z);
        LabelParent.transform.eulerAngles = c - f;

        startPoint.transform.eulerAngles = c-f;
        endPoint.transform.eulerAngles = c-f;

        //LabelParent.transform.localEulerAngles = new Vector3(SceneView.lastActiveSceneView.camera.transform.eulerAngles.x, rotationx.eulerAngles.y, SceneView.lastActiveSceneView.camera.transform.eulerAngles.z);

        // Rotate the start and end points to face each other
        //startPoint.transform.LookAt(endPoint);
        //endPoint.transform.LookAt(startPoint);

#endif

    }
}