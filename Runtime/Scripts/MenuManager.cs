using ADM.UISystem;
using MyNamespace;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : VisualElement
{
    public static bool SectionButtonRegistered;

    UI_Manager uI_Manager;
    VisualElement m_ReferenceMenu;
    public Toggle m_PlayButton;
    Toggle m_PanTool;
    Toggle m_PenTool;
    VisualElement m_InformationButton;
    Toggle m_ReferenceMenuToggle;
    public Toggle m_LabelToggle;
    VisualElement m_helpWindowCloseButton;
    VisualElement m_SkipBack;
    VisualElement m_SkipForward;
    VisualElement m_ReturnMain;
    public VisualElement m_laserPointerTexture;
    public VisualElement m_helpWindow;
    public VisualElement m_windowBackground;
    public VisualElement m_popoutLayout;
    public VisualElement m_popoutHandle;
    public Label m_popupWarning;
    VisualElement m_laserPointerToolTip;
    VisualElement m_helpContactButton;

    Slider m_SliderHandle;
    Slider m_mouseSensitivitySlider;

    VisualElement m_GameWindow;
    VisualElement m_sliderTracker;
    VisualElement m_screenCaptureButton;
    public Toggle m_FullScreenButton;
    public Label ExhibitsTab;
    public Label SettingsTab;
    public Label ToolsTab;

    public static bool MouseInGameWindow;
    public static bool PopoutIsSliding;



    private ListView listView;
    public TabbedMenuController controller;

    public new class UxmlFactory : UxmlFactory<MenuManager, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }

    public MenuManager()
    {
        uI_Manager = UI_Manager.FindFirstObjectByType<UI_Manager>();
        //uI_Manager.menuManager = this;
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void OnGeometryChange(GeometryChangedEvent evt)
    {
        m_FullScreenButton = this.Q<Toggle>("FullScreenToggle");
        m_ReferenceMenu = this.Q("PopOutMenu");
        m_ReferenceMenuToggle = this.Q<Toggle>("MenuToggle");
        m_LabelToggle = this.Q<Toggle>("LabelToggle");
        m_PlayButton = this.Q<Toggle>("PlayTimeline");
        m_SliderHandle = this.Q<Slider>("TimelineSlider");
        m_SkipBack = this.Q("SkipBackwards");
        m_SkipForward = this.Q("SkipForwards");
        m_GameWindow = this.Q("GameWindow");
        m_PenTool = this.Q<Toggle>("PenToggle");
        m_PanTool = this.Q<Toggle>("PanToggle");
        m_helpWindow = this.Q<VisualElement>("HelpWindow");
        m_windowBackground = this.Q<VisualElement>("WindowBackground");
        m_InformationButton = this.Q<Button>("Info");
        m_helpWindowCloseButton = this.Q<VisualElement>("HelpWindowCloseButton");
        m_helpContactButton = this.Q<VisualElement>("ContactButton");
        m_laserPointerTexture = this.Q<VisualElement>("LaserPointerTexture");
        m_popupWarning = this.Q<Label>("PopupWarning");


        
        m_popoutLayout = this.Q<VisualElement>("PopoutLayout");
        m_popoutHandle = this.Q<VisualElement>("PopoutHandle");

        m_screenCaptureButton = this.Q<Slider>("ScreenCaptureButton");
        

        m_helpContactButton?.RegisterCallback<ClickEvent>(ev => SendEmail());
        m_helpWindowCloseButton?.RegisterCallback<ClickEvent>(ev => CloseDocumentation());
        m_InformationButton?.RegisterCallback<ClickEvent>(ev => OpenDocumentation());
       
        ExhibitsTab = this.Q<Label>("SectionsTab");
        SettingsTab = this.Q<Label>("SettingsTab");
        ToolsTab = this.Q<Label>("ToolsTab");

        m_laserPointerToolTip = this.Q<VisualElement>("ToolTipLaser");


        m_screenCaptureButton?.RegisterCallback<ClickEvent>(ev => CaptureScreen());

        m_PenTool?.RegisterCallback<ClickEvent>(ev => EnablePen());

        m_PanTool?.RegisterCallback<ClickEvent>(ev => EnablePan());

        m_SkipForward?.RegisterCallback<ClickEvent>(ev => NextSection());

        m_SkipBack?.RegisterCallback<ClickEvent>(ev => PreviousSection());

        m_LabelToggle?.RegisterCallback<ClickEvent>(ev => ToggleLabels());

        m_ReferenceMenuToggle?.RegisterCallback<ClickEvent>(ev => EnableHome());

        m_PlayButton?.RegisterCallback<ClickEvent>(ev => ControlTimeline());

        m_SliderHandle?.RegisterValueChangedCallback(ev => RegisterTimelineScrub());


        m_SliderHandle?.RegisterCallback<MouseDownEvent>(ev => ReleaseTimeline());

        m_GameWindow?.RegisterCallback<MouseDownEvent>(evt => ReleaseMouse());

        m_GameWindow?.RegisterCallback<MouseEnterEvent>(evt => MouseInGameWindow = true);
        m_GameWindow?.RegisterCallback<MouseLeaveEvent>(evt => MouseInGameWindow = false);


        if(m_laserPointerToolTip != null)
        {
            //m_penStrokeSlider?.RegisterValueChangedCallback(ev => RegisterLaserToolTip(m_laserPointerToolTip));
            /*m_penStrokeSlider?.RegisterCallback<PointerDownEvent>(ev => RegisterLaserToolTip(m_laserPointerToolTip, ev));
            m_penStrokeSlider?.RegisterCallback<MouseMoveEvent>(ev => UpdateLaserToolTip(m_laserPointerToolTip, ev));
            m_penStrokeSlider?.RegisterCallback<PointerUpEvent>(ev => UnRegisterToolTip(m_laserPointerToolTip, ev));*/
        }

        m_FullScreenButton?.RegisterCallback<ClickEvent>(ev => FullScreenMode());



        m_popoutHandle?.RegisterCallback<PointerDownEvent>(evt => BeginPopoutDrag());

        m_popoutHandle?.RegisterCallback<MouseEnterEvent>(evt => RevealHandle());

        m_popoutHandle?.RegisterCallback<MouseLeaveEvent>(evt => EndPopoutDrag());

        //m_popoutHandle?.RegisterCallback<PointerMoveEvent>(ResizePopout);

        RegisterTabCallbacks();
    }

    private bool ToolTipPenStrokeInitialized;
    public const float mouseOffset = 10;
    public bool isDragging = false;
    public void UpdateLaserToolTip(VisualElement tooltipContainer, MouseMoveEvent ev)
    {
        /*if (!isDragging)
        {
            return;
        }*/
        Debug.Log(ev.mousePosition);

        Vector3 tooltipPosition = new Vector3(ev.mousePosition.x, ev.mousePosition.y + mouseOffset,0);
        tooltipContainer.transform.position = tooltipPosition;
    }

    /*public void RegisterLaserToolTip(VisualElement tooltipContainer, PointerDownEvent ev)
    {
        *//*if (isDragging)
        {
            return;
        }
        Vector3 tooltipPosition = new Vector3(ev.position.x, ev.position.y + mouseOffset, ev.position.z);
        tooltipContainer.transform.position = tooltipPosition;*//*
        tooltipContainer.style.opacity = 100;
        isDragging = true;
        m_penStrokeSlider.CaptureMouse();

        tooltipContainer.CapturePointer(ev.pointerId);
    }
    public void UnRegisterToolTip(VisualElement tooltipContainer, PointerUpEvent ev)
    {

        tooltipContainer.style.opacity = 0;
        isDragging = false;
        m_penStrokeSlider.ReleaseMouse();
        tooltipContainer.ReleasePointer(ev.pointerId);
    }*/

    public void DisplayPopupWarning(string message, float delay)
    {
        if (uI_Manager.m_popMessageToggle.value)
        {
            m_popupWarning.text = message;


            m_popupWarning.style.transitionDuration = new List<TimeValue>()
            {
                new TimeValue(0, TimeUnit.Second)
            };
            m_popupWarning.style.transitionDelay = new List<TimeValue>()
            {
                new TimeValue(0, TimeUnit.Second)
            };



            m_popupWarning.style.opacity = 0.7f;


            m_popupWarning.style.transitionDelay = new List<TimeValue>()
            {
                new TimeValue(delay, TimeUnit.Second)
            };
            m_popupWarning.style.transitionDuration = new List<TimeValue>()
            {
                new TimeValue(3.5f, TimeUnit.Second)
            };

            m_popupWarning.style.opacity = 0;
        }
        
    }
    public void CaptureScreen()
    {
        string folderPath = "C:/screenshots/";
        string fileName = "filename";
    }



    public void RevealHandle()
    {
        m_popoutHandle.RemoveFromClassList("inactivePopoutHandle");
        m_popoutHandle.AddToClassList("activePopoutHandle");
    }
    public void BeginPopoutDrag()
    {
        PopoutIsSliding = true;
        m_popoutLayout.RemoveFromClassList("inactivePopoutHandle");
        m_popoutLayout.AddToClassList("activePopoutHandle");
        uI_Manager.m_popoutLayout.style.transitionDuration = new List<TimeValue>()
        {
            new TimeValue(0, TimeUnit.Second)
        };
    }

    public void EndPopoutDrag()
    {
        if (!PopoutIsSliding)
        {
            m_popoutHandle.RemoveFromClassList("activePopoutHandle");
            m_popoutHandle.AddToClassList("inactivePopoutHandle");
        }
    }
    public void NextSection()
    {
        uI_Manager.NextSection();
        m_PlayButton.value = true;
        SectionButtonRegistered = true;
    }
    public void PreviousSection()
    {
        uI_Manager.PreviousSection();
        m_PlayButton.value = true;
        SectionButtonRegistered = true;
    }
    public void EnableHome()
    {
        if(m_ReferenceMenuToggle.value)
        {
            m_ReferenceMenu.style.minWidth = 350;
            InputManager.AdjustCameraFraming(350, 0);
            InitTab(ExhibitsTab);
        }
        else
        {
            GetAllTabs().ForEach(UnselectTab);
            m_ReferenceMenu.style.minWidth = 0;
            InputManager.AdjustCameraFraming(0, 0);
        }
    }

    public void ToggleLabels()
    {
        if (m_LabelToggle.value)
        {
            if (Application.isPlaying)
            {
                VisualElement Container = uI_Manager.projectManager.ActiveSection.CalloutCanvasDocument.rootVisualElement.Q<VisualElement>("RootCalloutCanvas");
                Container.RemoveFromClassList("activeCanvas");
                Container.AddToClassList("inactiveCanvas");
            }
            DisplayPopupWarning("Callouts disabled", 2);
        }
        else
        {
            VisualElement Container = uI_Manager.projectManager.ActiveSection.CalloutCanvasDocument.rootVisualElement.Q<VisualElement>("RootCalloutCanvas");
            Container.RemoveFromClassList("inactiveCanvas");
            Container.AddToClassList("activeCanvas");
            DisplayPopupWarning("Callouts enabled", 2);
        }
    }

    public void ControlTimeline()
    {
        if (m_PlayButton.value)
        {
            uI_Manager.ResumeTimeline();
            uI_Manager.CheckForEnd = true;
        }
        else
        {
            uI_Manager.PauseTimeline();
            uI_Manager.CheckForEnd = false;
        }
    }

    public void ReleaseTimeline()
    {
        uI_Manager.PauseTimeline();
    }
    public void RegisterTimelineScrub()
    {
        m_sliderTracker = m_SliderHandle.Q<VisualElement>("unity-tracker");
        m_sliderTracker.style.minWidth = new Length(100 * m_SliderHandle.value, LengthUnit.Percent);
        uI_Manager.UIScrubTimeline();
    }

   

    public void LockMouse()
    {
        UI_Manager.RestrictMovement = true;
    }

    public void ReleaseMouse()
    {
        if(uI_Manager.projectManager.ActiveSection.hideTimeline)
        {
            return;
        }

        InputManager.CameraRotating = true;
        if (m_PlayButton.value)
        {
            uI_Manager.PauseTimeline();
            uI_Manager.CheckForEnd = false;
        }
    }

    int PreviousHeight;
    int PreviousWidth;
    public void FullScreenMode()
    {
        if (m_FullScreenButton.value)
        {
            DisplayPopupWarning("Fullscreen enabled. Press Esc to exit", 3.5f);
            UI_Manager.PreviousScreenHeight = Screen.height;
            UI_Manager.PreviousScreenWidth = Screen.width;
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            Screen.fullScreen = true;
            UI_Manager.isFullScreen = true;
        }
        else
        {
            DisplayPopupWarning("Fullscreen disabled", 2f);
            Screen.SetResolution(UI_Manager.PreviousScreenWidth, UI_Manager.PreviousScreenHeight, false);
            Screen.fullScreen = false;
            UI_Manager.isFullScreen = false;
        }
    }

    public void EnablePan()
    {
        if (m_PenTool.value)
        {
            m_laserPointerTexture.pickingMode = PickingMode.Ignore;
            hasBeenClicked = false;
            m_PenTool.value = false;
            uI_Manager.drawManager.Clear();
            UI_Manager.DrawingMode = false;
            DisplayPopupWarning("Pen tool disabled", 3);
        }
        if (!m_PanTool.value)
        {
            m_laserPointerTexture.pickingMode = PickingMode.Ignore;
            UI_Manager.RestrictMovement = true;
        }
        else
        {
            UI_Manager.RestrictMovement = false;
        }
    }

    public bool hasBeenClicked;
    public void EnablePen()
    {
        UI_Manager.RestrictMovement = true;
        uI_Manager.drawManager.InitCanvas();
        if (hasBeenClicked)
        {
            uI_Manager.drawManager.Clear();
            hasBeenClicked = false;
        }
        if (m_PanTool.value)
        {
            m_PanTool.value = false;
            UI_Manager.DrawingMode = true;
            m_laserPointerTexture.pickingMode = PickingMode.Position;
            hasBeenClicked = true;
            DisplayPopupWarning("Pen tool enabled, Camera movements disabled.", 4);
        }
        else if (m_PenTool.value)
        {
            m_laserPointerTexture.pickingMode = PickingMode.Position;
            UI_Manager.DrawingMode = true;
            hasBeenClicked = true;
        }
        if(!m_PenTool.value)
        {
            m_laserPointerTexture.pickingMode = PickingMode.Ignore;
            UI_Manager.DrawingMode = false;
            uI_Manager.drawManager.Clear();
        }
    }

    public void OpenDocumentation()
    {
        m_windowBackground.RemoveFromClassList("inactiveWindow");
        m_windowBackground.AddToClassList("activeBackground");
        m_helpWindow.RemoveFromClassList("inactiveWindow");
        m_helpWindow.AddToClassList("activeWindow");
    }
    public void CloseDocumentation()
    {
        m_windowBackground.AddToClassList("inactiveWindow");
        m_windowBackground.RemoveFromClassList("activeBackground");
        m_helpWindow.AddToClassList("inactiveWindow");
        m_helpWindow.RemoveFromClassList("activeWindow");
    }

    void SendEmail()
    {
        /*        string email = "susi@advocacydm.com";
                string subject = "";
                string body = "";
                Application.OpenURL("mailto:" + email + "?subject=" + "" + "&body=" + "");
        */
        Application.OpenURL("https://advocacydm.com/contact/");
    }

    public ThemeStyleSheet Theme;

    [Tooltip("The root classes to apply the color to")]
    public VisualElement[] rootClasses;

    [Tooltip("The class style pointer to apply the color to")]
    private string PrimaryBase = ".PrimaryBase-olor";
    private string SecondaryBase = ".SecondaryBase-color";
    public string TertiaryBase = "TertiaryBase-color";
    public string Highlight = "Highlight-color";
    public string Text1 = "Text1-color";
    public string Text2 = "Text2-color";
    public string Border = ".borderColor";

   /* public ColorData currentTheme
    {
        get => _currentTheme;

        set
        {
            _currentTheme = value;
            ApplyThemeColors(_currentTheme);
        }

    }*/
    public void ApplyThemeColors(ColorData data)
    {
        this.Query(className: PrimaryBase).ForEach((element) =>
        {
            element.style.backgroundColor = data.PrimaryBaseColor;
        });
        this.Query(className: SecondaryBase).ForEach((element) =>
        {
            element.style.backgroundColor = data.SecondaryBaseColor;
        });
        this.Query(className: TertiaryBase).ForEach((element) =>
        {
            element.style.backgroundColor = data.TertiaryBaseColor;
        });

        this.Query(className: Highlight).ForEach((element) =>
        {
            element.style.backgroundColor = data.HighlightColor;
        });
        this.Query(className:Text1).ForEach((element) =>
        {
            element.style.backgroundColor = data.Text1Color;
        });
        this.Query(className: Text2).ForEach((element) =>
        {
            element.style.backgroundColor = data.Text2Color;
        });
        this.Query(className: Border).ForEach((element) =>
        {
            element.style.backgroundColor = data.BorderColor;
        });
    }

    #region TabbedMenu


    private const string tabClassName = "tab";
    private const string currentlySelectedTabClassName = "currentlySelectedTab";
    private const string unselectedContentClassName = "unselectedContent";
    private const string selectedContentClassName = "selectedContent";
    // Tab and tab content have the same prefix but different suffix
    // Define the suffix of the tab name
    private const string tabNameSuffix = "Tab";
    // Define the suffix of the tab content name
    private const string contentNameSuffix = "Content";
    private bool Init;
    private VisualElement InitiatedContent;

    public void RegisterTabCallbacks()
    {
        UQueryBuilder<Label> tabs = GetAllTabs();
        tabs.ForEach((Label tab) => {
            tab.RegisterCallback<ClickEvent>(TabOnClick);
        });
    }

    /* Method for the tab on-click event: 

       - If it is not selected, find other tabs that are selected, unselect them 
       - Then select the tab that was clicked on
    */
    private void TabOnClick(ClickEvent evt)
    {
        Label clickedTab = evt.currentTarget as Label;
        if (Init)
        {
            Debug.Log("Init is false");
            InitiatedContent.style.transitionDelay = new List<TimeValue>() { new TimeValue(0f, TimeUnit.Second) };
            Init = false;
        }
        if (!TabIsCurrentlySelected(clickedTab))
        {
            GetAllTabs().Where(
                (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
            ).ForEach(UnselectTab);
            SelectTab(clickedTab);
        }
    }
    public void InitTab(Label tab)
    {
        
        Label clickedTab = tab;
        InitiatedContent = FindContent(tab);
        InitiatedContent.style.transitionDelay = new List<TimeValue>() { new TimeValue(0.15f, TimeUnit.Second) };
        Init = true;
        SelectTab(clickedTab);
        
    }
    //Method that returns a Boolean indicating whether a tab is currently selected
    private static bool TabIsCurrentlySelected(Label tab)
    {
        return tab.ClassListContains(currentlySelectedTabClassName);
    }

    private UQueryBuilder<Label> GetAllTabs()
    {
        return this.Query<Label>(className: tabClassName);
    }

    /* Method for the selected tab: 
       -  Takes a tab as a parameter and adds the currentlySelectedTab class
       -  Then finds the tab content and removes the unselectedContent class */
    private void SelectTab(Label tab)
    {
        tab.AddToClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.RemoveFromClassList(unselectedContentClassName);
        content.AddToClassList(selectedContentClassName);
        tab.style.minWidth = new Length(25, LengthUnit.Percent);
    }

    /* Method for the unselected tab: 
       -  Takes a tab as a parameter and removes the currentlySelectedTab class
       -  Then finds the tab content and adds the unselectedContent class */
    private void UnselectTab(Label tab)
    {
        tab.RemoveFromClassList(currentlySelectedTabClassName);
        VisualElement content = FindContent(tab);
        content.AddToClassList(unselectedContentClassName);
        content.RemoveFromClassList(selectedContentClassName);
        tab.style.minWidth = new Length(0, LengthUnit.Percent);
    }

    // Method to generate the associated tab content name by for the given tab name
    private static string GenerateContentName(Label tab) =>
        tab.name.Replace(tabNameSuffix, contentNameSuffix);

    // Method that takes a tab as a parameter and returns the associated content element
    private VisualElement FindContent(Label tab)
    {
        return this.Q(GenerateContentName(tab));
    }

    #endregion
}
