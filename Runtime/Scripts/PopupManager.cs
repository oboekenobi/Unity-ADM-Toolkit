using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace ADM.UISystem
{
    public class PopupManager : VisualElement
    {
        private VisualElement _popupContent;

        private PresentationSection _presentationSection;
        //regenrate the content when the content is set
        public PresentationSection presentationSection
        {
            get => _presentationSection;
            set
            {
                //executed after the first PresentationSection is passed through
                if(_presentationSection != null)
                {
                    ResetContent();
                    if (_presentationSection == value)
                        return;
                    //disable the curret parent container
                    _popupContent.RemoveFromClassList(selectedContentClassName);
                    _popupContent.AddToClassList(unselectedContentClassName);

                    _popupContent = _presentationSection.GeneratedPopup;

                    //enable the curret parent container
                    _popupContent.RemoveFromClassList(unselectedContentClassName);
                    _popupContent.AddToClassList(selectedContentClassName);
                    m_scrollView.Remove(_popupContent);
                }

                //executed the first time the value is set;



                _presentationSection = value;
                _popupContent = _presentationSection.GeneratedPopup;
                _popupContent.RemoveFromClassList(unselectedContentClassName);
                _popupContent.AddToClassList(selectedContentClassName);
                m_scrollView = this.Q<VisualElement>("ContentView");
                
                m_scrollView.Add(_popupContent);
            }
        }
        UI_Manager uI_Manager;
        private const string unselectedContentClassName = "unselectedPopupContent";
        private const string selectedContentClassName = "selectedPopupContent";
        private const string selectedPopupClassName = "selectedPopup";
        private const string unselectedPopupClassName = "unselectedPopup";
        public static List<GeneratedContent> generatedContents = new List<GeneratedContent>();
        public static List<string> ExhibitNames = new List<string>();
        public static bool MouseInWindow;

        //the current index of the parent container cainintg all of the child containers that contain content(images)
        public int currentSectionIndex = 0;

        //the current index of the child container that contains the images
        public int currentContentIndex;
        Button m_nextButton;
        Button m_backButton;
        Label m_counter;
        VisualElement m_scrollView;
        VisualElement m_layout;
        DropdownField m_selector;
        Toggle m_fullScreenToggle;
        public Toggle m_popoutToggle;
        [Serializable]
        public class GeneratedContent
        {
            public VisualElement Container = new VisualElement();
            [SerializeField]
            public List<ContentSection> ChildContent = new List<ContentSection>();
            public string Title;
        }
        [Serializable]
        public struct ContentSection
        {
            [SerializeField]
            public VisualElement content;
            public string Title;
        }

        public new class UxmlFactory : UxmlFactory<PopupManager, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        public PopupManager()
        {
            uI_Manager = UI_Manager.FindFirstObjectByType<UI_Manager>();
            this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        public void OnGeometryChange(GeometryChangedEvent evt)
        {

            RegisterVisualElements();
        }

        void RegisterVisualElements()
        {
            m_nextButton = this.Q<Button>("NextButton");
            m_backButton = this.Q<Button>("BackButton");
            m_counter = this.Q<Label>("Counter");
            m_scrollView = this.Q<VisualElement>("ContentView");
            m_selector = this.Q<DropdownField>("Selector");
            m_fullScreenToggle = this.Q<Toggle>("PopupFullScreenToggle");
            m_popoutToggle = this.Q<Toggle>("PopoutToggle");
            m_layout = this.Q<VisualElement>("PopupLayout");
            m_selector.choices = ExhibitNames;
            if (_popupContent == null)
                return;
            m_nextButton?.RegisterCallback<ClickEvent>(evt => NextConent(currentSectionIndex));
            m_backButton?.RegisterCallback<ClickEvent>(evt => PreviousContent(currentSectionIndex));
            m_fullScreenToggle?.RegisterCallback<ClickEvent>(evt => PopupFullScreenToggle());
            m_popoutToggle?.RegisterCallback<ClickEvent>(evt => MoveToPopoutWindow());
            m_selector?.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                SetSection(evt.newValue);
            });
        }

        void MoveToPopoutWindow()
        {
            if (m_popoutToggle.value)
            {
                uI_Manager.popupManager.pickingMode = PickingMode.Ignore;
                uI_Manager.popupManager.style.minWidth = StyleKeyword.Null;
                uI_Manager.popupManager.style.minHeight = StyleKeyword.Null;
                uI_Manager.popupManager.style.maxHeight = StyleKeyword.Null;
                uI_Manager.popupManager.style.maxWidth = StyleKeyword.Null;

                uI_Manager.m_popoutContainer.Add(uI_Manager.m_popupWindowContainer);
                this.RemoveFromClassList("minimizedPopup");
                this.AddToClassList("maximizedPopup");
                uI_Manager.m_popoutLayout.AddToClassList("activePopoutWindow");
                uI_Manager.m_popoutLayout.RemoveFromClassList("inactivePopoutWindow");

                lastWindowPosition = this.transform.position;
                uI_Manager.popupManager.transform.position = Vector3.zero;
                //ZeroOutPopup();

                uI_Manager.popupManager.RemoveManipulator(uI_Manager.popupManipulator);
            }
            else
            {
                uI_Manager.popupManager.pickingMode = PickingMode.Position;
                uI_Manager.popupManager.style.minWidth = uI_Manager.popupManipulator.lastElementSize.x;
                uI_Manager.popupManager.style.minHeight = uI_Manager.popupManipulator.lastElementSize.y;
                uI_Manager.popupManager.transform.position = uI_Manager.popupManipulator.lastPosition;
                uI_Manager.popupManager.style.maxHeight = uI_Manager.popupManipulator.lastElementSize.x;
                uI_Manager.popupManager.style.maxWidth = uI_Manager.popupManipulator.lastElementSize.y;
                
                uI_Manager.m_popoutLayout.style.minWidth = StyleKeyword.Null;
                uI_Manager.m_popupLayout.Add(uI_Manager.m_popupWindowContainer);
                uI_Manager.m_popoutLayout.AddToClassList("inactivePopoutWindow");
                uI_Manager.m_popoutLayout.RemoveFromClassList("activePopoutWindow");
                this.AddToClassList("minimizedPopup");
                this.RemoveFromClassList("maximizedPopup");
                uI_Manager.popupManager.AddManipulator(uI_Manager.popupManipulator);

                uI_Manager.popupManager.transform.position = lastWindowPosition;
            }
        }
        Vector3 lastWindowPosition;
        void PopupFullScreenToggle()
        {
            Debug.Log("fullScreen Clicked");
            if (m_fullScreenToggle.value)
            {
                
                this.transform.position = Vector3.zero;

                m_layout.transform.position = Vector3.zero;

                if (uI_Manager.projectManager.ActiveSection.SectionMarkers.Count > 0)
                {
                    foreach (MarkerScript marker in uI_Manager.projectManager.ActiveSection.SectionMarkers)
                    {
                        marker.Marker.style.display = DisplayStyle.None;
                    }
                }
                uI_Manager.m_scrubBarLayout.SendToBack();
                m_popoutToggle.style.display = DisplayStyle.None;

                //if the Popout is enabled then keep it in the the current container and expand it to fullscreen
                if (m_popoutToggle.value)
                {
                    uI_Manager.m_popoutLayout.style.minWidth = StyleKeyword.Null;
                    uI_Manager.m_popoutLayout.style.maxWidth = StyleKeyword.Null;
                    uI_Manager.m_popoutLayout.RemoveFromClassList("activePopoutWindow");
                    uI_Manager.m_popoutLayout.AddToClassList("maximizedPopoutWindow");
                }
                //if the window is seperate maximize it and remove the manipulator
                else
                {
                    lastWindowPosition = this.transform.position;
                    this.RemoveManipulator(uI_Manager.popupManipulator);
                    uI_Manager.popupManager.style.minWidth = StyleKeyword.Null;
                    uI_Manager.popupManager.style.minHeight = StyleKeyword.Null;
                    uI_Manager.popupManager.style.maxHeight = StyleKeyword.Null;
                    uI_Manager.popupManager.style.maxWidth = StyleKeyword.Null;
                    this.RemoveFromClassList("minimizedPopup");
                    
                    this.AddToClassList("maximizedPopup");
                    Debug.Log("popup attempted to go fullscreen");
                }
            }
            else
            {
                if (uI_Manager.projectManager.ActiveSection.SectionMarkers.Count > 0)
                {
                    foreach (MarkerScript marker in uI_Manager.projectManager.ActiveSection.SectionMarkers)
                    {
                        marker.Marker.style.display = DisplayStyle.Flex;
                    }
                }
                uI_Manager.m_scrubBarLayout.BringToFront();
                m_popoutToggle.style.display = DisplayStyle.Flex;

                //if the Popout is enabled then return the container to the default size
                if (m_popoutToggle.value)
                {
                    this.RemoveFromClassList("minimizedPopup");
                    this.AddToClassList("maximizedPopup");
                    uI_Manager.m_popoutLayout.RemoveFromClassList("maximizedPopoutWindow");
                    uI_Manager.m_popoutLayout.AddToClassList("activePopoutWindow");
                }
                //if the window is seperate return it to the original size and add its manipulator back
                else
                {
                    this.transform.position = lastWindowPosition;
                    uI_Manager.popupManager.style.minWidth = StyleKeyword.Null;
                    uI_Manager.popupManager.style.minHeight = StyleKeyword.Null;
                    uI_Manager.popupManager.style.maxHeight = StyleKeyword.Null;
                    uI_Manager.popupManager.style.maxWidth = StyleKeyword.Null;
                    this.AddManipulator(uI_Manager.popupManipulator);
                    this.RemoveFromClassList("maximizedPopup");
                    this.AddToClassList("minimizedPopup");
                }
            }
        }
        void NextConent(int Section)
        {
            if (currentContentIndex == generatedContents[Section].ChildContent.Count-1)
            {
                
                return;
            }
            generatedContents[Section].ChildContent[currentContentIndex].content.RemoveFromClassList(selectedContentClassName);
            generatedContents[Section].ChildContent[currentContentIndex].content.AddToClassList(unselectedContentClassName);
            currentContentIndex++;
            generatedContents[Section].ChildContent[currentContentIndex].content.RemoveFromClassList(unselectedContentClassName);
            generatedContents[Section].ChildContent[currentContentIndex].content.AddToClassList(selectedContentClassName);
            m_counter.text = (currentContentIndex + 1)+"/"+ (generatedContents[Section].ChildContent.Count);
            Debug.Log(currentContentIndex);
        }
        void PreviousContent(int Section)
        {
            if (currentContentIndex == 0)
            {
                return;
            }
            generatedContents[Section].ChildContent[currentContentIndex].content.RemoveFromClassList(selectedContentClassName);
            generatedContents[Section].ChildContent[currentContentIndex].content.AddToClassList(unselectedContentClassName);
            currentContentIndex--;
            generatedContents[Section].ChildContent[currentContentIndex].content.RemoveFromClassList(unselectedContentClassName);
            generatedContents[Section].ChildContent[currentContentIndex].content.AddToClassList(selectedContentClassName);
            m_counter.text = (currentContentIndex + 1) + "/" + (generatedContents[Section].ChildContent.Count);
        }
        void SetSection(string SectionName)
        {
            int newIndex = new int();
            generatedContents[currentSectionIndex].Container.RemoveFromClassList(selectedContentClassName);
            generatedContents[currentSectionIndex].Container.AddToClassList(unselectedContentClassName);

            for (int i = 0; i < generatedContents.Count; i++)
            {

                if (generatedContents[i].Title == SectionName)
                {
                    newIndex = i;
                    Debug.Log("New Section is " + i);
                    generatedContents[i].Container.RemoveFromClassList(unselectedContentClassName);
                    generatedContents[i].Container.AddToClassList(selectedContentClassName);
                }
                
            }
            foreach(ContentSection content in generatedContents[newIndex].ChildContent)
            {
                content.content.RemoveFromClassList(selectedContentClassName);
                content.content.AddToClassList(unselectedContentClassName);
            }
            generatedContents[newIndex].ChildContent[0].content.RemoveFromClassList(unselectedContentClassName);
            generatedContents[newIndex].ChildContent[0].content.AddToClassList(selectedContentClassName);
            currentContentIndex = 0;
            currentSectionIndex = newIndex;
        }

        void ResetContent()
        {
            //Reset the Child container (Visual Elements containing images)

            foreach(GeneratedContent section in generatedContents)
            {
                section.Container.RemoveFromClassList(selectedContentClassName);
                section.Container.AddToClassList(unselectedContentClassName);
                foreach(ContentSection content in section.ChildContent)
                {
                    content.content.RemoveFromClassList(selectedContentClassName);
                    content.content.AddToClassList(unselectedContentClassName);
                }

            }

            //Reset the Child Contents (Images)
            generatedContents[0].ChildContent[0].content.RemoveFromClassList(unselectedContentClassName);
            generatedContents[0].ChildContent[0].content.AddToClassList(selectedContentClassName);
            uI_Manager.Popup.transform.position = Vector3.zero;
            currentContentIndex = 0;
            currentSectionIndex = 0;
            m_counter.text = (currentContentIndex + 1) + "/" + (generatedContents[0].ChildContent.Count);
        }

        private Vector3 flippedDirection;
        public void ZeroOutPopup()
        {
            /*if (parent.resolvedStyle.flexDirection == FlexDirection.ColumnReverse)
            {

                //'Vector3 flippedDirection = new Vector3();

                if(transform.position != flippedDirection)
                {
                    flippedDirection = new Vector3(parent.worldBound.width - worldBound.width, worldBound.height - parent.worldBound.height, 0);
                    transform.position = flippedDirection;
                }
                
                Debug.Log(new Vector2(parent.worldBound.width, transform.position.x));
                Debug.Log("Column reverse Zeroed");
            }
            if (parent.resolvedStyle.flexDirection == FlexDirection.RowReverse)
            {
                transform.position = Vector3.zero;
                Debug.Log("Row Reverse Zeroed");
            }
            if (parent.resolvedStyle.flexDirection == FlexDirection.Column || parent.resolvedStyle.flexDirection == FlexDirection.Row)
            {

                if (transform.position != flippedDirection)
                {
                    flippedDirection = new Vector3((parent.worldBound.width - worldBound.width), 0, 0);
                    transform.position = flippedDirection;
                }
                Debug.Log("Row and Column zeroed");
            }*/

            parent.style.flexDirection = FlexDirection.RowReverse;
            transform.position = Vector3.zero;

            Debug.Log("Zeroed popup");
        }



        void ClearContent()
        {
           /* ExhibitNames.Clear();
            m_scrollView.Clear();
            Content.Clear();
            currentContentIndex = 0;*/
        }

        public void OpenPopup()
        {
            RegisterVisualElements();
            this.RemoveFromClassList(unselectedPopupClassName);
            this.AddToClassList(selectedPopupClassName);
            m_counter = this.Q<Label>("Counter");
            m_counter.text = (1) + "/" + (generatedContents[0].ChildContent.Count);
        }
        public void ClosePopup()
        {
            this.AddToClassList(unselectedPopupClassName);
            this.RemoveFromClassList(selectedPopupClassName);
        }
    }
}