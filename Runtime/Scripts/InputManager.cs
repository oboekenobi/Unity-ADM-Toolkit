using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using System.Linq;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor;
using Cinemachine.PostFX;
using UnityEngine.UIElements;
using geniikw.DataRenderer2D;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

namespace ADM.UISystem
{
    public class InputManager : MonoBehaviour
    {
        #region Dependencies
        [Header("Script References")]
        public UI_Manager uI_Manager;
        public static InputManager _instance;
        public static Camera MainCamera;
        public ProjectManager projectManager;
        public CinemachineBrain brain;
        public PlayableAsset TimelineTemplate;
        public CinemachineVirtualCamera TransitionCamera;
        public CinemachineVolumeSettings TransitionSettings;
        public TimelineAsset TimelineSequence;
        public InputActionAsset Inputs;
        public InputAction RotateCamera;
        public InputAction Zoom;
        public InputAction SecondaryFinger;
        public InputAction PrimaryFinger;
        public InputAction TouchContact;
        public InputAction Contact;
        public InputAction Shift;
        public CutOutPoint nextCutOutPoint;
        public Effects_Manager Transition;
        #endregion

        #region Transforms & Vectors
        [Space (10)]
        [Header("Transforms")]
        public Transform MainObject;
        public Transform MainCameraTransform;
        public Transform CameraOffset;
        private Vector2 swipeDirection; //swipe delta vector2
        public Vector3 StartPivotPosition;
        public Vector3 debugPos;
        private Quaternion cameraRot; // store the quaternion after the slerp operation
        #endregion

        #region Floats & Ints
        [Range(0f, 4f)]
        public float mouseDragThreshold;
        [Range(0f, 2f)]
        public float labelFadeDuration = 0.5f;
        [Space(10)]
        [Header("Float Values")]

        [Tooltip("How sensitive the mouse drag to camera rotation")]
        public float ZoomSpeed = 0.8f;

        [Tooltip("How sensitive the mouse drag to camera rotation")]
        public float ZoomMinFOV = 0.8f;

        [Tooltip("How sensitive the mouse drag to camera rotation")]
        public float ZoomMaxFOV = 0.8f;
        public Transform target;
        [Range(0.1f, 8f)]
        [Tooltip("How sensitive the mouse drag to camera rotation")]
        public float mouseRotateSpeed = 0.8f;
        [Range(0.01f, 100)]
        [Tooltip("How sensitive the touch drag to camera rotation")]
        public float touchRotateSpeed = 17.5f;
        [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
        public float slerpValue = 0.25f;
        private float slerpedValue;
        [Range(-10, 10)]
        public float CameraScreenOffset = 0;

        public float currentBrainBlend;


        public Vector3 blendedCamera;
        public Vector3 blendedCameraRotation;

        public float currentCameraDistance;
        public Vector3 currentCameraDirection;
        public Vector3 startCameraPosition;
        public Vector3 secondCameraPosition;
        //[HideInInspector]
        public float distanceBetweenCameraAndTarget()
        {
            float BlendedDistance = new float();


            /*PreviousCamera = (brain.ActiveBlend.CamA.VirtualCameraGameObject);
            NextCamera = (brain.ActiveBlend.CamB.VirtualCameraGameObject);*/

            /*float blendedDistance = Vector3.Distance(MainCameraTransform.position, BlendedCameraPivot());
            CachedPivotPosition = BlendedCameraPivot();
            return blendedDistance;*/

            float blend = new float();


            currentCameraDirection = (secondCameraPosition - startCameraPosition).normalized;
            currentCameraDistance = Vector3.Distance(startCameraPosition, secondCameraPosition);
            Vector3 blendPosition = startCameraPosition + (currentCameraDirection * (currentCameraDistance * currentBrainBlend));
            blend = Vector3.Distance(blendPosition, BlendedCameraPivot());
            blendedCamera = blendPosition;

            Vector3 pivotDirection = (BlendedCameraPivot() - gameObject.transform.position);
            Quaternion rotation = Quaternion.LookRotation(pivotDirection);
            blendedCameraRotation = rotation.eulerAngles;
            CachedDistance = blend;
            return blend;

            /*if (brain != null && brain.IsBlending && brain.ActiveBlend.BlendWeight >= 0 && brain.ActiveBlend.BlendWeight <= 1)
            {
                //float distance = Vector3.Distance(brain.ActiveBlend.CamA.VirtualCameraGameObject.transform.position, brain.ActiveBlend.CamB.VirtualCameraGameObject.transform.position);
                //Vector3 Direction = (brain.ActiveBlend.CamB.VirtualCameraGameObject.transform.position - brain.ActiveBlend.CamA.VirtualCameraGameObject.transform.position).normalized;
                 
                //float blendPercentage = brain.ActiveBlend.BlendWeight;

                Vector3 blendPosition = startCameraPosition + (currentCameraDirection * (currentCameraDistance * currentBrainBlend));
                blend = Vector3.Distance(blendPosition, BlendedCameraPivot());
                blendedCamera = blendPosition;

                Vector3 pivotDirection = (BlendedCameraPivot() - gameObject.transform.position);
                Quaternion rotation = Quaternion.LookRotation(pivotDirection);
                blendedCameraRotation = rotation.eulerAngles;
                CachedDistance = blend;
                return blend;
            }
            else
            {
                return CachedDistance;
            }*/
            


            /*if (brain.ActiveBlend != null)
            {

                if (NextCamera.GetComponent<TweenSection>() != null && PreviousCamera.GetComponent<TweenSection>() != null)
                {
                    TweenSection CameraA = (brain.ActiveBlend.CamA.VirtualCameraGameObject).GetComponent<TweenSection>();
                    TweenSection BlendCamera = (brain.ActiveBlend.CamB.VirtualCameraGameObject).GetComponent<TweenSection>();
                    float DistanceA = Vector3.Distance(MainCameraTransform.position, CameraA.PivotAngle.transform.position);
                    float DistanceB = Vector3.Distance(BlendCamera.gameObject.transform.position, BlendCamera.PivotAngle.transform.position);
                    BlendedDistance = Mathf.Lerp(DistanceA, DistanceB, LastBlend);
                    CachedPivotPosition = BlendedCameraPivot();
                    return BlendedDistance;
                }
                else
                {
                    Debug.Log("Flase Tween");
                }
                if (PreviousCamera.GetComponent<TweenSection>() != null && NextCamera == projectManager.ActiveSection.gameObject)
                {
                    TweenSection CameraA = (brain.ActiveBlend.CamA.VirtualCameraGameObject).GetComponent<TweenSection>();
                    PresentationSection BlendCamera = projectManager.ActiveSection;
                    float DistanceA = Vector3.Distance(MainCameraTransform.position, CameraA.PivotAngle.transform.position);
                    float DistanceB = Vector3.Distance(BlendCamera.gameObject.transform.position, BlendCamera.Pivot.transform.position);
                    BlendedDistance = Mathf.Lerp(DistanceA, DistanceB, LastBlend);
                    CachedPivotPosition = BlendedCameraPivot();
                    return BlendedDistance;
                }
                else
                {
                    Debug.Log("Flase Tween End");
                }
                if (PreviousCamera == TransitionCamera.gameObject && NextCamera == projectManager.ActiveSection.gameObject)
                {
                    PresentationSection BlendCamera = projectManager.ActiveSection;
                    float DistanceA = Vector3.Distance(MainCameraTransform.position, PreviousPresentationSection.Pivot.transform.position);
                    float DistanceB = Vector3.Distance(BlendCamera.gameObject.transform.position, BlendCamera.Pivot.transform.position);
                    BlendedDistance = Mathf.Lerp(DistanceA, DistanceB, LastBlend);
                    CachedPivotPosition = BlendedCameraPivot();
                    return BlendedDistance;
                }
                else
                {
                    Debug.Log("Flase Regular");
                }
                if (PreviousCamera.GetComponent<TweenCamera>() != null && NextCamera == PreviousPresentationSection.gameObject)
                {
                    PresentationSection BlendCamera = PreviousPresentationSection;
                    TweenSection CameraA = (brain.ActiveBlend.CamA.VirtualCameraGameObject).GetComponent<TweenSection>();
                    float DistanceA = Vector3.Distance(MainCameraTransform.position, CameraA.PivotAngle.transform.position);
                    float DistanceB = Vector3.Distance(BlendCamera.gameObject.transform.position, BlendCamera.Pivot.transform.position);
                    BlendedDistance = Mathf.Lerp(DistanceA, DistanceB, LastBlend);
                    CachedPivotPosition = BlendedCameraPivot();
                    return BlendedDistance;
                }
                else
                {
                    Debug.Log("Flase Backwards");
                }
                if (PreviousCamera == TransitionCamera.gameObject && NextCamera.GetComponent<TweenSection>() != null)
                {
                    TweenSection BlendCamera = NextCamera.GetComponent<TweenSection>();
                    float DistanceA = Vector3.Distance(MainCameraTransform.position, PreviousPresentationSection.Pivot.transform.position);
                    float DistanceB = Vector3.Distance(NextCamera.transform.position, BlendCamera.PivotAngle.position);
                    BlendedDistance = Mathf.Lerp(DistanceA, DistanceB, LastBlend);
                    CachedPivotPosition = BlendedCameraPivot();
                    return BlendedDistance;
                }
                else
                {
                    Debug.Log("Flase Beginning");
                }
            }
            else
            {
                GameObject currentActiveCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject;

                if (currentActiveCamera == TransitionCamera.gameObject)
                {
                    float DistanceA = Vector3.Distance(MainCameraTransform.position, PreviousPresentationSection.Pivot.position);
                    BlendedDistance = DistanceA;
                    CachedDistance = BlendedDistance;
                    CachedPivotPosition = BlendedCameraPivot();
                    Debug.Log("Tween is found");
                    return BlendedDistance;
                }
                if (currentActiveCamera.GetComponent<PresentationSection>() != null)
                {
                    PresentationSection BlendCamera = currentActiveCamera.GetComponent<PresentationSection>();
                    float DistanceA = Vector3.Distance(MainCameraTransform.position, BlendCamera.Pivot.position);
                    BlendedDistance = DistanceA;
                    CachedDistance = BlendedDistance;
                    CachedPivotPosition = BlendedCameraPivot();
                    return BlendedDistance;
                }
                if (currentActiveCamera.GetComponent<TweenSection>() != null)
                {
                    TweenSection BlendCamera = currentActiveCamera.GetComponent<TweenSection>();
                    float DistanceA = Vector3.Distance(currentActiveCamera.transform.position, BlendCamera.PivotAngle.position);
                    BlendedDistance = DistanceA;
                    CachedDistance = BlendedDistance;
                    CachedPivotPosition = BlendedCameraPivot();
                    return BlendedDistance;
                }
            }

            return BlendedDistance;*/
        }

        public float Blend()
        {
            float blend = brain.ActiveBlend.TimeInBlend;
            return blend;
        }

        public float LastBlend;
        public GameObject PreviousCamera;
        public GameObject NextCamera;

        public float rotX; // around x
        public float rotY; // around y
        public float clampedDistance;
        public float debugFloat;
        public float ScrollWheel;
        public float ScrollWheelChange;
        public enum RotateMethod { Mouse, Touch };
        [Tooltip("How do you like to rotate the camera")]
        public RotateMethod rotateMethod = RotateMethod.Mouse;
        //Used to keep euler angles over 180
        public static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }
        RaycastHit hit;
        private Touch touch;

        private float minXRotAngle = -80; //min angle around x axis
        private float maxXRotAngle = 80; // max angle around x axis
        private float cameraDistance;
        private float zoomDist;


        #endregion

        #region Lists & Arrays
        [HideInInspector]
        public List<Vector3> DefaultCameraPositions;
        [HideInInspector]
        public List<Vector3> DefaultCameraRotations;
        #endregion

        #region Bools
        [Space(10)]
        [Header("Bool Values")]

        //Mouse rotation related
        public static bool CameraRotating;

        //InFocus determines wether or not the user is already in one of the presentation sections and not at the main overview(First section)
        public bool InFocus;
        public bool toOrigin;
        public bool InitCamera;
        public bool cutOutTransition;
        public bool animationStopped = false;
        public static bool isTransitioning;
        public static bool isBlending;
        public bool canStopTransition;
        private bool drawLocked;

        #endregion
        void Awake()
        {
            projectManager = GameObject.FindFirstObjectByType<ProjectManager>();

            if (projectManager == null)
            {
                Debug.LogError("There is no project manager. Add a project manager!");
            }

            Application.targetFrameRate = 60;
            PrimaryFinger = Inputs.FindAction("PrimaryFingerPosition");
            SecondaryFinger = Inputs.FindAction("SecondaryFingerPosition");
            TouchContact = Inputs.FindAction("SecondaryTouchContact");
            Contact = Inputs.FindAction("Contact");
            //Shift = Inputs.FindAction("Pan");
            RotateCamera = Inputs.FindAction("Rotate Camera");
            Zoom = Inputs.FindAction("Zoom");

            TouchContact.started += TouchZoomStart;
            TouchContact.canceled += TouchZoomEnd;

            RotateCamera.performed += CameraRotation;
            Contact.canceled += StopCameraRotationDetection;

            //Shift.started += ShiftCameraStart;
            //Shift.canceled += ShiftCameraEnd;

            Zoom.performed += MouseZoom;

#if UNITY_ANDROID
#endif
           
        }

        public void StartTags()
        {
            _instance = this;

            MainCamera = Camera.main;
            projectManager.ActiveSection = projectManager.Sections[0];
            InitCamera = true;
            projectManager.ActiveSection.director.Play();
            projectManager.ActiveSection.director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            CachedPivotPosition = projectManager.ActiveSection.sectionCamera.PivotAngle.transform.position;
            //CachedDistance = Vector3.Distance(manager.ActiveSection.Camera.transform.position, manager.ActiveSection.Camera.PivotAngle.position);
           
            StartPivotPosition = projectManager.ActiveSection.sectionCamera.PivotAngle.position;

            //Values to set the camera to the default position
            slerpedValue = slerpValue;
            rotX = WrapAngle(projectManager.ActiveSection.VirtualCamera.transform.eulerAngles.x);
            rotY = WrapAngle(projectManager.ActiveSection.VirtualCamera.transform.eulerAngles.y);
            CachedDistance = Vector3.Distance(projectManager.ActiveSection.VirtualCamera.transform.position, projectManager.ActiveSection.Pivot.position);
            ZoomDistance = new Vector3(0, 0, -CachedDistance);

            if (projectManager.Sections[0].CallOutGameObjects.Count > 0)
            {
                //manager.SwitchRunTimeCallOuts(manager.Sections[0], true);
            }
            foreach (PresentationSection V in projectManager.Sections)
            {
                if(V.CallOutGameObjects.Count > 0 && V != projectManager.ActiveSection)
                {
                    for(int i = 0; i < V.CallOutGameObjects.Count; i++)
                    {
                        V.CallOutGameObjects[i].SetActive(false);
                    }
                }
            }
            if (projectManager.ActiveSection.SectionTitle != null)
            {
                uI_Manager.ChangeTitle();
            }
        }
        void Start()
        {
            StartTags();
           
            for (int i = 0; i < projectManager.Sections.Count; i++)
            {
                if (projectManager.Sections[i].sectionCamera != null)
                {
                    DefaultCameraPositions.Add(projectManager.Sections[i].sectionCamera.CameraChild.transform.position);
                    DefaultCameraRotations.Add(projectManager.Sections[i].sectionCamera.CameraChild.transform.eulerAngles);
                }
            }

        }

        public static bool CameraRotated;
        public static bool PivotShifting;
        public Vector3 ZoomDistance;


        public void ShiftCameraStart(InputAction.CallbackContext context)
        {
            PivotShifting = true;
            uI_Manager.ForceSetCursor(uI_Manager.PanCursor, new Vector2(24, 24));
        }
        public void ShiftCameraEnd(InputAction.CallbackContext context)
        {
            PivotShifting = false;
        }
        public void TouchZoomStart(InputAction.CallbackContext context)
        {
            Debug.Log("Touch Zoom Detected");
            StartCoroutine(TouchZoomDetection());
        }
        public void TouchZoomEnd(InputAction.CallbackContext context) 
        {
            Debug.Log("Touch Zoom Ended");
            StopCoroutine(TouchZoomDetection());
        }

        IEnumerator TouchZoomDetection()
        {
            float previousDistance = Vector2.Distance(PrimaryFinger.ReadValue<Vector2>(), SecondaryFinger.ReadValue<Vector2>()), distance = 0f;
            while (true)
            {
                distance = Vector2.Distance(PrimaryFinger.ReadValue<Vector2>(), SecondaryFinger.ReadValue<Vector2>());
                //zooming out

                ZoomCamera(distance);
                /*if (distance > previousDistance)
                {
                    ZoomCamera(distance);
                }
                //zooming in
                else if(distance < previousDistance)
                {
                    ZoomCamera(distance);
                }*/
                previousDistance = distance;
            }
        }
        public void MouseZoom(InputAction.CallbackContext context)
        {
            Vector2 delta = Zoom.ReadValue<Vector2>();
            ZoomCamera(delta.y);
        }

        public void ZoomCamera(float zoom)
        {
            if (!projectManager.StaticPresentation && !UI_Manager.DrawingMode && MenuManager.MouseInGameWindow)
            {
                Vector2 delta = Zoom.ReadValue<Vector2>();
                if (!isTransitioning)
                {
                    
                    //ScrollWheelChange = ScrollWheel;

                    if (clampedDistance <= ZoomMinFOV)
                    {
                        ScrollWheel -= Mathf.Clamp((zoom) / 4, -1, 0);
                    }
                    else if (clampedDistance >= ZoomMaxFOV)
                    {
                        ScrollWheel -= Mathf.Clamp((zoom) / 4, 0, 1);
                    }
                    else
                    {
                        ScrollWheel -= Mathf.Clamp((zoom) / 4, -1, 1);
                    }
                }
            }
                
        }
        public void CameraRotation(InputAction.CallbackContext context)
        {
            Vector2 delta = context.ReadValue<Vector2>();

            if (!projectManager.StaticPresentation && !UI_Manager.DrawingMode && MenuManager.MouseInGameWindow && !PopupManager.MouseInWindow && !PopupManipulator.isResizing && !PopupManipulator.isDragging && !UI_Manager.RestrictMovement || CameraRotating)
            {
                #region Logic
                //Indicate the camera has started rotating so that it does not cut out when exiting the gamewindow until the mouse is released.

                if (brain.ActiveBlend != null)
                {
                    CurrentBlend = brain.ActiveBlend.TimeInBlend;
                }

                CameraRotating = true;
                CameraRotated = true;

                MainCameraTransform.LookAt(CachedPivotPosition);

                #endregion

                #region Input Logic
                //This portion controls the camera movement INPUTS
                if (!isTransitioning)
                {
                    if (!PivotShifting)
                    {
                        if (slerpedValue == 1)
                        {
                            slerpedValue = slerpValue;
                        }
                        if (!UI_Manager.RestrictMovement && !DICOMSliderInput.wasSliding)
                        {
                            rotX += -delta.y * mouseRotateSpeed;
                            rotY += delta.x * mouseRotateSpeed;
                        }
                    }
                    else
                    {
                        //MainCameraTransform.localPosition.x += delta.x;
                    }
                }
                if (DICOMSliderInput.wasSliding)
                {
                    DICOMSliderInput.wasSliding = false;
                }

                if (isTransitioning)
                {
                    if (CurrentBlend >= 0.99f)
                    {
                        canStopTransition = true;
                    }

                    if (CurrentBlend >= 0.99f && canStopTransition && projectManager.ActiveSection.director.time != 0 && !UI_Manager.isScrubbing && !CameraRotated)
                    {
                        target = projectManager.ActiveSection.sectionCamera.PivotAngle;
                        ScrollWheel = 0;
                        ScrollWheelChange = 0;
                        if (uI_Manager.PreviousPresentationSection != null)
                        {
                            ResetPreviousCamera(uI_Manager.PreviousPresentationSection.sectionCamera.VirtualCamera);
                        }

                        isTransitioning = false;
                        canStopTransition = false;
                    }


                }


                if (rotX < minXRotAngle)
                {
                    rotX = minXRotAngle;
                }
                else if (rotX > maxXRotAngle)
                {
                    rotX = maxXRotAngle;
                }
                #endregion
            }
        }

        public void StopCameraRotationDetection(InputAction.CallbackContext context)
        {
            CameraRotating = false;

            if (MenuManager.PopoutIsSliding)
            {
                uI_Manager.m_popoutLayout.style.transitionDuration = new List<TimeValue>()
                {
                    new TimeValue(0.4f, TimeUnit.Second)
                };
                MenuManager.PopoutIsSliding= false;
            }
        }

        public float CurrentBlend;
        public Vector3  BlendedCameraPivot()
        {

            //Vector3 blend = ((MainCameraTransform.position) + (MainCameraTransform.forward * distanceBetweenCameraAndTarget()));

            Vector3 blend = new Vector3();

            if(brain.IsBlending && brain.ActiveBlend.BlendWeight >= 0 && brain.ActiveBlend.BlendWeight <= 1)
            {
                float distance = Vector3.Distance(projectManager.ActiveSection.Pivot.transform.position, uI_Manager.PreviousPresentationSection.Pivot.transform.position);
                Vector3 Direction = (projectManager.ActiveSection.Pivot.transform.position - uI_Manager.PreviousPresentationSection.Pivot.transform.position);

                float blendPercentage = brain.ActiveBlend.BlendWeight;
                Vector3 blendedPivot = uI_Manager.PreviousPresentationSection.Pivot.transform.position + (Direction * blendPercentage);

                //float blendedDistance = Mathf.Lerp(DistanceB, DistanceA, brain.ActiveBlend.BlendWeight);
                float blendedDistance = Vector3.Distance(gameObject.transform.position, blendedPivot);

                blend = ((gameObject.transform.position) + (gameObject.transform.forward * blendedDistance));
                CachedPivotPosition = blend;

                

                return blend;
 
                /*float distance = Vector3.Distance(projectManager.ActiveSection.Pivot.transform.position, PreviousPresentationSection.Pivot.transform.position);
                Vector3 Direction = (projectManager.ActiveSection.Pivot.transform.position - PreviousPresentationSection.Pivot.transform.position);

                float blendPercentage = brain.ActiveBlend.BlendWeight;
                blend = PreviousPresentationSection.Pivot.transform.position + (Direction * blendPercentage);

                CachedPivotPosition = blend;
                return blend;*/
            }
            else
            {
                return CachedPivotPosition;
            }



        }
        public static Vector3 CachedPivotPosition;
        public float CachedDistance;
        public void LockSlideForDrawing()
        {
            if(Input.GetKey(KeyCode.LeftControl) && !drawLocked)
            {

            }
            else if (drawLocked)
            {

            }
        }

        void Update()
        {
            Debug.DrawLine(MainCameraTransform.position, BlendedCameraPivot(), Color.red);
            Debug.DrawLine(blendedCamera, BlendedCameraPivot(), Color.green);


            //Updated Camera Logic
            if (!isTransitioning && !UI_Manager.RestrictMovement)
            {
                ScrollWheelChange = Mathf.Lerp(ScrollWheelChange, ScrollWheel * (ZoomSpeed + projectManager.ActiveSection.ZoomSpeedOffset), slerpedValue);
                cameraDistance = -CachedDistance;
                zoomDist = cameraDistance - + ScrollWheelChange;

                //clampedDistance = Vector3.Distance(transform.position, projectManager.ActiveSection.Camera.PivotAngle.position);

                ZoomDistance = new Vector3(0, 0, zoomDist); //assign value to the distance between the maincamera and the target
            

                Quaternion newQ; // value equal to the delta change of our mouse or touch position

                newQ = Quaternion.Euler(rotX, rotY, 0);

                cameraRot = Quaternion.Slerp(cameraRot, newQ, slerpedValue);  //let cameraRot value gradually reach newQ which corresponds to our touch

                MainCameraTransform.position = (CachedPivotPosition + cameraRot * ZoomDistance);
                Vector3 Direction = (CachedPivotPosition - MainCameraTransform.position).normalized;
                Vector3 rotate = (Quaternion.LookRotation(Direction).eulerAngles);
                //MainCameraTransform.LookAt(CachedPivotPosition);
                MainCameraTransform.eulerAngles = new Vector3(rotate.x, rotate.y, rotate.z + transform.eulerAngles.z);
            }

            if (isTransitioning)
            {
                //MainCameraTransform.transform.position = transform.position;
                //MainCameraTransform.eulerAngles = transform.eulerAngles;
            }
            if (projectManager.ActiveSection.WorldspaceLabels.Count > 0)
            {
                foreach(TextMeshPro labels in projectManager.ActiveSection.WorldspaceLabels)
                {
                    labels.transform.rotation = MainCamera.transform.rotation;
                    //labels.transform.LookAt(MainCamera.transform.position);
                }
            }
        }
       
        public void SetCinemachineCamera()
        {
            TransitionSettings.m_FocusTarget = uI_Manager.PreviousPresentationSection.VolumeSettings.m_FocusTarget;
            TransitionCamera.transform.position = MainCameraTransform.position;
            TransitionCamera.transform.rotation = MainCameraTransform.rotation;
        }

        #region Transition Functions
        //CREATE A TRANSITION TYPE ENUM FOR THE TRANSITIONER

        public static Material CurrentFadeMat;
        public static bool PopupOpen;

        
        public IEnumerator Labelsets(Color color, float Start, float Target)
        {
            float elapsedTime = 0f;
            while (elapsedTime <= labelFadeDuration)
            {
                color.a = Mathf.Lerp(Start, Target, elapsedTime / labelFadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            color.a = Target;
        }
        public void ResetPreviousCamera(CinemachineVirtualCamera Cam)
        {
            for (int i = 0; i < projectManager.Sections.Count; i++)
            {
                if (projectManager.Sections[i].sectionCamera.VirtualCamera == Cam)
                {
                    Cam.transform.position = DefaultCameraPositions[i];
                    Cam.transform.LookAt(projectManager.Sections[i].sectionCamera.PivotAngle);
/*                    MainCameraTransform.position = DefaultCameraPositions[i];
                    MainCameraTransform.LookAt(manager.Sections[i].Camera.PivotAngle);*/
                    break;
                }
            }
        }

        public void ResetCurrentCamera()
        {
            //manager.ActiveSection
        }

        public void ResetTimelineCamera()
        {
            //target = manager.ActiveSection.Camera.PivotAngle;
            //CameraMovement.isTransitioning = false;

            distanceBetweenCameraAndTarget();

            rotX = WrapAngle(blendedCameraRotation.x);
            rotY = WrapAngle(blendedCameraRotation.y);

            CachedPivotPosition = BlendedCameraPivot();
            //distanceBetweenCameraAndTarget = (Vector3.Distance(transform.position, CachedPivotPosition)) - SplitScreen.ScaleCamera;
            //CachedDistance = distanceBetweenCameraAndTarget();
            ScrollWheel = 0;
            ScrollWheelChange = 0;
            slerpedValue = 1;
            CameraRotated = false;
            isTransitioning = false;
        }
        public void LabelFadeDepth()
        {
            //Write a function that puts labels on the screen each time they are closer to the camera
        }
        #endregion

        #region Editor Code

        public static void AdjustCameraFraming(int Height, int Width)
        {
            /*Vector3 RayPoint = MainCamera.transform.forward * 1;
            Vector3 ScreenPos = MainCamera.WorldToScreenPoint(RayPoint);
            float AdjustedWidthPos = ScreenPos.x + Width;
            float AdjustedHeightPos = ScreenPos.y + Height;
            
            Vector3 WidthWorldPos = MainCamera.ScreenToWorldPoint(new Vector3(AdjustedWidthPos, RayPoint.y, RayPoint.z));
            float WidthDistance = Vector3.Distance(RayPoint, WidthWorldPos);

            Vector3 HeightWorldPos = MainCamera.ScreenToWorldPoint(new Vector3(RayPoint.x, AdjustedHeightPos, RayPoint.z));
            float HeightDistance = Vector3.Distance(RayPoint, HeightWorldPos);
            MainCamera.transform.localPosition = new Vector3(WidthDistance, HeightDistance, 0);*/
        }
#if UNITY_EDITOR
        public static void BindTimelineTracks(string SourceTrackName, GameObject obj, PlayableDirector director)
        {
            foreach (var playableAssetOutput in director.playableAsset.outputs)
            {
                if (playableAssetOutput.streamName == SourceTrackName)
                {
                    director.SetGenericBinding(playableAssetOutput.sourceObject, obj);
                }
            }
        }

        public static void BindTimelineResets(string SourceTrackName, PresentationSection section, UnityEngine.Object Binding)
        {
            foreach (var playableAssetOutput in section.director.playableAsset.outputs)
            {
                if (playableAssetOutput.streamName == SourceTrackName)
                {
                    section.director.SetGenericBinding(playableAssetOutput.sourceObject, Binding);
                }
            }
        }
        public void BindCinemachineCameras(PresentationSection section, InputManager cameraManager)
        {
            foreach (var playableAssetOutput in section.director.playableAsset.outputs)
            {
                if (playableAssetOutput.streamName == "Cinemachine Track")
                {
                    var cinemachineTrack = playableAssetOutput.sourceObject as CinemachineTrack;
                    section.director.SetGenericBinding(playableAssetOutput.sourceObject, cameraManager.GetComponent<CinemachineBrain>());

                    if (cinemachineTrack == null)
                    {
                        Debug.Log("Track is null");
                    }

/*                    if (section.SectionID != 0)
                    {
                        var SecondCinemachineShot = cinemachineTrack.GetClips().ElementAt(0).asset as CinemachineShot;
                        SecondCinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
                        section.director.SetReferenceValue(SecondCinemachineShot.VirtualCamera.exposedName, TransitionCamera);
                    }*/

                    var SecondCinemachineShot = cinemachineTrack.GetClips().ElementAt(0).asset as CinemachineShot;
                    SecondCinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
                    section.director.SetReferenceValue(SecondCinemachineShot.VirtualCamera.exposedName, TransitionCamera);

                    var FirstCinemachineShot = cinemachineTrack.GetClips().ElementAt(1).asset as CinemachineShot;
                    FirstCinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
                    section.director.SetReferenceValue(FirstCinemachineShot.VirtualCamera.exposedName, section.VirtualCamera);

                }

            }

            AssetDatabase.Refresh();
        }

        public void SetDefaultCinemachineCamera()
        {
            if(uI_Manager.PreviousPresentationSection != null)
            {
                PresentationSection previousSection = uI_Manager.PreviousPresentationSection;
                TransitionSettings.m_FocusTarget = previousSection.VolumeSettings.m_FocusTarget;
                TransitionCamera.transform.position = previousSection.VirtualCamera.transform.position;
                TransitionCamera.transform.rotation = previousSection.VirtualCamera.transform.rotation;
            }
        }




#endif
        #endregion
    }
}