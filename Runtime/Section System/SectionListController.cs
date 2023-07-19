using ADM.UISystem;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SectionListController : MonoBehaviour
{
    // UXML template for list entries

    public VisualTreeAsset ListEntryTemplate;
    UI_Manager uIManager;
    // UI element references
    GroupBox SectionList;
    GroupBox ReferenceList;
    Label CharClassLabel;
    Label CharNameLabel;
    VisualElement CharPortrait;
    VisualTreeAsset Menu;

    public void InitializeCharacterList(VisualElement root,VisualTreeAsset listEntryTemplate, List<PresentationSection> sections, UI_Manager manager)
    {
        Sections = sections;
        uIManager = manager;

        ListEntryTemplate = listEntryTemplate;

        // Store a reference to the character list element
        SectionList = root.Q<GroupBox>("Sections");

        // Store references to the selected character info elements
        FillCharacterList();
    }

    List<PresentationSection> Sections;
    public static List<RadioButton> SectionButtons;
    public static List<RadioButton> ReferenceButtons = new List<RadioButton>();

    void FillCharacterList()
    {
        //Debug.Log(Sections.Count);
        for(int i = 0; i < Sections.Count; i++)
        {
            // Instantiate the UXML template for the entry
            var newListEntry = ListEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new SectionListEntryController();
            newListEntryLogic.uI_Manager = uIManager;
            
            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry, Sections[i]);

            // Add UXML template to the GroupBox
            SectionList.Add(newListEntry);
        }

    }

    void SkipToSection(int index, RadioButton button)
    {
        uIManager.SetPresentationSection(uIManager.projectManager.Sections[index]);
        ReferenceButtons[index].value = true;
        Debug.Log(index);
    }
}
