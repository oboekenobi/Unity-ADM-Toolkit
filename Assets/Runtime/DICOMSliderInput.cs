using ADM.UISystem;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class DICOMSliderInput : MonoBehaviour
{
    public Transform Slicer;
    public Slider DICOMSlider;
    public GameObject DICOM;
    public float DICOMWidth;
    public float DICOMHeight;
    public float DICOMLength;
    public float padding = 1.5f;
    public static bool wasSliding;

#if UNITY_EDITOR

    private void OnEnable()
    {
        Vector3 DICOMSize = GetSize(DICOM);
        DICOMLength = DICOMSize.x;
        DICOMHeight = DICOMSize.y;
        DICOMWidth = DICOMSize.z;
        DICOMSlider.minValue = -(DICOMWidth / 2) - padding;
        DICOMSlider.maxValue = (DICOMWidth / 2) + padding;
    }

#endif

    public Vector3 slicerPosition()
    {
        List<Vector3> AxisPositions = new List<Vector3>
        { new Vector3(-DICOMSlider.value, DICOM.transform.position.y, DICOM.transform.position.z),
          new Vector3(DICOM.transform.position.x, DICOMSlider.value, DICOM.transform.position.z),
          new Vector3(DICOM.transform.position.x, DICOM.transform.position.y, DICOMSlider.value),
        };

        return AxisPositions[Incriment];
    }
    public Vector3 slicerRotation()
    {
        List<Vector3> AxisRotations = new List<Vector3>
        { new Vector3(0, -90, 0),
          new Vector3(-90, 0, 0),
          new Vector3(0, 0, 0),
        };

        return AxisRotations[Incriment];
    }

    public void SlideSlicer()
    {
        Slicer.position = slicerPosition();
        Slicer.localEulerAngles = slicerRotation();
        wasSliding = true;
    }

    public void FlipAxis()
    {
        Next();
        Slicer.position = slicerPosition();
        Slicer.localEulerAngles = slicerRotation();
        if(Incriment == 0)
        {
            DICOMSlider.minValue = -(DICOMWidth / 2) - padding;
            DICOMSlider.maxValue = (DICOMWidth / 2) + padding;
        }
        if (Incriment == 1)
        {
            DICOMSlider.minValue = -(DICOMHeight / 2) - padding;
            DICOMSlider.maxValue = (DICOMHeight / 2) + padding;
        }
        if (Incriment == 2)
        {
            DICOMSlider.minValue =  -(DICOMLength / 2) - padding;
            DICOMSlider.maxValue =  (DICOMLength / 2) + padding;
        }
    }


    public int Incriment = 0;
    public void Next()
    {
        int Count = 3;
        // Increment the current index and wrap around to the start of the list
        // if we've reached the end
        Incriment = (Incriment + 1) % Count;
    }

    //write a function that returns the height of a GameObject
    public Vector3 GetSize(GameObject obj)
    {
        // Get the transform component of the object
        Transform transform = obj.GetComponent<Transform>();

        // Return the size of the object (width and height) as a Vector2
        return new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}