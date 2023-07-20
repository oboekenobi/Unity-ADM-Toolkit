using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LabelPoint : MonoBehaviour
{
#if UNITY_EDITOR
    public Transform point;
    public CallOutLabel label;
    public GameObject Circle;
    public bool hasInitialized;
    public ProjectManager manager;
    [SerializeField] int instanceID = 0;
    public void OnEnable()
    {
        point = gameObject.GetComponent<Transform>();
        if (instanceID != GetInstanceID())
        {
            if (instanceID == 0)
            {
                instanceID = GetInstanceID();
            }
            else
            {
                instanceID = GetInstanceID();
                if (instanceID < 0)
                {
                    Debug.Log("Point initialized");
                    hasInitialized = false;
                }
            }
        }
    }
    public void Update()
    {
        if (!Application.isPlaying)
        {
            if (label != null & !hasInitialized)
            {
                label.InterestPoints.Add(point);
                gameObject.transform.parent = GameObject.FindWithTag("LabelPoints Folder").transform;
                manager = GameObject.FindWithTag("SceneManager").GetComponent<ProjectManager>();
                if (manager != null)
                {
                    manager.Sections[manager.ActiveSectionIndex].CallOutPoints.Add(gameObject);
                }
                hasInitialized = true;
            }
            if (label != null)
            {
                gameObject.name = label.gameObject.name + " Point";
            }
            if(label == null)
            {
                //DestroyImmediate(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if(label != null)
        {
            for (int i = 0; i < label.InterestPoints.Count; i++)
            {
                if (label.InterestPoints[i] == point)
                {

                    label.InterestPoints.RemoveAt(i);
                    DestroyImmediate(label.Circles[i].gameObject);
                    label.Circles.RemoveAt(i);
                    DestroyImmediate(label.Lines[i]);
                    label.Lines.RemoveAt(i);
                    label.UILines.RemoveAt(i);
                }
            }
            if (manager != null)
            {
                for (int i = 0; i < manager.Sections[manager.ActiveSectionIndex].CallOutPoints.Count; i++)
                {
                    if (manager.Sections[manager.ActiveSectionIndex].CallOutPoints[i] == gameObject)
                    {
                        manager.Sections[manager.ActiveSectionIndex].CallOutPoints.RemoveAt(i);
                    }
                }
            }
        }
    }
#endif
}
