
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using System;
//using FFmpegOut;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace ADM.UISystem
{
    public class UI_Manager : MonoBehaviour
    {
        #region Script Dependencies
        public UI_Manager _instance;
        public InputManager cam;
        public Effects_Manager Transition;
        public ProjectManager projectManager;
        public PresentationSection PreviousPresentationSection;
        //public CameraCapture cameraCapture;
        public DrawTextureUI drawManager;

        //This changes and sets the control panel whenever the CurrentControlPanel value is set
        private ControlPanel _currentControlPanel;

        public ControlPanel CurrentControlPanel
        {
            get => _currentControlPanel;
            set
            {
                if (_currentControlPanel != null)
                {
                    if (_currentControlPanel == value)
                        return;
                    //Unregister callbacks here
                }
                _currentControlPanel = value;
                if (_currentControlPanel == null)
                    return;
                //register callbacks here
                SetControlPanel(_currentControlPanel);
            }
        }
        private CalloutManager _currentCalloutCanvas;
        public CalloutManager currentCalloutCanvas
        {
            get => _currentCalloutCanvas;
            set
            {
                if(_currentCalloutCanvas != null)
                {
                    if (_currentCalloutCanvas == value)
                        return;
                    //unrgister callbacks here

                    //deactive the last callout canvas

                    _currentCalloutCanvas.RemoveFromClassList("activeCalloutCanvas");
                    _currentCalloutCanvas.AddToClassList("inactiveCalloutCanvas");
                    _currentCalloutCanvas.style.display = DisplayStyle.None;
                }
                _currentCalloutCanvas = value;
                if (_currentCalloutCanvas == null)
                    return;
                ChangeCalloutCanvas();
                //register callbacks here
                
            }
        }
        #endregion

        #region UIElement Dependencies
        public List<ThemeStyleSheet> themeStyleSheets = new List<ThemeStyleSheet>();
        public List<SectionListEntryController> RegisteredSections = new List<SectionListEntryController>();
        public List<UniversalRenderPipelineAsset> RenderPipelines = new List<UniversalRenderPipelineAsset>();
        public UIDocument uIDocument;
        public static VisualElement Root;
        public MenuManager menuManager;
        private SectionListController listController;
        [HideInInspector]
        public PopupManipulator popupManipulator;
        public ToolTipManipulator toolTipManipulator;
        public ResizableWindowManipulator ResizeManipulator;

        public VisualTreeAsset ExhibitButtonTemplate;
        public VisualTreeAsset ToolsToggleTemplate;
        public VisualTreeAsset CompassTemplate;
        public VisualTreeAsset PenStrokeToolTip;

        public VisualTreeAsset ButtonTemplate;
        public VisualTreeAsset SliderTemplate;
        public VisualTreeAsset ToggleTemplate;

        public ListView PopupContent;
        public Button m_recordButton;
        public PopupManager popupManager;
        public Slider TimelineSlider;
        public Toggle PlayButton;
        public Toggle PanButton;
        public Toggle PenButton;
        public Toggle m_popMessageToggle;
        public Toggle m_rememberMySettingsToggle;
        public Label SectionTitle;
        public Label ExhibitCounter;
        public Label m_versionLabel;
        public Label m_versionNotes;
        public VisualElement HelpScreenInfo;
        public Slider m_mouseSensitivitySlider;
        public Slider m_penStrokeSlider;

        public DropdownField screenSelection;
        public DropdownField ThemePicker;
        public DropdownField GraphicsPicker;

        public VisualElement ToolsManager;
        public VisualElement m_gameWindow;
        public VisualElement root;
        public VisualElement Popup;
        public VisualElement m_popupLayout;
        public VisualElement m_welcomeWindow;
        public VisualElement m_windowBackground;
        public VisualElement m_welcomeCloseButton;
        public VisualElement m_gameWindowLayout;
        public VisualElement m_scrubBarLayout;
        public VisualElement m_popoutLayout;
        public VisualElement m_popoutHandle;
        public VisualElement m_popupWindowContainer;
        public VisualElement m_popoutContainer;
        public VisualElement m_compassContainer;
        public VisualElement m_compass;
        public VisualElement m_controlsContainer;
        public VisualElement m_controlPanelTitleLayout;
        public VisualElement m_laserPointerToolTip;
        public VisualElement m_penLayout;
        public VisualElement m_markerLayout;
        public VisualElement m_currentControlPanel;
        public VisualElement TitleBackground;
        public VisualElement m_defaultSettingsButton;

        public UnityEngine.UIElements.Label m_controlPanelTitle;
        #endregion

        #region Component Dependencies
        public Material DitherHighlight;
        public GameObject MarkerList;
        

        //Cursor Textures
        public Texture2D recordCursor;
        public Texture2D DrawCursor;
        public Texture2D resizeHorizontal;
        public Texture2D resizeVertical;
        public Texture2D resizeDiagonalRight;
        public Texture2D resizeDiagonalLeft;
        public Texture2D MoveCursor;
        public Texture2D PanCursor;
        #endregion

        #region Simple DataType Values
        public string ToolsManagerQuery;
        public string currentScreen;

        public static int PreviousScreenHeight;
        public static int PreviousScreenWidth;

        public float TitleSpacing = -10;
        public float TitleOpenSpeed = 0.5f;
        public static float ScreenSize;

        public static bool isScrubbing;
        public bool quitScreen;
        private bool compassActive;
        private bool screenIsRecording = false;
        public static bool RestrictMovement;
        public static bool isFullScreen;
        public static bool DrawingMode;

        public Vector3 OriginalCameraPosition;
        public Quaternion OriginalCameraRotation;
        #endregion
        
        // *------------------------------------------------------------------------* //

        #region Monobehaviour Functions
        void Awake()
        {
            Root = uIDocument.rootVisualElement;
            projectManager = GameObject.FindFirstObjectByType<ProjectManager>();

            if (projectManager == null)
            {
                Debug.LogError("There is no project manager. Add a project manager!");
            }

            //Set the Active Presentation Section
            projectManager.ActiveSection = projectManager.Sections[0];
            PreviousPresentationSection = projectManager.Sections[0];

            if (_instance == null)
            {
                _instance = this;
            }
            
            
            root = uIDocument.rootVisualElement;
            menuManager = root.Q<MenuManager>("RootLayout");
            ToolsManager = root.Q<TejadaToolsManager>(ToolsManagerQuery);
            m_welcomeWindow = root.Q<VisualElement>("WelcomeWindowLayout");
            m_windowBackground = root.Q<VisualElement>("WindowBackground");
            m_welcomeCloseButton = root.Q<VisualElement>("WelcomeCloseButton");
            m_gameWindowLayout = root.Q<VisualElement>("GameWindowLayout");
            m_popupWindowContainer = root.Q<VisualElement>("PopupWindowContainer");
            m_popoutContainer = root.Q<VisualElement>("PopoutContainer");
            m_popupLayout = root.Q<VisualElement>("PopupWindowLayout");
            m_popoutLayout = root.Q<VisualElement>("PopoutLayout");
            m_controlsContainer = root.Q<VisualElement>("ControlsContainer");
            m_controlPanelTitleLayout = root.Q<VisualElement>("ControlPanelTitleLayout");
            m_controlPanelTitle = root.Q<Label>("ControlPanelTitle");
            m_markerLayout = root.Q<VisualElement>("MarkerLayout");
            m_defaultSettingsButton = root.Q<VisualElement>("DefaultSettingsButton");
            m_penLayout = root.Q<VisualElement>("LaserPointerTexture");
            m_popMessageToggle = root.Q<Toggle>("PopupMessageToggle");
            m_versionLabel = root.Q<Label>("VersionLabel");
            m_versionNotes = root.Q<Label>("VersionNotes");

            if(projectManager.ProjectVersionName != "")
            {
                m_versionLabel.text = projectManager.ProjectVersionName;
            }
            if (projectManager.ProjectVersionNotes != "")
            {
                m_versionNotes.text = projectManager.ProjectVersionNotes;
            }


            m_recordButton = root.Q<Button>("RecordButton");
            m_scrubBarLayout = root.Q<VisualElement>("ScrubBarLayout");
            m_rememberMySettingsToggle = root.Q<Toggle>("RememberSettingsToggle");
            m_mouseSensitivitySlider = root.Q<Slider>("MouseSensitivitySlider");
            m_laserPointerToolTip = root.Q<VisualElement>("ToolTipLaser");


            OpenWelcomeWindow();

            
            popupManager = root.Q<PopupManager>("PopupManager");

            Popup = root.Q<VisualElement>("PopupLayout");
            TimelineSlider = root.Q<UnityEngine.UIElements.Slider>("TimelineSlider");
            m_gameWindow = root.Q<VisualElement>("GameWindow");
            PlayButton = root.Q<UnityEngine.UIElements.Toggle>("PlayTimeline");
            SectionTitle = root.Q<UnityEngine.UIElements.Label>("Title");
            ExhibitCounter = root.Q<Label>("ExhibitCounter");
            
            PanButton = root.Q<UnityEngine.UIElements.Toggle>("PanToggle");
            TitleBackground = root.Q<VisualElement>("TitleBackground");

            screenSelection = root.Q<DropdownField>("ScreenSize");
            ThemePicker = root.Q<DropdownField>("Theme");
            GraphicsPicker = root.Q<DropdownField>("GraphicsPicker");


            ThemePicker?.RegisterCallback<ChangeEvent<String>>(evt =>
            {
                PickTheme(ThemePicker.index);
            });

            screenSelection?.RegisterCallback<ChangeEvent<String>>(evt =>
            {
                ChangeScreenSize(screenSelection.index);
            });

            GraphicsPicker?.RegisterCallback<ChangeEvent<String>>(evt =>
            {
                ChangeGraphicsSettings(GraphicsPicker.index);
            });
            m_defaultSettingsButton?.RegisterCallback<ClickEvent>(evt => LoadDefaultPlayerPrefs());

            PenButton = root.Q<Toggle>("PenToggle");
            HelpScreenInfo = root.Q<VisualElement>("HelpScreenInfoButton");



            m_penStrokeSlider = root.Q<Slider>("PenStrokeSlider");
            m_penStrokeSlider?.RegisterValueChangedCallback(ev => ChangePenStroke(m_penStrokeSlider.value));
            m_popMessageToggle?.RegisterCallback<MouseDownEvent>(ev => SetPopupSettings());

            HelpScreenInfo.RegisterCallback<MouseDownEvent>(evt => OpenStartInfoPanel());
            m_mouseSensitivitySlider?.RegisterValueChangedCallback(ev => ChangeMouseSensitivity(m_mouseSensitivitySlider.value));

           // m_recordButton?.RegisterCallback<ClickEvent>(evt => CaptureScreenRecording());
            m_rememberMySettingsToggle?.RegisterCallback<ClickEvent>(evt => SetRememberSettings(m_rememberMySettingsToggle.value));


            m_welcomeCloseButton.RegisterCallback<ClickEvent>(evt => CloseWelcomeWindow());
            PanButton.value = true;

            listController = new SectionListController();
            



            //Add VisualElement Manipulators to the Popup Window
            popupManipulator = new PopupManipulator(uIDocument, 25, LengthUnit.Percent, m_popupWindowContainer);
            //ResizeManipulator = new ResizableWindowManipulator(popupManager, 16, LengthUnit.Percent, m_popupLayout);

            //toolTipManipulator = new ToolTipManipulator(root, true, m_laserPointerToolTip);
            //m_penStrokeSlider.AddManipulator(toolTipManipulator);
           // Popup.AddManipulator(popupManipulator);
            popupManager.AddManipulator(popupManipulator);
          
            

            PopupContent = root.Q<ListView>("ContentView");

            //Load Player Preferences, if it is disabled load the default values
            InitializePlayerPrefs();

        }
        private void Start()
        {


            listController.InitializeCharacterList(root, ExhibitButtonTemplate, projectManager.Sections, this);

            for (int i = 0; i < projectManager.Sections.Count; i++)
            {
                if (projectManager.Sections[i] == projectManager.Sections[0])
                {
                    //RegisteredSections[i].ReferenceButton.value = true;
                    RegisteredSections[i].SectionButton.value = true;
                }
                else
                {
                    //RegisteredSections[i].ReferenceButton.value = false;
                    RegisteredSections[i].SectionButton.value = false;
                }
            }

            if (projectManager.ActiveSection.SectionMarkers.Count > 0)
            {
                foreach (MarkerScript marker in projectManager.ActiveSection.SectionMarkers)
                {
                    marker.Marker.style.display = DisplayStyle.Flex;
                }
            }
            if(projectManager.ActiveSection.CustomMenu != null)
            {
                projectManager.ActiveSection.CustomMenu.MenuElement.style.display = DisplayStyle.Flex;
                projectManager.ActiveSection.CustomMenu.MenuElement.style.opacity = 100;
            }

            projectManager.ActiveSection.director.time = projectManager.ActiveSection.director.duration;

            if (!projectManager.ActiveSection.hideTimeline)
            {
                m_scrubBarLayout.AddToClassList("activeTimeline");
                m_scrubBarLayout.RemoveFromClassList("inactiveTimeline");
            }

            if (projectManager.ActiveSection.showCompass)
            {
                m_compassContainer = CompassTemplate.CloneTree();
                m_compass = m_compassContainer.Q<VisualElement>("Compass");

                m_gameWindow.Add(m_compassContainer);

                compassActive = true;
            }

            if (projectManager.Sections[0].ControlPanelOverride.menuBindings.Count > 0)
            {
                Debug.Log("Attempted to dispaly controls");
                m_controlPanelTitleLayout.RemoveFromClassList("inactiveTimeline");
                m_controlPanelTitleLayout.AddToClassList("activeTimeline");
                SetControlPanel(projectManager.Sections[0].ControlPanelOverride);
            }
            else if(projectManager.MasterControlPanel.menuBindings.Count > 0)
            {
                m_controlPanelTitleLayout.RemoveFromClassList("inactiveTimeline");
                m_controlPanelTitleLayout.AddToClassList("activeTimeline");
                SetControlPanel(projectManager.MasterControlPanel);
            }

            if (projectManager.ActiveSection.GeneratedPopup != null)
            {
                InitializePopup(projectManager.Sections[0]);
                popupManager.OpenPopup();
                InputManager.PopupOpen = true;
            }

            foreach(PresentationSection section in projectManager.Sections)
            {
                if(section == projectManager.ActiveSection)
                {
                    VisualElement Container = section.CalloutCanvasDocument.rootVisualElement.Q<VisualElement>("RootCalloutCanvas");
                    Container.RemoveFromClassList("inactiveCanvas");
                    Container.AddToClassList("activeCanvas");
                }
                else
                {
                    VisualElement Container = section.CalloutCanvasDocument.rootVisualElement.Q<VisualElement>("RootCalloutCanvas");
                    Container.RemoveFromClassList("activeCanvas");
                    Container.AddToClassList("inactiveCanvas");
                }
            }

            if(projectManager.ActiveSection.TimelineOverride != null)
            {
                CurrentTimeline = projectManager.ActiveSection.TimelineOverride;
                ResumeTimeline();
                PauseTimeline();
            }

            cam.MainCameraTransform.localPosition = Vector3.zero;
            cam.MainCameraTransform.localEulerAngles = Vector3.zero;
        }

        void Update()
        {

            if (compassActive)
            {
                m_compass.style.rotate = new StyleRotate(new Rotate(new Angle(projectManager.ActiveSection.VirtualCamera.transform.eulerAngles.y, AngleUnit.Degree)));
            }

            if (MenuManager.PopoutIsSliding)
            {
                float percent = ((Screen.width - Input.mousePosition.x) / Screen.width) * 100;

                Debug.Log(new Vector2(Screen.width, (Screen.width - Input.mousePosition.x)));
                //Debug.Log(root.worldBound.xMax - Input.mousePosition.x));


                m_popoutLayout.style.minWidth = new Length(percent, LengthUnit.Percent);
                m_popoutLayout.style.maxWidth = new Length(percent, LengthUnit.Percent);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                ScreenCapture.CaptureScreenshot("still" + ".jpeg");
                Debug.Log("Screen has been captured");
            }
            if (Input.GetKeyDown(KeyCode.O))
            {


            }

            if (!InputManager.isTransitioning)
            {
                if(projectManager.ActiveSection.CalloutBindings.Count > 0)
                {
                    foreach (CalloutBinding binding in projectManager.ActiveSection.CalloutBindings)
                    {
                        float opacity = InputManager.CalculateRotationFactor(binding.transform.position, InputManager.MainCamera);

                        binding.calloutContainer.style.opacity = opacity;
                    }
                }
            }

            if (UnityEngine.Application.isPlaying)
            {

#if UNITY_EDITOR

#endif

                //UILayerDetection();

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (isFullScreen)
                    {
                        Screen.SetResolution(PreviousScreenWidth, PreviousScreenHeight, false);
                        menuManager.m_FullScreenButton.value = false;
                        menuManager.DisplayPopupWarning("Fullscreen disabled", 2f);
                        Screen.fullScreen = false;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ResumeOrPauseTimeline();
                }

                UpdateTimeline();


            }

        }
        #endregion

        #region UIelement Callback Functions
        private bool controlsWereAdded;

        public static void GameWindowEditMode(bool value)
        {
            VisualElement gameWindow = Root.Q<VisualElement>("GameWindow");
            if (value == true)
            {
                gameWindow.RemoveFromClassList("defualt_gameWindow");
                gameWindow.AddToClassList("recording_gameWindow");
            }
            else
            {
                gameWindow.RemoveFromClassList("recording_gameWindow");
                gameWindow.AddToClassList("default_gameWindow");
            }
        }
        public void ChangeCalloutCanvas()
        {
            _currentCalloutCanvas.style.display = DisplayStyle.Flex;

            _currentCalloutCanvas.RemoveFromClassList("inactiveCalloutCanvas");
            _currentCalloutCanvas.AddToClassList("activeCalloutCanvas");
        }

        public void ResetCamera()
        {
            projectManager.ActiveSection.sectionCamera.transform.position = OriginalCameraPosition;
            projectManager.ActiveSection.sectionCamera.transform.rotation = OriginalCameraRotation;
        }
        public void SetPopupSettings()
        {
            if (Application.isPlaying)
            {
                if (canLoadDefaultSettings)
                {
                    m_popMessageToggle.value = projectManager.DefaultPopupWarningSettings;
                }
                if (canLoadPlayerPrefs)
                {
                    if (PlayerPrefs.GetString("PopupMessageWarning") == "True")
                    {
                        m_popMessageToggle.value = true;
                    }
                    if (PlayerPrefs.GetString("PopupMessageWarning") == "False")
                    {
                        m_popMessageToggle.value = false;
                    }
                }
                else if (m_rememberMySettingsToggle.value)
                {
                    if (m_popMessageToggle.value)
                    {
                        PlayerPrefs.SetString("PopupMessageWarning", "True");
                    }
                    else
                    {
                        PlayerPrefs.SetString("PopupMessageWarning", "False");
                    }
                }
            }
            
            
        }

        public void SetControlPanel(ControlPanel controlPanel)
        {
            if (projectManager.MasterControlPanel.menuBindings.Count > 0 && projectManager.ActiveSection.GeneratedControls.Count == 0 || projectManager.ActiveSection.GeneratedControls.Count > 0 && projectManager.ActiveSection.OverrideType == PresentationSection.ControlPanleOverrideType.ReplaceControls)
            {
                if (controlsWereAdded)
                {
                    foreach(VisualElement control in PreviousPresentationSection.GeneratedControls)
                    {
                        control.RemoveFromHierarchy();
                    }
                    controlsWereAdded = false;
                }

                if (m_currentControlPanel != null)
                {
                    m_currentControlPanel.RemoveFromHierarchy();
                    m_currentControlPanel = null;
                }
                m_currentControlPanel = controlPanel.GeneratedContainer;

                m_controlPanelTitle.text = controlPanel.ControlPanelTitle;
                m_controlsContainer.Add(m_currentControlPanel);
            }

            if (projectManager.ActiveSection.GeneratedControls.Count > 0 && projectManager.ActiveSection.OverrideType == PresentationSection.ControlPanleOverrideType.AddControls)
            {
                controlsWereAdded = true;
                if(m_currentControlPanel != projectManager.MasterControlPanel.GeneratedContainer)
                {
                    if (m_currentControlPanel != null)
                    {
                        m_currentControlPanel.RemoveFromHierarchy();
                        m_currentControlPanel = null;
                    }
                    m_currentControlPanel = projectManager.MasterControlPanel.GeneratedContainer;

                    m_controlPanelTitle.text = projectManager.MasterControlPanel.ControlPanelTitle;
                    m_controlsContainer.Add(m_currentControlPanel);
                }

                foreach (VisualElement control in projectManager.ActiveSection.GeneratedControls)
                {
                    m_currentControlPanel.Add(control);
                }
                
            }

            
        }
        /// <summary>
        /// Takes in an instance of a ControlPanel class and generates a VisualElement container with controls built from UXML templates
        /// </summary>
        /// <returns></returns>
        public VisualElement GenerateControlPanel(ControlPanel controls)
        {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            foreach (MenuBinding binding in controls.menuBindings)
            {


                if (binding.selectedType == MenuBinding.Type.Booleans)
                {
                    VisualElement ToggleContainer = ToggleTemplate.CloneTree();
                    Toggle Toggle = ToggleContainer.Q<Toggle>("Toggle");
                    Toggle.value = binding.InitialBooleanValue;

                    GameObject go = binding.GameObjectReference;
                    UnityEngine.UIElements.Label title = ToggleContainer.Q<UnityEngine.UIElements.Label>("Title");

                    if (binding.ControlTitle == null)
                    {
                        Debug.LogWarning("Your control has no title");
                    }
                    title.text = binding.ControlTitle;
                    Toggle.RegisterCallback<ClickEvent>(evt => UI_Manager.GameObjectSwitch(go, Toggle.value));

                    container.Add(ToggleContainer);
                }
                if (binding.selectedType == MenuBinding.Type.Markers)
                {
                    VisualElement ToggleContainer = ToggleTemplate.CloneTree();
                    Toggle Toggle = ToggleContainer.Q<Toggle>("Toggle");
                    Toggle.value = binding.InitialBooleanValue;

                    UnityEngine.UIElements.Label title = ToggleContainer.Q<UnityEngine.UIElements.Label>("Title");

                    if (binding.ControlTitle == null)
                    {
                        Debug.LogWarning("Your control has no title");
                    }
                    title.text = binding.ControlTitle;
                    Toggle.RegisterCallback<ClickEvent>(evt => ToggleMarkers(Toggle.value));

                    container.Add(ToggleContainer);
                }
                if (binding.selectedType == MenuBinding.Type.WorldSpaceLabels)
                {
                    VisualElement ToggleContainer = ToggleTemplate.CloneTree();
                    Toggle Toggle = ToggleContainer.Q<Toggle>("Toggle");
                    Toggle.value = binding.InitialBooleanValue;

                    UnityEngine.UIElements.Label title = ToggleContainer.Q<UnityEngine.UIElements.Label>("Title");

                    if (binding.ControlTitle == null)
                    {
                        Debug.LogWarning("Your control has no title");
                    }
                    title.text = binding.ControlTitle;
                    Toggle.RegisterCallback<ClickEvent>(evt => ToggleWSlabels(Toggle.value));

                    container.Add(ToggleContainer);
                }
                if (binding.selectedType == MenuBinding.Type.Visbility)
                {
                    VisualElement ToggleContainer = ToggleTemplate.CloneTree();
                    Toggle Toggle = ToggleContainer.Q<Toggle>("Toggle");
                    Toggle.value = binding.InitialBooleanValue;

                    UnityEngine.UIElements.Label title = ToggleContainer.Q<UnityEngine.UIElements.Label>("Title");

                    if (binding.ControlTitle == null)
                    {
                        Debug.LogWarning("Your control has no title");
                    }
                    title.text = binding.ControlTitle;
                    Toggle.RegisterCallback<ClickEvent>(evt => ToggleRendererFeature(Toggle.value));

                    container.Add(ToggleContainer);
                }
                if (binding.selectedType == MenuBinding.Type.SectionSwitch)
                {
                    VisualElement buttonContainer = ButtonTemplate.CloneTree();
                    Button button = buttonContainer.Q<Button>("Button");
                    button.text = binding.ControlTitle;

                    button.RegisterCallback<ClickEvent>(evt => SectionSwitch(binding.SectionReference));
                    container.Add(buttonContainer);

                }

            }



            return container;
        }
        /// <summary>
        /// Takes in an instance of a ControlPanel class and generates just the individual controls from UXML Templates and places them in a list to be added to a container.
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        public void GenerateControls(ControlPanel controls, PresentationSection section)
        {
            foreach (MenuBinding binding in controls.menuBindings)
            {

                if (binding.selectedType == MenuBinding.Type.Booleans)
                {
                    VisualElement ToggleContainer = ToggleTemplate.CloneTree();
                    Toggle Toggle = ToggleContainer.Q<Toggle>("Toggle");
                    Toggle.value = binding.InitialBooleanValue;

                    GameObject go = binding.GameObjectReference;
                    UnityEngine.UIElements.Label title = ToggleContainer.Q<UnityEngine.UIElements.Label>("Title");

                    if (binding.ControlTitle == null)
                    {
                        Debug.LogWarning("Your control has no title");
                    }
                    title.text = binding.ControlTitle;
                    Toggle.RegisterCallback<ClickEvent>(evt => UI_Manager.GameObjectSwitch(go, Toggle.value));

                    section.GeneratedControls.Add(ToggleContainer);
                }
                if (binding.selectedType == MenuBinding.Type.Markers)
                {
                    VisualElement ToggleContainer = ToggleTemplate.CloneTree();
                    Toggle Toggle = ToggleContainer.Q<Toggle>("Toggle");
                    Toggle.value = binding.InitialBooleanValue;

                    UnityEngine.UIElements.Label title = ToggleContainer.Q<UnityEngine.UIElements.Label>("Title");

                    if (binding.ControlTitle == null)
                    {
                        Debug.LogWarning("Your control has no title");
                    }
                    title.text = binding.ControlTitle;
                    Toggle.RegisterCallback<ClickEvent>(evt => ToggleMarkers(Toggle.value));

                    section.GeneratedControls.Add(ToggleContainer);
                }
                if (binding.selectedType == MenuBinding.Type.WorldSpaceLabels)
                {
                    VisualElement ToggleContainer = ToggleTemplate.CloneTree();
                    Toggle Toggle = ToggleContainer.Q<Toggle>("Toggle");
                    Toggle.value = binding.InitialBooleanValue;

                    UnityEngine.UIElements.Label title = ToggleContainer.Q<UnityEngine.UIElements.Label>("Title");

                    if (binding.ControlTitle == null)
                    {
                        Debug.LogWarning("Your control has no title");
                    }
                    title.text = binding.ControlTitle;
                    Toggle.RegisterCallback<ClickEvent>(evt => ToggleWSlabels(Toggle.value));

                    section.GeneratedControls.Add(ToggleContainer);
                }
                if (binding.selectedType == MenuBinding.Type.Visbility)
                {
                    VisualElement ToggleContainer = ToggleTemplate.CloneTree();
                    Toggle Toggle = ToggleContainer.Q<Toggle>("Toggle");
                    Toggle.value = binding.InitialBooleanValue;

                    UnityEngine.UIElements.Label title = ToggleContainer.Q<UnityEngine.UIElements.Label>("Title");

                    if (binding.ControlTitle == null)
                    {
                        Debug.LogWarning("Your control has no title");
                    }
                    title.text = binding.ControlTitle;
                    Toggle.RegisterCallback<ClickEvent>(evt => ToggleRendererFeature(Toggle.value));

                    section.GeneratedControls.Add(ToggleContainer);
                }
                if (binding.selectedType == MenuBinding.Type.SectionSwitch)
                {
                    VisualElement buttonContainer = ButtonTemplate.CloneTree();
                    Button button = buttonContainer.Q<Button>("Button");
                    button.text = binding.ControlTitle;

                    button.RegisterCallback<ClickEvent>(evt => SectionSwitch(binding.SectionReference));
                    section.GeneratedControls.Add(buttonContainer);

                }

            }
        }
        /*public void ReadjustPopout(OnMouseDrag evt)
        {
            float MousePosition = evt.position.x;

            //float WindowWidth = this.worldBound.xMax 

            m_popoutLayout.style.minWidth = new Length(MousePosition, LengthUnit.Pixel);
            Debug.Log("Popout Readjusting");
        }*/

        public void ChangeMouseSensitivity(float value)
        {
            cam.mouseRotateSpeed = value;
            float WindowsSensitivity = 9.14f;
            float MacSensitivity = 14.4f;
            cam.ZoomSpeed = 1 + (value * WindowsSensitivity);
#if UNITY_STANDALONE_OSX
            cam.ZoomSpeed = 1 + (value * MacSensitivity);
#endif

            if (Application.isPlaying)
            {
                if (m_rememberMySettingsToggle.value)
                {
                    mouseSensitivitySetting = value;
                    PlayerPrefs.SetFloat("MouseSensitivity", value);
                }
                if (canLoadDefaultSettings)
                {
                    m_mouseSensitivitySlider.value = value;
                }
            }
            
        }

        public void ChangePenStroke(float value)
        {
            drawManager.Size = value;


            if (Application.isPlaying)
            {
                drawManager.ChangeSize();
                if (m_rememberMySettingsToggle.value)
                {
                    penSizeSetting = value;
                    PlayerPrefs.SetFloat("PenStroke", value);
                }
                if (canLoadDefaultSettings)
                {
                    m_penStrokeSlider.value = value;
                }
            }
        }

        public void SetRememberSettings(bool value)
        {
            if (value)
            {
                PlayerPrefs.SetString("RememberSettings", "True");

                PlayerPrefs.SetInt("Graphics", GraphicsPicker.index);
                PlayerPrefs.SetInt("Theme", ThemePicker.index);
                PlayerPrefs.SetInt("ScreenSize", screenSelection.index);
                PlayerPrefs.SetFloat("MouseSensitvity", m_mouseSensitivitySlider.value);
                PlayerPrefs.SetFloat("PenStroke", m_penStrokeSlider.value);
                if (m_popMessageToggle.value)
                {
                    PlayerPrefs.SetString("PopupMessageWarning", "True");
                }
                else
                {
                    PlayerPrefs.SetString("PopupMessageWarning", "Flase");
                }
            }
            else
            {
                PlayerPrefs.SetString("RememberSettings", "False");
            }
        }

        /*public void CaptureScreenRecording()
        {
            if (screenIsRecording)
            {
                m_gameWindow.RemoveFromClassList("recording_gameWindow");
                m_gameWindow.AddToClassList("default_gameWindow");

                m_recordButton.text = "Capture Recording";
                cameraCapture.enabled = false;
                screenIsRecording = false;
                return;
            }
            if (!screenIsRecording)
            {
                m_gameWindow.RemoveFromClassList("default_gameWindow");
                m_gameWindow.AddToClassList("recording_gameWindow");
                m_recordButton.text = "Stop Recording";
                cameraCapture.enabled = true;
                screenIsRecording = true;
            }
        }*/

        public void PickTheme(int Selection)
        {
            if (themeStyleSheets.Count > 0)
            {
                ThemeSetting = Selection;
                if (Selection == 0)
                {
                    uIDocument.panelSettings.themeStyleSheet = themeStyleSheets[0];
                }
                if (Selection == 1)
                {
                    uIDocument.panelSettings.themeStyleSheet = themeStyleSheets[1];
                }
            }

            if (Application.isPlaying)
            {
                if (m_rememberMySettingsToggle.value)
                {
                    ThemeSetting = Selection;
                    PlayerPrefs.SetFloat("Theme", Selection);
                }
                if (canLoadDefaultSettings)
                {
                    ThemePicker.index = Selection;
                }
            }
        }
        public void ChangeScreenSize(int Selection)
        {
            Debug.Log("Selection");
            screenSetting = Selection;
            if (Selection == 1)
            {
                ScreenSize = 0.6f;
                uIDocument.panelSettings.scale = 0.6f;
            }
            if (Selection == 0)
            {
                ScreenSize = 0.4f;
                uIDocument.panelSettings.scale = 0.4f;
            }
            if (Selection == 2)
            {
                ScreenSize = 1f;
                uIDocument.panelSettings.scale = 1f;
            }
            if (Selection == 3)
            {
                ScreenSize = 1.5f;
                uIDocument.panelSettings.scale = 1.5f;
            }

            if (Application.isPlaying)
            {
                if (m_rememberMySettingsToggle.value)
                {
                    ThemeSetting = Selection;
                    PlayerPrefs.SetFloat("ScreenSize", Selection);
                }
                if (canLoadDefaultSettings)
                {
                    screenSelection.index = Selection;
                }
            }
        }

        public void ChangeGraphicsSettings(int Selection)
        {
            graphicSetting = Selection;
            if(Selection == 0)
            {
                GraphicsSettings.renderPipelineAsset = RenderPipelines[0];
            }
            if(Selection == 1)
            {
                GraphicsSettings.renderPipelineAsset = RenderPipelines[1];
            }
            if(Selection == 2)
            {
                GraphicsSettings.renderPipelineAsset = RenderPipelines[2];
            }
            if(Selection == 3)
            {
                GraphicsSettings.renderPipelineAsset = RenderPipelines[3];
            }

            if (Application.isPlaying)
            {
                if (m_rememberMySettingsToggle.value)
                {
                    ThemeSetting = Selection;
                    PlayerPrefs.SetFloat("Graphics", Selection);
                }
                if (canLoadDefaultSettings)
                {
                    GraphicsPicker.index = Selection;
                }
            }
        }

        public void RestrictMouseMovement()
        {
            if (!InputManager.CameraRotating)
            {
                RestrictMovement = true;
            }
        }

        public void KillApplication()
        {
            UnityEngine.Application.Quit();
        }

        public void OpenWelcomeWindow()
        {
            m_windowBackground.RemoveFromClassList("inactiveWindow");
            m_windowBackground.AddToClassList("activeBackground");
            m_welcomeWindow.RemoveFromClassList("inactiveWindow");
            m_welcomeWindow.AddToClassList("activeWindow");
        }

        public void OpenStartInfoPanel()
        {
            m_welcomeWindow.RemoveFromClassList("activeWindow");
            m_welcomeWindow.AddToClassList("inactiveWindow");
            VisualElement m_helpWindow = root.Q<VisualElement>("HelpWindow");
            m_helpWindow.RemoveFromClassList("inactiveWindow");
            m_helpWindow.AddToClassList("activeWindow");
        }
        public void CloseWelcomeWindow()
        {
            m_windowBackground.RemoveFromClassList("activeBackground");
            m_windowBackground.AddToClassList("inactiveWindow");
            m_welcomeWindow.RemoveFromClassList("activeWindow");
            m_welcomeWindow.AddToClassList("inactiveWindow");
        }

        public void NextSection()
        {
            if (projectManager.ActiveSection.SectionID != projectManager.Sections.Last().SectionID)
            {
                SetPresentationSection(projectManager.Sections[projectManager.ActiveSection.SectionID + 1]);
                Debug.Log(projectManager.ActiveSectionIndex);
            }
            else
            {
                SetPresentationSection(projectManager.Sections[0]);
            }
            Debug.Log("Clicky");
        }
        public void PreviousSection()
        {
            if (projectManager.ActiveSection.SectionID != 0)
            {
                SetPresentationSection(projectManager.Sections[projectManager.ActiveSection.SectionID - 1]);
            }
            else
            {
                SetPresentationSection(projectManager.Sections.Last());
            }
        }

        #region Timeline Scrubber Management
        public PlayableDirector CurrentTimeline;
        public bool InitializeCinemachineVariables;
        public float actTime;
        public bool canScrub;
        public bool TimelineScrubberisSelected;
        public bool TimelineActive;
        public bool TimelinePaused;
        public bool CheckForEnd;
        public bool CheckForTransitionEnd;
        public bool TimelineEnded;
        public bool FreezeTimeline = false;

        public void UpdateTimeline()
        {

            if (TimelineActive)
            {
                if (CheckForEnd)
                {
                    if(MenuManager.CanScrubTimeline)
                    {
                        Input.GetMouseButtonDown(0);
                        {
                            FreezeTimeline = true;
                        }
                    }
                    if (FreezeTimeline)
                    {
                        Input.GetMouseButtonUp(0);
                        {
                            FreezeTimeline = false;
                        }
                    }
                    if (!FreezeTimeline)
                    {
                        float ProgressValue = (float)(CurrentTimeline.time / CurrentTimeline.duration);
                        TimelineSlider.value = ProgressValue;

                        if (CurrentTimeline.time == CurrentTimeline.duration)
                        {
                            PauseTimeline();
                            PlayButton.value = false;
                            TimelineEnded = true;
                            TimelineActive = false;
                            CheckForEnd = false;
                        }
                    }
                    
                }
                if (CheckForTransitionEnd)
                {
                    if (projectManager.ActiveSection.director.time == projectManager.ActiveSection.director.duration)
                    {
                        projectManager.ActiveSection.director.Pause();
                        projectManager.ActiveSection.director.playableGraph.GetRootPlayable(0).SetSpeed(1);
                        cam.ResetTimelineCamera();
                        InputManager.CachedPivotPosition = cam.BlendedCameraPivot();
                        InputManager.isTransitioning = false;
                        CheckForTransitionEnd = false;
                    }
                }

            }
            if (isScrubbing)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    SetSlider();
                    isScrubbing = false;
                }
            }
        }
        public void UIScrubTimeline()
        {
            if (projectManager.ActiveSection.TimelineOverride == null)
            {
                RestrictMovement = true;
                if (cam.brain.IsBlending && cam.brain.ActiveBlend.BlendWeight >= 0 && cam.brain.ActiveBlend.BlendWeight <= 1 && !InitializeCinemachineVariables)
                {
                    cam.currentBrainBlend = cam.brain.ActiveBlend.BlendWeight;
                    cam.startCameraPosition = cam.brain.ActiveBlend.CamA.VirtualCameraGameObject.transform.position;
                    cam.secondCameraPosition = cam.brain.ActiveBlend.CamB.VirtualCameraGameObject.transform.position;
                }
            }
            if (!TimelineActive)
            {
                
                float maxtime = (float)CurrentTimeline.duration;
                actTime = TimelineSlider.value * maxtime;
                CurrentTimeline.time = actTime;

                //CurrentTimeline.RebuildGraph();
                CurrentTimeline.Play();
                CurrentTimeline.playableGraph.GetRootPlayable(0).SetSpeed(0);

                if (CurrentTimeline.time != CurrentTimeline.duration)
                {
                    TimelineEnded = false;
                }
                if (projectManager.ActiveSection.TimelineOverride == null)
                {
                    InputManager.CachedPivotPosition = cam.BlendedCameraPivot();
                    GrabMousePosition();

                }
                TimelineActive = false;
                canScrub = false;
                isScrubbing = true;
                PlayButton.value = false;




            }
        }
        public void GrabMousePosition()
        {
            InputManager.isTransitioning = true;

            cam.MainCameraTransform.localPosition = Vector3.zero;
            cam.MainCameraTransform.localEulerAngles = Vector3.zero;
        }
        public void TimelineSkip()
        { 
            if (Input.GetMouseButton(0) && isScrubbing)
            {
                TimelineScrubberisSelected = true;
                if (!canScrub)
                {
                    if (projectManager.ActiveSection.TimelineOverride == null)
                    {
                        GrabMousePosition();
                    }
                    TimelineActive = false;

                    canScrub = true;
                }
            }
            if (TimelineScrubberisSelected && Input.GetMouseButtonUp(0))
            {
                isScrubbing = false;
                canScrub = false;
                if (projectManager.ActiveSection.TimelineOverride == null)
                {
                    cam.ResetTimelineCamera();
                }

                CurrentTimeline.Pause();
                CurrentTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
                TimelineScrubberisSelected = false;
            }

        }
        public void SetSlider()
        {
            if(projectManager.ActiveSection.TimelineOverride == null)
            {
                if (InputManager.isBlending)
                {
                    cam.CachedDistance = cam.distanceBetweenCameraAndTarget();
                    InputManager.isBlending = false;
                }
                RestrictMovement = false;
                cam.ResetTimelineCamera();
            }
            isScrubbing = false;
            canScrub = false;
            CurrentTimeline.Pause();
            CurrentTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
            TimelineScrubberisSelected = false;
        }
        public void PlayTimeline()
        {
            CurrentTimeline.Play();
            CurrentTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
            TimelineActive = true;
            CheckForEnd = true;
            if(projectManager.ActiveSection.TimelineOverride == null)
            {
                cam.transform.position = cam.MainCameraTransform.position;
                cam.transform.position = cam.MainCameraTransform.eulerAngles;
                cam.MainCameraTransform.localPosition = Vector3.zero;
                cam.MainCameraTransform.localEulerAngles = Vector3.zero;
                InputManager.isTransitioning = true;
            }
        }

        public void ResumeTimeline()
        {
            PlayButton.value = true;
            if (TimelineEnded && !TimelineScrubberisSelected)
            {
                CurrentTimeline.time = 0;
                //CurrentTimeline.RebuildGraph();
                CurrentTimeline.Pause();
                CurrentTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
                TimelineActive = true;
                TimelineEnded = false;
            }

            if(projectManager.ActiveSection.TimelineOverride != null)
            {
                CurrentTimeline.Play();
                CurrentTimeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
                TimelineActive = true;
                CheckForEnd = true;
            }

            if (CurrentTimeline.time == CurrentTimeline.duration)
            {
                InputManager.isTransitioning = true;
                cam.MainCameraTransform.localPosition = Vector3.zero;
                cam.MainCameraTransform.localEulerAngles = Vector3.zero;
            }
        }

        public void PauseTimeline()
        {
            RestrictMovement = false;
            PlayButton.value = false;
            if (CurrentTimeline.time == CurrentTimeline.duration)
            {
                if(projectManager.ActiveSection.TimelineOverride == null)
                {
                    if (cam.brain.ActiveBlend != null)
                    {
                        cam.CachedDistance = cam.distanceBetweenCameraAndTarget();
                        InputManager.CameraRotated = false;
                    }
                }
            }
            SetSlider();
            CurrentTimeline.Pause();
            TimelineActive = false;
            TimelineEnded = false;
        }
        
        public void ResumeOrPauseTimeline()
        {
            if (projectManager.ActiveSection.hideTimeline)
            {
                return;
            }

            Debug.Log("TimelinePLaying");
            if (TimelineActive)
            {
                if(projectManager.ActiveSection.TimelineOverride == null)
                {
                    cam.ResetTimelineCamera();
                    InputManager.CameraRotated = false;
                }
                PauseTimeline();
                TimelineActive = false;
                TimelineEnded = false;
                return;
            }
            if (!TimelineActive)
            {
                ResumeTimeline();
                return;
            }

        }
        #endregion

        #endregion

        #region Cursor Override Functions

        public void ForceSetCursor(Texture2D cursor, Vector2 Hotspot)
        {
            UnityEngine.Cursor.SetCursor(cursor, Hotspot, CursorMode.Auto);
        }

        public void SetHorizontalResizeCursor()
        {
            ForceSetCursor(resizeHorizontal, new Vector2(24, 24));
        }
        public void SetVerticalResizeCursor()
        {
            ForceSetCursor(resizeVertical, new Vector2(24, 24));
        }
        public void SetDiagonalRightResizeCursor()
        {
            ForceSetCursor(resizeDiagonalRight, new Vector2(24, 24));
        }
        public void SetDiagonalLeftResizeCursor()
        {
            ForceSetCursor(resizeDiagonalLeft, new Vector2(24, 24));
        }
        public void SetMoveCursor()
        {
            ForceSetCursor(MoveCursor, new Vector2(24, 24));
        }
        public void ResetCursor()
        {
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        #endregion

        #region PresentationSection Management 
        public void SetPresentationSection(PresentationSection targetSection)
        {            
            if (targetSection != projectManager.ActiveSection)
            {
                //Establish the original position of the Camera


                //targetSection.Markers.gameObject.SetActive(true);

                PresentationSection activeSection = targetSection;
                PreviousPresentationSection = projectManager.ActiveSection;
                ExhibitCounter.text = (targetSection.SectionID + 1).ToString() + ".";
                
                if(PreviousPresentationSection.CameraManipulatorType == PresentationSection.CameraManipulator.POV)
                {
                    InputManager.MainCamera.transform.localEulerAngles = Vector3.zero;

                    cam.MainCameraTransform.transform.localEulerAngles = Vector3.zero;
                }

                if (UI_Manager.DrawingMode)
                {
                    menuManager.EnablePan();
                }

                //Select the corresponding UXML Buttons
                for(int i = 0; i < projectManager.Sections.Count; i++)
                {
                    if (projectManager.Sections[i] == targetSection)
                    {
                        //RegisteredSections[i].ReferenceButton.value = true;
                        RegisteredSections[i].SectionButton.value = true;
                    }
                    else
                    {
                        //RegisteredSections[i].ReferenceButton.value = false;
                        RegisteredSections[i].SectionButton.value = false;
                    }
                }


                if (activeSection.hideTimeline && !PreviousPresentationSection.hideTimeline)
                {

                    if (!PreviousPresentationSection.hideTimeline)
                    {
                        m_scrubBarLayout.RemoveFromClassList("activeTimeline");
                        m_scrubBarLayout.AddToClassList("inactiveTimeline");
                    }
                }
                if(!activeSection.hideTimeline) 
                {
                    if(activeSection.TimelineOverride != null)
                    {
                        if(PreviousPresentationSection.TimelineOverride != null)
                        {
                            if (activeSection.TimelineOverride != PreviousPresentationSection.TimelineOverride)
                            {
                                CurrentTimeline = activeSection.TimelineOverride;
                            }
                        }
                        else
                        {
                            CurrentTimeline = activeSection.TimelineOverride;
                        }
                    }
                    else
                    {
                        CurrentTimeline = activeSection.director;
                    }
                    if (PreviousPresentationSection.hideTimeline)
                    {
                        m_scrubBarLayout.AddToClassList("activeTimeline");
                        m_scrubBarLayout.RemoveFromClassList("inactiveTimeline");
                    }
                }

                

                if (targetSection.PopupWindowContent != null)
                {
                    InitializePopup(targetSection);
                    if (targetSection.PopupWindowContent == null)
                    {
                        if (popupManager.m_popoutToggle.value)
                        {
                            Debug.Log("Popout revived");
                            m_popoutLayout.RemoveFromClassList("inactivePopoutWindow");
                            m_popoutLayout.AddToClassList("activePopoutWindow");
                        }
                    }

                    if (PreviousPresentationSection.PopupWindowContent == null)
                    {
                        /*if (popupManipulator.Parent.style.flexDirection == FlexDirection.ColumnReverse)
                        {
                            popupManager.transform.position = new Vector3((popupManipulator.Parent.worldBound.width - popupManager.transform.position.x), (popupManipulator.Parent.worldBound.height - popupManager.transform.position.y), 0);
                        }
                        if (popupManipulator.Parent.style.flexDirection == FlexDirection.RowReverse)
                        {
                            popupManager.transform.position = Vector3.zero;
                        }
                        if (popupManipulator.Parent.style.flexDirection == FlexDirection.Column || popupManipulator.Parent.style.flexDirection == FlexDirection.Row)
                        {
                            popupManager.transform.position = new Vector3((popupManipulator.Parent.worldBound.width - popupManager.transform.position.x), 0, 0);
                        }*/
                        popupManager.ZeroOutPopup();

                        popupManager.OpenPopup();
                        InputManager.PopupOpen = true;
                    }
                }

                if (PreviousPresentationSection.PopupWindowContent != null)
                {
                    if(targetSection.PopupWindowContent == null)
                    {
                        popupManager.ClosePopup();
                        InputManager.PopupOpen = false;
                    }
                }

                if (targetSection.showCompass && !PreviousPresentationSection.showCompass)
                {
                    m_compassContainer = CompassTemplate.CloneTree();
                    m_compass = m_compassContainer.Q<VisualElement>("Compass");

                    m_gameWindow.Add(m_compassContainer);

                    compassActive = true;
                }

                if(PreviousPresentationSection.showCompass && !targetSection.showCompass)
                {
                    m_compass.RemoveFromHierarchy();

                    m_gameWindow.Add(m_compass);

                    compassActive = true;
                }

                if(PreviousPresentationSection.PopupWindowContent!= null)
                {
                    m_popoutLayout.style.minWidth = StyleKeyword.Null;
                    m_popoutLayout.style.maxWidth = StyleKeyword.Null;
                    if (popupManager.m_popoutToggle.value && targetSection.PopupWindowContent == null)
                    {
                        m_popoutLayout.AddToClassList("inactivePopoutWindow");
                        m_popoutLayout.RemoveFromClassList("activePopoutWindow");
                    }
                }


                VisualElement Container = projectManager.ActiveSection.CalloutCanvasDocument.rootVisualElement.Q<VisualElement>("RootCalloutCanvas");
                Container.RemoveFromClassList("activeCanvas");
                Container.AddToClassList("inactiveCanvas");

             
                targetSection.director.RebuildGraph();
                projectManager.ActiveSection = targetSection;
                ChangeTitle();



                if (targetSection.WorldCallouts == null && PreviousPresentationSection.WorldCallouts != null)
                {
                    if (PreviousPresentationSection.WorldCallouts != null)
                    {
                        StartCoroutine(ResetWorldLabels());
                    }
                }
                else
                {
                    StartCoroutine(ToggleWorldLabels(0, 1, 0.3f));
                }

                if (!menuManager.m_LabelToggle.value)
                {
                    VisualElement ActiveContainer = projectManager.ActiveSection.CalloutCanvasDocument.rootVisualElement.Q<VisualElement>("RootCalloutCanvas");
                    ActiveContainer.RemoveFromClassList("inactiveCanvas");
                    ActiveContainer.AddToClassList("activeCanvas");
                }
                

                if (targetSection.ControlPanelOverride.menuBindings.Count > 0)
                {
                    if (PreviousPresentationSection.ControlPanelOverride.menuBindings.Count == 0)
                    {
                        m_controlPanelTitleLayout.RemoveFromClassList("inactiveTimeline");
                        m_controlPanelTitleLayout.AddToClassList("activeTimeline");
                    }

                    CurrentControlPanel = targetSection.ControlPanelOverride;
                    Debug.Log(targetSection.ControlPanelOverride.ControlPanelTitle);
                    Debug.Log("Control Panel Override");
                }
                else if (PreviousPresentationSection.ControlPanelOverride.menuBindings.Count > 0)
                {
                    m_controlPanelTitleLayout.RemoveFromClassList("activeTimeline");
                    m_controlPanelTitleLayout.AddToClassList("inactiveTimeline");
                }

                if (targetSection.ControlPanelOverride.menuBindings.Count == 0 && projectManager.MasterControlPanel.menuBindings.Count > 0)
                {
                    CurrentControlPanel = projectManager.MasterControlPanel;
                    m_controlPanelTitleLayout.RemoveFromClassList("inactiveTimeline");
                    m_controlPanelTitleLayout.AddToClassList("activeTimeline");
                }

                if (PreviousPresentationSection.VisualElementsToShow.Count > 0)
                {
                    foreach(string binding in PreviousPresentationSection.VisualElementsToShow)
                    {
                        VisualElement element = root.Q<VisualElement>(binding);

                        element.style.display = DisplayStyle.None;
                    }
                }
                if (PreviousPresentationSection.SectionMarkers.Count > 0)
                {
                    foreach (MarkerScript marker in PreviousPresentationSection.SectionMarkers)
                    {
                        Debug.Log("Markers Hidden");
                        marker.Marker.style.display = DisplayStyle.None;
                    }
                }
                if (targetSection.VisualElementsToShow.Count > 0)
                {
                    foreach (string binding in targetSection.VisualElementsToShow)
                    {
                        VisualElement element = root.Q<VisualElement>(binding);

                        element.style.display = DisplayStyle.Flex;
                    }
                }
                if (targetSection.SectionMarkers.Count > 0)
                {
                    foreach (MarkerScript marker in targetSection.SectionMarkers)
                    {
                        marker.Marker.style.display = DisplayStyle.Flex;
                    }
                }

                if (targetSection.CustomMenu != null)
                {
                    targetSection.CustomMenu.MenuElement.style.display = DisplayStyle.Flex;
                    targetSection.CustomMenu.MenuElement.style.opacity = 100;
                }

                StartCoroutine(SectionChangeRoutine());

                PlayButton.value = true;
            }
        }

        public IEnumerator SectionChangeRoutine()
        {
            CheckForTransitionEnd = true;

            if (PreviousPresentationSection.CustomMenu != null)
            {
                PreviousPresentationSection.CustomMenu.MenuElement.style.opacity = 0;
                PreviousPresentationSection.CustomMenu.MenuElement.style.display = DisplayStyle.None;
            }


            cam.SetCinemachineCamera();
            PreviousPresentationSection.director.Stop();

            if (projectManager.ActiveSection.TimelineOverride != null)
            {
                cam.TransitionCamera.transform.position = cam.MainCameraTransform.transform.position;
                cam.TransitionCamera.transform.eulerAngles = cam.MainCameraTransform.transform.eulerAngles;
                cam.MainCameraTransform.localPosition = Vector3.zero;
                cam.MainCameraTransform.localEulerAngles = Vector3.zero;

                projectManager.ActiveSection.director.Play();

            }
            
            InputManager.CameraRotated = false;

            ResumeTimeline();
            InputManager.isTransitioning = true;
            //manager.ActiveSection.director.Play();

            yield return new WaitForSeconds(0.1f);

            Transition.FadeFromXray();
            Transition.FadeFromAlpha();
            Transition.FadeFromCanvasGroup();
        }
        public IEnumerator ToggleWorldLabels(float start, float end, float duration)
        {
            float elapsedTime = 0f;
            while(elapsedTime < duration)
            {
                projectManager.ActiveSection.WorldCallouts.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            projectManager.ActiveSection.WorldCallouts.alpha = end;
        }

        public IEnumerator ResetWorldLabels()
        {
            float elapsedTime = 0f;
            float duration = 0.3f;
            while (elapsedTime < duration)
            {
                PreviousPresentationSection.WorldCallouts.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            PreviousPresentationSection.WorldCallouts.alpha = 0;
        }

        public void HideWorldSpaceLabels()
        {
            if(projectManager.ActiveSection.WorldCallouts != null)
            {
                StartCoroutine(ToggleWorldLabels(1, 0, 0.3f));
            }
        }
        public void ShowWorldSpaceLabels()
        {
            if(projectManager.ActiveSection.WorldCallouts != null)
            {
                StartCoroutine(ToggleWorldLabels(0, 1, 0.3f));
            }
        }

        public Vector2 TitleDimensions;
        public void ChangeTitle()
        {
            SectionTitle.text = projectManager.ActiveSection.SectionTitle;
            TitleDimensions = SectionTitle.MeasureTextSize(SectionTitle.text, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined); ;
            SectionTitle.style.minWidth = TitleDimensions.x + 40;
        }

        #endregion

        #region Initializer Functions
        /// <summary>
        /// sets up a popup window on the Presentation Section's popup content
        /// </summary>
        /// <param name="section"></param>
        public void InitializePopup(PresentationSection section)
        {
            if (section.GeneratedPopup != null)
            {
                section.InitPopup();
                popupManager.presentationSection = section;
                popupManager.currentContentIndex = 0;

                section.GeneratedPopup.RemoveFromClassList("unselectedPopupContent");
                section.GeneratedPopup.AddToClassList("selectedPopupContent");
                section.generatedContents[0].Container.RemoveFromClassList("unselectedPopupContent");
                section.generatedContents[0].Container.AddToClassList("selectedPopupContent");
                section.generatedContents[0].ChildContent[0].content.RemoveFromClassList("unselectedPopupContent");
                section.generatedContents[0].ChildContent[0].content.AddToClassList("selectedPopupContent");
            }
        }

        public int ThemeSetting;
        public int screenSetting;
        public int graphicSetting;
        public float mouseSensitivitySetting;
        public float penSizeSetting;
        public string saveSettingsSetting;
        public string popupWarningSetting;

        private bool canLoadDefaultSettings;
        private bool canLoadPlayerPrefs;
        public void InitializePlayerPrefs()
        {
            //ThemePicker.choices = ThemeNames;
            //screenSelection.choices = ScreenSizes;
            //GraphicsPicker.choices = GraphicsSettings;


            if (PlayerPrefs.HasKey("RememberSettings"))
            {
                canLoadPlayerPrefs = true;
                if (PlayerPrefs.GetString("RememberSettings") == "True")
                {
                    m_rememberMySettingsToggle.value = true;
                    SetRememberSettings(true);
                    ChangeScreenSize(PlayerPrefs.GetInt("ScreenSize"));
                    PickTheme(PlayerPrefs.GetInt("Theme"));
                    ChangeGraphicsSettings(PlayerPrefs.GetInt("Graphics"));
                    ChangePenStroke(PlayerPrefs.GetFloat("PenStroke"));
                    ChangeMouseSensitivity(PlayerPrefs.GetFloat("MouseSensitivity"));
                    SetPopupSettings();
                }
                else
                {
                    SetRememberSettings(false);
                    m_rememberMySettingsToggle.value = false;
                    LoadDefaultPlayerPrefs();
                }
                canLoadPlayerPrefs = false;
            }
            else
            {
                SetRememberSettings(false);
                m_rememberMySettingsToggle.value = false;
                LoadDefaultPlayerPrefs();
            }
        }
        public void LoadDefaultPlayerPrefs()
        {
            canLoadDefaultSettings = true;
            if (projectManager != null)
            {
                if (projectManager.DefaultScreenSetting == "Small")
                {
                    ChangeScreenSize(0);
                }
                if (projectManager.DefaultScreenSetting == "Medium")
                {
                    ChangeScreenSize(1);
                }
                if (projectManager.DefaultScreenSetting == "Large")
                {
                    ChangeScreenSize(2);
                }
                if (projectManager.DefaultScreenSetting == "ExtraLarge")
                {
                    ChangeScreenSize(3);
                }

                if (projectManager.DefaultThemeSetting == "Dark")
                {
                    PickTheme(0);
                }
                if (projectManager.DefaultThemeSetting == "Light")
                {
                    PickTheme(1);
                }

                if (projectManager.DefaultGraphicsSetting == "Fastest")
                {
                    ChangeGraphicsSettings(0);
                }
                if (projectManager.DefaultGraphicsSetting == "Fast")
                {
                    ChangeGraphicsSettings(1);
                }
                if (projectManager.DefaultGraphicsSetting == "Medium")
                {
                    ChangeGraphicsSettings(2);
                }
                if (projectManager.DefaultGraphicsSetting == "High")
                {
                    ChangeGraphicsSettings(3);
                }

                ChangePenStroke(projectManager.DefaultPenStrokeSetting);
                ChangeMouseSensitivity(projectManager.DefaultMouseSensitivitySetting);
                SetPopupSettings();
            }
            canLoadDefaultSettings = false;
        }
        #endregion

        #region ControlPanel functions
        void ToggleMarkers(bool value)
        {
            if (value)
            {
                m_markerLayout.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_markerLayout.style.display = DisplayStyle.None;
            }
        }

        void ToggleRendererFeature(bool value)
        {
            if (value)
            {
                DitherHighlight.SetFloat("_AlphaThreshold", 0);
            }
            if (!value)
            {
                DitherHighlight.SetFloat("_AlphaThreshold", 2);
            }
        }

        void ToggleWSlabels(bool value)
        {
            if (value)
            {
                foreach (TextMeshPro label in projectManager.ActiveSection.WorldspaceLabels)
                {
                    label.gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (TextMeshPro label in projectManager.ActiveSection.WorldspaceLabels)
                {
                    label.gameObject.SetActive(false);
                }
            }
        }

        public static void GameObjectSwitch(GameObject go, bool value)
        {
            if (value)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }

        public void SectionSwitch(PresentationSection section)
        {
            SetPresentationSection(section);
        }

        #endregion
    }
}

