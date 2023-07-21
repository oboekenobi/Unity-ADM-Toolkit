using ADM.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SectionListEntryController
{
    public RadioButton SectionButton;
    public RadioButton ReferenceButton;
    public UI_Manager uI_Manager;
    Label ReferenceNumber;

    //This function retrieves a reference to the 
    //character name label inside the UI element.

    public void SetVisualElement(VisualElement visualElement, PresentationSection section)
    {
        SectionButton = visualElement.Q<RadioButton>("SectionButton");

        SectionButton.text = section.referenceTitle;

        uI_Manager.RegisteredSections.Add(this);

        SectionButton.RegisterCallback<ClickEvent>(ev => SkipToSection(section));

    }

    //This function receives the character whose name this list 
    //element displays. Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.


    void SkipToSection(PresentationSection section)
    {
        uI_Manager.SetPresentationSection(section);

    }
}
