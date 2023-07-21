using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PictureTrackingPoint : MonoBehaviour
{
#if UNITY_EDITOR
    public VisualLabel Label;

    public bool Added;

    private void OnEnable()
    {
        if (!Added)
        {
            Transform PointFolder = GameObject.FindWithTag("LabelPoints Folder").transform;
            gameObject.transform.parent = PointFolder;

            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(200,200,0));

            Added = true;
        }


    }
    private void Update()
    {
        if (!Application.isPlaying)
        {
            if (Label != null)
            {
                gameObject.name = (Label.gameObject.name + " Point");
            }
        }
    }
#endif
}
