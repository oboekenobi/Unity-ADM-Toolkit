using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CalloutCanvas : VisualElement
{

    public PresentationSection section { get; set; }
    

    public new class UxmlFactory : UxmlFactory<CalloutCanvas, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits {
        /*public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }


        UxmlIntAttributeDescription sectionAttr =
            new UxmlIntAttributeDescription { name = "Presentation Section", type = "PresentationSection" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {

            base.Init(ve, bag, cc);
            var canvas = ve as CalloutCanvas;

            canvas.section = sectionAttr.GetValueFromBag(bag, cc);

        }*/
    }


    public CalloutCanvas()
    {
        this.StretchToParentSize();

    }

}
