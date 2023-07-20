using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using geniikw.DataRenderer2D;

public class LaserPointer : MonoBehaviour//, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public UILine lineRenderer;
    public LineRenderer rend;
    public Canvas canvas;
    public RectTransform canvasRectTransform;
    [Range(0, 5)]
    public float lineWidth;
    Coroutine drawing;
    private int numPoints;
    private Vector2[] points;

    private void Start()
    {
/*        numPoints = 0;
        points = new Vector2[1024];
        lineRenderer.line. = numPoints;
        rend.*/
    }

    /*public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            points[numPoints] = localPoint;
            numPoints++;
            lineRenderer.positionCount = numPoints;
            lineRenderer.SetPosition(numPoints - 1, localPoint);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            points[numPoints] = localPoint;
            numPoints++;
            lineRenderer.positionCount = numPoints;
            lineRenderer.SetPosition(numPoints - 1, localPoint);
        }
    }*/

    public void OnPointerUp(PointerEventData eventData)
    {
        // do nothing
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLine();
        }
        if (Input.GetMouseButtonUp(0))
        {
            FinishLine();
        }
    }

    public void StartLine()
    {
        if (drawing != null)
        {
            StopCoroutine(drawing);
        }
        drawing = StartCoroutine(DrawLine());
    }

    public void FinishLine()
    {
        StopCoroutine(drawing);
    }


    public IEnumerator DrawLine()
    {
        //GameObject newGameObject = Instantiate(Resources.Load("LaserPointerStroke") as GameObject, new Vector3(0, 0, 0), Quaternion.identity);
        //lineRenderer = newGameObject.GetComponent<UILine>();
        UILine line = GameObject.Instantiate(lineRenderer, Vector3.zero, Quaternion.identity);
        line.transform.parent = canvas.transform;
        while (true)
        {
            Vector3 position = Input.mousePosition;
            position.z = 0;
            
            lineRenderer.line.Push(position, Vector3.zero, Vector3.zero, lineWidth);
            line.GeometyUpdateFlagUp();
            //lineRenderer.line.points.Add(new Point(position, Vector3.zero, Vector3.zero, lineWidth));
            yield return null;
            Debug.Log("Drawing");
        }
    }
}
