#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotGizmo : MonoBehaviour
{
    // Start is called before the first frame update

    [HideInInspector]
    public Color selectedColor;

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(0.15f, 0.15f, 0.15f));
        Gizmos.color = selectedColor;
        Gizmos.DrawSphere(Vector3.zero, ProjectManager.GetGizmoSize(new Vector3(0.1f, 0.1f, 0.1f)));
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

#endif
