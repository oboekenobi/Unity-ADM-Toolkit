/* Original code[1] Copyright (c) 2022 Shane Celis[2]
   Licensed under the MIT License[3]

   [1]: https://gist.github.com/shanecelis/b6fb3fe8ed5356be1a3aeeb9e7d2c145
   [2]: https://twitter.com/shanecelis
   [3]: https://opensource.org/licenses/MIT
*/

using UnityEngine;
using UnityEngine.UIElements;
using ADM.UISystem;

/** This manipulator makes a visual element draggable at runtime. Unity's
    UIToolkit also has a [drag-and-drop system][1] but it is only appropriate
    for use within its editor.

    ## Usage

    ```
    element.AddManipulator(new DragManipulator());
    element.RegisterCallback<DropEvent>(evt =>
      Debug.Log($"{evt.target} dropped on {evt.droppable}");
    ```

    OR

    ```
    foreach (var element in root.Query(className: "draggable").Build()) {
      element.AddManipulator(new DragManipulator());
    }
    root.RegisterCallback<DropEvent>(evt =>
      Debug.Log($"{evt.target} dropped on {evt.droppable}");
    ```

    ### Styling

    When dragging, one should be able to style the participating elements.
    Coupled with Unity Style Sheet (USS) transitions, one can provide automatic
    tweens.

    | USS Selectors        | Description                                   |
    |----------------------+-----------------------------------------------|
    | .draggable           | Present on any element with a DragManipulator |
    | .draggable--dragging | Present while dragging                        |
    | .draggable--can-drop | Present while dragging over a droppable       |
    | .droppable           | Identifies a droppable element (editable)     |
    | .droppable--can-drop | Present while a draggable is hovering         |

    A custom property also allows one to disable dragging via the style sheet.

    | USS Properties      | Description                                    |
    |---------------------+------------------------------------------------|
    | --draggable-enabled | When set to false, dragging is disabled        |

    ## Requirements

    - Unity 2020.3 or later

    ## Dragging

    Clicking and dragging on the draggable element will cause it to move. The
    USS class "draggable--dragging" will be present during
    the duration.

    ### Remove USS Class on Drag

    One can remove a USS class while dragging by setting the following
    parameter at initialization:

    ```
    var dragger = new DragManipulator { removeClassOnDrag = "transitions" };
    ```

    Usage: If one has translation USS transitions set, dragging may look wrong
    and may not be smooth. Placing transitions into a special class and removing
    that class during the drag fixed that problem.

    ## Dropping

    Elements that have a "droppable" USS class will be considered droppable.
    When dragging and hovering over a droppable element, the USS class
    "droppable--can-drop" will be added; the draggable element will have
    "draggable--can-drop" added to it.

    If the draggable element is dropped on a non-droppable element, the
    draggable element's position is reset. It is suggested that one turn on USS
    transitions if one wants the draggable to tween back into its original
    place.

    ### Distinct Droppables

    If one has distinct droppable objects, one set the `droppableId` on the
    `DragManipulator` to something other than "droppable".

    ```
    var dragger = new DragManipulator { droppableId = "discard-pile" };
    ```

    ## Handling Events

    When a draggable element is released on a droppable element or its child, a
    `DropEvent` is emitted. The position of the element is not reset
    automatically in that case. If the dropped object is supposed to return to
    its original position, one ought to do that in the callback code.

    ```
    void OnDrag(DropEvent evt) {
      evt.target.transform.position = Vector3.zero;
      // OR
      // evt.dragger.ResetPosition();
    }
    ```

    ## Limitations

    This manipulator changes the `transform.position` of the target element
    while dragging. If one's styling is making use of that, the behavior is
    undefined.

    ## Notes

    The drop event bubbles up, so the callback can be placed on the parent or
    root element.

    Acknowledgments to Crayz[2] and Stacey[3] for their inspiring code.

    [1]: https://forum.unity.com/threads/visualelement-drag-and-drop-during-runtime.930000/#post-6373881
    [2]: https://forum.unity.com/threads/creating-draggable-visualelement-and-clamping-it-to-screen.1017715/
    [3]: https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
*/
public class PopupManipulator : IManipulator {

    private VisualElement _target;
    private VisualElement Parent;
    private UIDocument uI_Document;
    
    //Seperate class to manage the cursor modes since Runtime UI Toolkit does not support default cursors
    private UI_Manager uI_Manager;

    public Vector2 lastPosition;

    public Vector2 lastElementSize;
    public Vector2 lastScreenSize;
    private Vector2 finalPosition;
    private float edgeSize;
    private LengthUnit lengthUnit;
    private float minX;
    private float minY;
    private float maxX;
    private float maxY;
    float initialTargetHeight;
    float initialTargetWidth;
    float initialXmin;
    float initialXmax;
    float initialYmin;
    float initialYmax;

    //static bools to disable things externally from this script if actions are being performed 
    public static bool isResizing = false;
    public static bool isDragging = false;
    private Vector3 Offset;
    Vector3 ParentSizeOffset;
    

    //private bool Resizable;

    // Constructor that takes the target VisualElement, edge size, length unit and Parent Container if the length unit is percentage 

    public PopupManipulator(UIDocument document, float windowEdgeSize, LengthUnit lengthUnit, VisualElement parentContainer)
    {
        //target = target;
        this.edgeSize = windowEdgeSize;
        this.lengthUnit = lengthUnit;
        Parent = parentContainer;
        uI_Manager = GameObject.FindFirstObjectByType<UI_Manager>();
        uI_Document = document;
    }
    public VisualElement target
    {
        get => _target;
        set
        {
            if (_target != null)
            {
                if (_target == value)
                    return;
                _target?.UnregisterCallback<PointerDownEvent>(DragBegin);
                _target?.UnregisterCallback<PointerUpEvent>(DragEnd);
                _target?.UnregisterCallback<PointerMoveEvent>(PointerMove);
                _target?.UnregisterCallback<PointerLeaveEvent>(PointerExit);
                _target?.UnregisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
                _target?.RemoveFromClassList("draggable");
                lastDroppable?.RemoveFromClassList("droppable--can-drop");
                lastDroppable = null;
            }
            _target = value;

            _target?.RegisterCallback<PointerDownEvent>(DragBegin);
            _target?.RegisterCallback<PointerUpEvent>(DragEnd);
            _target?.RegisterCallback<PointerMoveEvent>(PointerMove);
            _target?.RegisterCallback<PointerLeaveEvent>(PointerExit);
            _target?.RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
            _target?.AddToClassList("draggable");
        }
    }
    protected static readonly CustomStyleProperty<bool> draggableEnabledProperty
      = new CustomStyleProperty<bool>("--draggable-enabled");
    protected Vector3 offset;
    
    private VisualElement lastDroppable = null;
    private string _droppableId = "droppable";
    /** This is the USS class that is determines whether the target can be dropped
        on it. It is "droppable" by default. */
    public string droppableId
    {
        get => _droppableId;
        init => _droppableId = value;
    }
    /** This manipulator can be disabled. */
    public bool enabled { get; set; } = true;
    private PickingMode lastPickingMode;
    private string _removeClassOnDrag;
    /** Optional. Remove the given class from the target element during the drag.
        If removed, replace when drag ends. */
    public string removeClassOnDrag
    {
        get => _removeClassOnDrag;
        init => _removeClassOnDrag = value;
    }
    private bool removedClass = false;

    private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
    {
        if (e.customStyle.TryGetValue(draggableEnabledProperty, out bool got))
            enabled = got;
    }

    private void DragBegin(PointerDownEvent ev)
    {
        /*if (UI_Manager.DrawingEnabled)
        {
            return;
        }*/

        // Capture the mouse to track its movement
        target.CaptureMouse();

        //Initialize the ResizeManipulator if the mouse is detected on the corners
        if (resizeLeft || resizeRight || resizeTop || resizeBottom || resizeBottomLeftCorner || resizeBottomRightCorner || resizeTopLeftCorner || resizeTopRightCorner)
        {
            InitializeResizeManipulator(ev.position);
        }

        if (!enabled || isResizing)
        {
            return;
        }

        

        target.AddToClassList("draggable--dragging");

        if (removeClassOnDrag != null)
        {
            removedClass = target.ClassListContains(removeClassOnDrag);
            if (removedClass)
                target.RemoveFromClassList(removeClassOnDrag);
        }

        //Get the initial sizes of the Visual Elements incase the style changes while the target is being dragged
        initialTargetHeight = target.worldBound.height;
        initialTargetWidth = target.worldBound.width;

        UI_Manager.RestrictMovement = true;

        target.style.opacity = 0.6f;
        
        target.style.scale = new Vector2(0.98f, 0.98f);

        lastScreenSize = ResolvedScreenSize();

        ParentSizeOffset = new Vector3(lastScreenSize.x - Parent.worldBound.width, lastScreenSize.y - Parent.worldBound.height, 0);


        lastPickingMode = target.pickingMode;
        // Store the original mouse position and element size
        lastPosition = ev.localPosition;

        lastElementSize = new Vector2(target.resolvedStyle.width, target.resolvedStyle.height);

        // Calculate the bounds of the resizing area based on the edge size and length unit
        CalculateResizingBounds();

        ev.StopPropagation();

        Debug.Log("Mouse down");

        target.pickingMode = PickingMode.Ignore;

        isDragging = true;
        offset = ev.localPosition;
        Offset = ev.localPosition;
        target.CapturePointer(ev.pointerId);
    }

    private void DragEnd(IPointerEvent ev)
    {
        ResetResizeManipulator();

        if (!isDragging)
        {
            return;
        }
        UI_Manager.RestrictMovement = false;
        VisualElement droppable;
        bool canDrop = CanDrop(ev.position, out droppable);
        if (canDrop)
        {
            droppable.RemoveFromClassList("droppable--can-drop");
        }
        lastPosition = target.transform.position;
        lastElementSize = new Vector2(target.resolvedStyle.width, target.resolvedStyle.height);
        target.RemoveFromClassList("draggable--dragging");
        target.RemoveFromClassList("draggable--can-drop");
        target.style.scale = Vector2.one;
        target.style.opacity = 1;
        lastDroppable?.RemoveFromClassList("droppable--can-drop");
        lastDroppable = null;
        target.ReleasePointer(ev.pointerId);
        target.pickingMode = lastPickingMode;
        isDragging = false;
        if (canDrop)
            Drop(droppable);
        else
        //ResetPosition();
        if (removeClassOnDrag != null && removedClass)
        {
            target.AddToClassList(removeClassOnDrag);
        }
    }

    protected virtual void Drop(VisualElement droppable) {
    var e = DropEvent.GetPooled(this, droppable);
    e.target = this.target;
    // We send the event one tick later so that our changes to the class list
    // will take effect.
    this.target.schedule.Execute(() => e.target.SendEvent(e));
  }

  /** Change parent while preserving position via `transform.position`.

      Usage: While dragging-and-dropping an element, if the dropped element were
      to change its parent in the hierarchy, but preserve its position on
      screen, which can be done with `transform.position`. Then one can lerp
      that position to zero for a nice clean transition.

      Notes: The algorithm isn't difficult. It's find position wrt new parent,
      zero out the `transform.position`, add it to the parent, find position wrt
      new parent, set `transform.position` such that its screen position will be
      the same as before.

      The tricky part is when you add this element to a newParent, you can't
      query for its position (at least not in a way I could find). You have to
      wait a beat. Then whatever was necessary to update will update.
   */
  public static IVisualElementScheduledItem ChangeParent(VisualElement target,
                                                         VisualElement newParent) {
    var position_parent = target.ChangeCoordinatesTo(newParent, Vector2.zero);
    target.RemoveFromHierarchy();
    target.transform.position = Vector3.zero;
    newParent.Add(target);
    // ChangeCoordinatesTo will not be correct unless you wait a tick. #hardwon
    // target.transform.position = position_parent - target.ChangeCoordinatesTo(newParent,
    //                                                                      Vector2.zero);
    return target.schedule.Execute(() => {
      var newPosition = position_parent - target.ChangeCoordinatesTo(newParent,
                                                                     Vector2.zero);
      target.RemoveFromHierarchy();
      target.transform.position = newPosition;

      newParent.Add(target);
    });
  }

    /** Reset the target's position to zero.

        Note: Schedules the change so that the USS classes will be restored when
        run. (Helps when a "transitions" USS class is used.)
     */
    public virtual void ResetPosition()
    {
        target.transform.position = Vector3.zero;
    }



    protected virtual bool CanDrop(Vector3 position, out VisualElement droppable)
    {
        droppable = target.panel.Pick(position);
        var element = droppable;
        // Walk up parent elements to see if any are droppable.
        while (element != null && !element.ClassListContains(droppableId))
            element = element.parent;
        if (element != null)
        {
            droppable = element;
            return true;
        }
        return false;
    }


    public Vector3 ConstrainedTargetPosition(Vector3 mousePosition, float offset)
    {

        


        var gameWindowRect = Parent.worldBound;
        var targetRect = target.worldBound;

        maxX = gameWindowRect.xMax - targetRect.width * offset;
        maxY = gameWindowRect.yMax - targetRect.height * offset;

        Vector3 localSpacePosition = new Vector3();

        Vector3 finalPosition = new Vector3();

        /*// Adjusting the position so that the top edge of the targetRect cannot go past the top edge of the gameWindowRect
        finalPosition.y = Mathf.Clamp(finalPosition.y, minY, maxY);

        // Adjusting the position so that the right edge of the targetRect cannot go past the right edge of the gameWindowRect
        finalPosition.x = Mathf.Clamp(finalPosition.x, minX, maxX);*/

        //xMax is to the right and yMax is to the bottom
        if (Parent.resolvedStyle.flexDirection == FlexDirection.Column || Parent.resolvedStyle.flexDirection == FlexDirection.Row)
        {

            localSpacePosition = new Vector3(mousePosition.x - ParentSizeOffset.x, mousePosition.y + ParentSizeOffset.y, 0);

            finalPosition = localSpacePosition - Offset;


            maxX = gameWindowRect.xMax - targetRect.width * offset;
            maxY = gameWindowRect.yMax - targetRect.height * offset;
            minY = gameWindowRect.yMin - targetRect.height * (1 - offset);
            minX = gameWindowRect.xMin - targetRect.width * (1 - offset);

            // Adjusting the position so that the top edge of the targetRect cannot go past the top edge of the gameWindowRect
            finalPosition.y = Mathf.Clamp(finalPosition.y, minY, maxY);

            // Adjusting the position so that the right edge of the targetRect cannot go past the right edge of the gameWindowRect
            finalPosition.x = Mathf.Clamp(finalPosition.x, minX, maxX);

        }

        //xMax is to the right yMax is to the top
        if (Parent.resolvedStyle.flexDirection == FlexDirection.ColumnReverse)
        {
            localSpacePosition = new Vector3(mousePosition.x - ParentSizeOffset.x, (mousePosition.y - (Parent.worldBound.height - ParentSizeOffset.y)), 0);     
            Vector3 flippedOffset = (new Vector3(initialTargetWidth, initialTargetHeight) - Offset);
            finalPosition = (localSpacePosition + new Vector3(flippedOffset.x - initialTargetWidth, flippedOffset.y, 0));

            float screenHeight = ResolvedScreenSize().y - gameWindowRect.height;

            maxX = gameWindowRect.xMax - targetRect.width * offset;
            maxY = screenHeight - (gameWindowRect.yMax - targetRect.height * offset);
            minY = gameWindowRect.yMin + targetRect.height * (1- offset);
            minX = gameWindowRect.xMin - targetRect.width * (1 - offset);

            // Adjusting the position so that the top edge of the targetRect cannot go past the top edge of the gameWindowRect
            finalPosition.y = Mathf.Clamp(finalPosition.y, maxY, minY);

            // Adjusting the position so that the right edge of the targetRect cannot go past the right edge of the gameWindowRect
            finalPosition.x = Mathf.Clamp(finalPosition.x, minX, maxX);
        }

        //xMax is to the left and 
        if (Parent.resolvedStyle.flexDirection == FlexDirection.RowReverse)
        {
            localSpacePosition = new Vector3(((mousePosition.x - ParentSizeOffset.x)) - (Parent.worldBound.width - initialTargetWidth), (mousePosition.y) - ParentSizeOffset.y, 0);
            finalPosition = localSpacePosition - Offset;

            float screenWidth = ResolvedScreenSize().x - gameWindowRect.width;

            maxX = (screenWidth - (gameWindowRect.xMax - targetRect.width * offset));
            maxY = gameWindowRect.yMax - targetRect.height * offset;
            minY = gameWindowRect.yMin - targetRect.height * (1 - offset);
            minX = gameWindowRect.xMin + targetRect.width * (1 - offset);

            // Adjusting the position so that the top edge of the targetRect cannot go past the top edge of the gameWindowRect
            finalPosition.y = Mathf.Clamp(finalPosition.y, minY, maxY);

            // Adjusting the position so that the right edge of the targetRect cannot go past the right edge of the gameWindowRect
            finalPosition.x = Mathf.Clamp(finalPosition.x, maxX, minX);

     


        }

        

        return finalPosition;
    }

    public Vector3 FinalPosition;

    //Uses UIElements Pointer Events Position and not the localPosition or delta since there is a flickering issue when using += to update the windows position. So it mirrors the ev.position on certain flexmodes its parent has.
    private void PointerMove(PointerMoveEvent ev)
    {
        // actively look to see if the mouse position is on the edges of the window so that it can resize the window
        //ParentSizeOffset = new Vector3((Screen.width / uI_Document.panelSettings.scale) - Parent.worldBound.width, (Screen.height / uI_Document.panelSettings.scale) - Parent.worldBound.height, 0);
        ResizeManipulator(ev.position);
        CalculateResizingBounds();


        Vector3 delta = new Vector3();

        //ParentSizeOffset = new Vector3(Screen.width - ResolvedElementSize(Parent.worldBound.size).x, Screen.height  - ResolvedElementSize(Parent.worldBound.size).y, 0) * uI_Document.panelSettings.scale;
        
        //Debug.Log((invertedPosition));
        
        



        

        if (Parent != null && !isResizing)
        {
            //FinalPosition = ConstrainedDelta(ev.position - offset, 0.3f);
        }
        else
        {
            delta = ev.localPosition - (Vector3)offset;
        }

        if (isDragging)
        {
            target.transform.position = ConstrainedTargetPosition(ev.position, 0.3f);
        }

        if (!isDragging)
        {
            return;
        }

        if (!enabled)
        {
            DragEnd(ev);
            return;
        }

        

        if (isResizing)
        {
            // Skip updating target.transform.position if resizing is in progress
            // to prevent jittering
            return;
        }

        //if()

        

        if (CanDrop(ev.position, out var droppable))
        {
            target.AddToClassList("draggable--can-drop");
            droppable.AddToClassList("droppable--can-drop");
            if (lastDroppable != droppable)
            {
                lastDroppable?.RemoveFromClassList("droppable--can-drop");
            }
            lastDroppable = droppable;
        }
        else
        {
            target.RemoveFromClassList("draggable--can-drop");
            lastDroppable?.RemoveFromClassList("droppable--can-drop");
            lastDroppable = null;
        }
    }


    private void PointerExit(PointerLeaveEvent evt)
    {
        if (!isResizing)
        {
            target.AddToClassList("inactivePopoutHandle");
            target.RemoveFromClassList("activePopoutHandle");
        }
    }

    // Constructor that takes the target VisualElement, edge size, length unit and Parent Container if the length unit is percentage 


    //bool PopupCanResize = false;

    bool PositiveDirectionX = false;
    bool NegativeDirectionY = false;
    bool NegativeDirectionX = false;
    bool PositiveDirectionY = false;
    bool flippedDirectionX = false;
    bool flippedDirectionY = false;

    FlexDirection PreviousFlexDirection;

    // Handle the mouse button down event

    private void InitializeResizeManipulator(Vector2 mousePosition)
    {
        //PopupCanResize = true;
        
        canLockDrag = true;
        target.RemoveFromClassList("inactivePopoutHandle");
        target.AddToClassList("activePopoutHandle");

        // Capture the mouse to track its movement, this allows the PointerMove method to continue calling even if the pointer is outside of the target. It continues until ReleaseMouse() is called

        #region FlexDirection position offsets

        float parentWidth = Parent.worldBound.width;
        float parentHeight = Parent.worldBound.height;
        float targetHeight = target.worldBound.height;
        float targetWidth = target.worldBound.width;
        
        Vector3 targetTransform = ResolvedElementSize(target.transform.position);

        PositiveDirectionX = false;
        NegativeDirectionY = false;
        NegativeDirectionX = false;
        PositiveDirectionY = false;
        flippedDirectionX = false;
        flippedDirectionY = false;

        if (resizeLeft || resizeBottomLeftCorner)
        {
            if (Parent.resolvedStyle.flexDirection == FlexDirection.Row || Parent.resolvedStyle.flexDirection == FlexDirection.Column)
            {
                NegativeDirectionX = true;
            }
            if (Parent.resolvedStyle.flexDirection == FlexDirection.ColumnReverse)
            {
                NegativeDirectionX = true;
                PositiveDirectionY = true;
            }

            initialXmin = ResolvedScreenSize().x - Parent.worldBound.xMax;
            initialXmin = ResolvedScreenSize().x - Parent.worldBound.xMin;
            initialXmin = Parent.worldBound.yMax;
            initialXmin = Parent.worldBound.yMin;
            Parent.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.RowReverse); //TODO transition to Column reverse breaks the window position
        }
        if (resizeRight)
        {
            if (Parent.resolvedStyle.flexDirection == FlexDirection.RowReverse)
            {
                PositiveDirectionX = true;
            }
            if (Parent.resolvedStyle.flexDirection == FlexDirection.ColumnReverse)
            {
                PositiveDirectionY = true;
            }

            Parent.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
        }

        if (resizeTop || resizeTopRightCorner || resizeTopLeftCorner)
        {
            if (Parent.resolvedStyle.flexDirection == FlexDirection.Column)
            {
                NegativeDirectionY = true;

            }
            if (Parent.resolvedStyle.flexDirection == FlexDirection.RowReverse)
            {
                /*                    flippedDirectionX = true;*/

                flippedDirectionX = true;
                flippedDirectionY = true;
                /*
                                    Debug.Log("Row Reverse change");
                                    flippedDirectionY = true;*/
            }



            Parent.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.ColumnReverse);
        }
        if (resizeBottom || resizeBottomRightCorner)
        {
            if (Parent.resolvedStyle.flexDirection == FlexDirection.ColumnReverse)
            {
                PositiveDirectionY = true;
            }
            if (Parent.resolvedStyle.flexDirection == FlexDirection.RowReverse)
            {
                PositiveDirectionX = true;
            }


            Parent.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);

        }



        if (PositiveDirectionX)
        {
            float FlippedXDirection = parentWidth - (targetWidth + -(target.transform.position.x));

            Vector3 FlippedValue = new Vector3(FlippedXDirection, targetTransform.y, 0);
            target.transform.position = (FlippedValue) * uI_Document.panelSettings.scale;
        }

        if (NegativeDirectionY)
        {
            float FlippedYDirection = -(parentHeight - (target.transform.position.y + targetHeight)); 
            Vector3 FlippedValue = new Vector3(targetTransform.x, FlippedYDirection, 0);
            target.transform.position = (FlippedValue) * uI_Document.panelSettings.scale;
        }

        if (NegativeDirectionX)
        {
            float FlippedXDirection = -(parentWidth - (targetWidth + (target.transform.position.x)));
            Vector3 FlippedValue = new Vector3(FlippedXDirection, target.transform.position.y, 0);
            target.transform.position = (FlippedValue) * uI_Document.panelSettings.scale;
        }

        if (PositiveDirectionY)
        {
            float FlippedYDirection = (parentHeight - (-(target.transform.position.y) + (targetHeight)));
            Vector3 FlippedValue = new Vector3(target.transform.position.x, FlippedYDirection, 0);
            target.transform.position = (FlippedValue) * uI_Document.panelSettings.scale;
        }

        if (flippedDirectionX)
        {
            float FlippedXDirection = (target.transform.position.x) + (parentWidth - targetWidth);

            Vector3 FlippedValue = new Vector3(FlippedXDirection, targetTransform.y, 0);
            target.transform.position = (FlippedValue) * uI_Document.panelSettings.scale;
        }

        if (flippedDirectionY)
        {
            float FlippedYDirection = (((target.transform.position.y))) - (parentHeight - targetHeight);
            Vector3 FlippedValue = new Vector3(target.transform.position.x, FlippedYDirection, 0);
            target.transform.position = (FlippedValue) * uI_Document.panelSettings.scale;
        }

        PreviousFlexDirection = Parent.resolvedStyle.flexDirection;
        #endregion

        isResizing = true;
    }

    


    // Handle the mouse button up event
    private void ResetResizeManipulator()
    {
        
        if (isResizing)
        {
            target.AddToClassList("inactivePopoutHandle");
            target.RemoveFromClassList("activePopoutHandle");

            // Release the mouse capture
            target.ReleaseMouse();
            canLockDrag = false;
/*            leftcanResize = false;
            rightCanResize = false;
            bottomCanResize = false;
            topCanResize = false;
            topLeftCornerCanResize = false;
            bottomLeftCornerCanResize = false;
            topRightCornerCanReSize = false;
            bottomRighCornerCanResize = false;*/
            //PopupCanResize = false;
            isResizing = false;
            Debug.Log("Mouse up");
        }
    }

    // Calculate the resizing bounds based on the edge size and length unit
    private void CalculateResizingBounds()
    {
        Rect rect = target.worldBound;
        minX = rect.xMin + edgeSize;
        minY = rect.yMin + edgeSize;
        maxX = rect.xMax - edgeSize;
        maxY = rect.yMax - edgeSize;
    }

    bool resizeLeft;
    bool resizeRight;
    bool resizeTop;
    bool resizeBottom;
    bool resizeTopLeftCorner;
    bool resizeTopRightCorner;
    bool resizeBottomRightCorner;
    bool resizeBottomLeftCorner;

    bool canLockDrag;

/*    bool leftcanResize;
    bool rightCanResize;
    bool bottomCanResize;
    bool topCanResize;
    bool topLeftCornerCanResize;
    bool bottomLeftCornerCanResize;
    bool topRightCornerCanReSize;
    bool bottomRighCornerCanResize;*/

    

    private void ResizeManipulator(Vector2 mousePosition)
    {
        //Highlight the margins of the window
        
        Vector2 delta = new Vector2(mousePosition.x, mousePosition.y);
        Rect rect = target.worldBound;
        CalculateResizingBounds();


        if (!canLockDrag)
        {
            resizeLeft = delta.x >= rect.xMin && delta.x <= minX;
            resizeRight = delta.x <= rect.xMax && delta.x >= maxX;
            resizeTop = delta.y >= rect.yMin && delta.y <= minY;
            resizeBottom = delta.y <= rect.yMax && delta.y >= maxY;

            resizeBottomRightCorner = resizeRight && resizeBottom;
            resizeBottomLeftCorner = resizeLeft && resizeBottom;
            resizeTopRightCorner = resizeTop && resizeRight;
            resizeTopLeftCorner = resizeTop & resizeLeft;

            if (resizeTopRightCorner || resizeBottomRightCorner || resizeTopLeftCorner || resizeBottomLeftCorner)
            {
                resizeLeft = false;
                resizeRight = false;
                resizeTop = false;
                resizeBottom = false;
            }

            /*resizeBottomRightCorner = (delta.x <= rect.xMax && delta.x >= maxX) && (delta.y <= rect.yMax && delta.y >= maxY);
            resizeBottomLeftCorner = (delta.x >= rect.xMin && delta.x <= minX) && (delta.y <= rect.yMax && delta.y >= maxY);
            resizeTopRightCorner = (delta.x <= rect.xMax && delta.x >= maxX) && (delta.y >= rect.yMin && delta.y <= minY);
            resizeTopLeftCorner = (delta.x >= rect.xMin && delta.x <= minX) && (delta.y >= rect.yMin && delta.y <= minY);

            if (!resizeTopRightCorner && !resizeBottomRightCorner && !resizeTopLeftCorner && !resizeBottomLeftCorner)
            {
                resizeLeft = delta.x >= rect.xMin && delta.x <= minX;
                resizeRight = delta.x <= rect.xMax && delta.x >= maxX;
                resizeTop = delta.y >= rect.yMin && delta.y <= minY;
                resizeBottom = delta.y <= rect.yMax && delta.y >= maxY;
            }*/
        }

        //use a style class to highlight the edges of the window when the cursor hover over the resize margin

        if (!isResizing)
        {
            
            if (resizeLeft || resizeRight || resizeTop || resizeBottom || resizeBottomLeftCorner || resizeBottomRightCorner || resizeTopLeftCorner || resizeTopRightCorner)
            {
                target.RemoveFromClassList("inactivePopoutHandle");
                target.AddToClassList("activePopoutHandle");
            }
            if (!resizeLeft & !resizeRight & !resizeTop & !resizeBottom & !resizeBottomLeftCorner & !resizeBottomRightCorner & !resizeTopLeftCorner & !resizeTopRightCorner)
            {
                uI_Manager.ResetCursor();
                target.AddToClassList("inactivePopoutHandle");
                target.RemoveFromClassList("activePopoutHandle");
            }
            
        }

        ResizeWindow(mousePosition);
    }

    
    private void ResizeWindow(Vector2 mousePosition)
    {

        float clampedXMax = (Parent.worldBound.xMax) + Mathf.Clamp(Parent.worldBound.xMin - target.worldBound.xMin, 0, Parent.worldBound.xMin - target.worldBound.xMin);
        float clampedXMin = (Parent.worldBound.xMin) - Mathf.Clamp(target.worldBound.xMax - Parent.worldBound.xMax,0, target.worldBound.xMax - Parent.worldBound.xMax);
        float clampedYMax = (Parent.worldBound.yMax) + Mathf.Clamp(Parent.worldBound.yMin - target.worldBound.yMin, 0, Parent.worldBound.yMin - target.worldBound.yMin);
        float clampedYMin = (Parent.worldBound.yMin) - Mathf.Clamp(target.worldBound.yMax - Parent.worldBound.yMax, 0, target.worldBound.yMax - Parent.worldBound.yMax);

        Vector2 clampedMousePosition = new Vector2(Mathf.Clamp(mousePosition.x, Parent.worldBound.xMin, Parent.worldBound.xMax), Mathf.Clamp(mousePosition.y, Parent.worldBound.yMin, Parent.worldBound.yMax));
        Debug.Log(new Vector2(clampedXMin, clampedXMax));

        Vector2 flippedPosition = new Vector2((lastScreenSize.x - clampedMousePosition.x), (lastScreenSize.y - clampedMousePosition.y));

        Vector2 finalMousePosition = ResolvedElementSize(flippedPosition);


        if (resizeLeft)
        {
            Debug.Log("Mouse detected on the left");

            uI_Manager.SetHorizontalResizeCursor();

            

            if (isResizing)
            {
                HandleResizeLeft(finalMousePosition);
            }
        }
        else if (resizeRight)
        {
            Debug.Log("Mouse detected on the right");

            uI_Manager.SetHorizontalResizeCursor();


            if (isResizing)
            {
                HandleResizeRight(finalMousePosition);
            }
        }
        else if (resizeTop)
        {
            Debug.Log("Mouse detected on the top");

            uI_Manager.SetVerticalResizeCursor();

            if (isResizing)
            {
                HandleResizeTop(finalMousePosition);
            }

        }
        else if (resizeBottom)
        {
            Debug.Log("Mouse detected on the bottom");

            uI_Manager.SetVerticalResizeCursor();

            if (isResizing)
            {
                HandleResizeBottom(finalMousePosition);
            }
        }
        else if (resizeTopLeftCorner)
        {
            Debug.Log("Mouse detected in the top-left corner");

            uI_Manager.SetDiagonalLeftResizeCursor();

            if (isResizing)
            {
                HandleResizeTopLeftCorner(finalMousePosition);
            }

        }
        else if (resizeBottomLeftCorner)
        {
            Debug.Log("Mouse detected in the bottom-left corner");

            uI_Manager.SetDiagonalRightResizeCursor();

            if (isResizing)
            {
                HandleResizeBottomLeftCorner(finalMousePosition);
            }
        }
        else if (resizeTopRightCorner)
        {
            Debug.Log("Mouse detected in the top-right corner");

            uI_Manager.SetDiagonalRightResizeCursor();

            if (isResizing)
            {
                HandleResizeTopRightCorner(finalMousePosition);
            }
        }
        else if (resizeBottomRightCorner)
        {
            uI_Manager.SetDiagonalLeftResizeCursor();

            Debug.Log("Mouse detected in the bottom-right corner");

            if (isResizing)
            {
                HandleResizeBottomRightCorner(finalMousePosition);
            }
        }
    }

    float leftSize;
    float leftPixels;
    private void HandleResizeLeft(Vector2 delta)
    {
        // Update the element's width in the style
        float offset = lastScreenSize.x - target.worldBound.xMax;
        if (lengthUnit == LengthUnit.Percent)
        {
            float newWidth = (delta.x - offset) / Parent.worldBound.width * 100;
            leftSize = (delta.x - offset);
            newWidth = Mathf.Clamp(newWidth, 10f, 200f);

            target.style.minWidth = new Length(newWidth, LengthUnit.Percent);
            target.style.maxWidth = new Length(newWidth, LengthUnit.Percent);
        }
        else
        {
            float newWidth = delta.x;

            target.style.width = new Length(newWidth, LengthUnit.Pixel);
            target.style.minWidth = new Length(newWidth, LengthUnit.Pixel);
        }
    }

    private void HandleResizeRight(Vector2 delta)
    {

        float offset = lastScreenSize.x - target.worldBound.xMin;

        // Update the element's width in the style
        if (lengthUnit == LengthUnit.Percent)
        {
            float newWidth = ((lastScreenSize.x - delta.x) - target.worldBound.xMin) / Parent.worldBound.width  * 100;
            newWidth = Mathf.Clamp(newWidth, 10f, 200f);

            target.style.minWidth = new Length(newWidth, LengthUnit.Percent);
            target.style.maxWidth = new Length(newWidth, LengthUnit.Percent);
        }
        else
        {
            float newWidth = delta.x;

            newWidth = Mathf.Clamp(newWidth, 0f, float.MaxValue);

            target.style.width = new Length(newWidth, LengthUnit.Pixel);
            target.style.minWidth = new Length(newWidth, LengthUnit.Pixel);
        }
    }

    
    private void HandleResizeTop(Vector2 delta)
    {
        float offset = lastScreenSize.y - target.worldBound.yMax;

        // Update the element's height in the style
        if (lengthUnit == LengthUnit.Percent)
        {
            float  newWidth = (((delta.y - offset) / Parent.worldBound.height * 100));


            Debug.Log(delta.y);
            newWidth = Mathf.Clamp(newWidth, 10f, 200f);

            target.style.minHeight = new Length(newWidth, LengthUnit.Percent);
            target.style.maxHeight = new Length(newWidth, LengthUnit.Percent);
        }
        else
        {
            float newHeight = delta.y;

            newHeight = Mathf.Clamp(newHeight, 0f, float.MaxValue);

            target.style.height = new Length(newHeight, LengthUnit.Pixel);
            target.style.minHeight = new Length(newHeight, LengthUnit.Pixel);
        }
    }

    private void HandleResizeBottom(Vector2 delta)
    {
        float offset = lastScreenSize.y - target.worldBound.yMin;
        // Update the element's height in the style
        if (lengthUnit == LengthUnit.Percent)
        {
            float newHeight = -((delta.y - offset) / Parent.worldBound.height * 100);
            newHeight = Mathf.Clamp(newHeight, 10f, 200f);

            target.style.minHeight = new Length(newHeight, LengthUnit.Percent);
            target.style.maxHeight = new Length(newHeight, LengthUnit.Percent);
        }
        else
        {
            float newHeight = (delta.y - offset) / Parent.worldBound.height * 100;
            newHeight = Mathf.Clamp(newHeight, 0f, float.MaxValue);

            target.style.height = new Length(newHeight, LengthUnit.Pixel);
            target.style.minHeight = new Length(newHeight, LengthUnit.Pixel);
        }
    }

    //This handler will be cheated a bit since there is currently no flex direction that handles both left and top flexing
    private void HandleResizeTopLeftCorner(Vector2 delta)
    {
        Vector3 flippedOffset = new Vector3(Parent.worldBound.width - (leftSize), target.transform.position.y, target.transform.position.z);
        target.transform.position = flippedOffset;
        HandleResizeTop(delta);
        HandleResizeLeft(delta);
    }

    private void HandleResizeBottomLeftCorner(Vector2 delta)
    {
        HandleResizeBottom(delta);
        HandleResizeLeft(delta);
    }

    private void HandleResizeTopRightCorner(Vector2 delta)
    {
        HandleResizeTop(delta);
        HandleResizeRight(delta);
    }

    private void HandleResizeBottomRightCorner(Vector2 delta)
    {
        HandleResizeBottom(delta);
        HandleResizeRight(delta);
    }


    public Vector2 ResolvedScreenSize()
    {
        Vector2 ScreenSize = new Vector2(Screen.width / uI_Document.panelSettings.scale, Screen.height / uI_Document.panelSettings.scale);

        return ScreenSize;
    }

    public Vector3 ResolvedElementSize(Vector2 size)
    {
        Vector2 finalElementSize = size; /// uI_Document.panelSettings.scale;

        return finalElementSize;
    }
    public float ScaledFloatValue(float value)
    {
        float final = value * uI_Document.panelSettings.scale;
        return final;

    }
}

public abstract class Popup
{
    

}

/** This event represents a runtime drag and drop event. */
public class DropEvent : EventBase<DropEvent> {
  public PopupManipulator dragger { get; protected set; }
  public VisualElement droppable { get; protected set; }

  protected override void Init() {
    base.Init();
    this.LocalInit();
  }

  private void LocalInit() {
    this.bubbles = true;
    this.tricklesDown = false;
  }

  public static DropEvent GetPooled(PopupManipulator dragger, VisualElement droppable) {
    DropEvent pooled = EventBase<DropEvent>.GetPooled();
    pooled.dragger = dragger;
    pooled.droppable = droppable;
    return pooled;
  }

  public DropEvent() => this.LocalInit();
}

// This hack allows us to use init properties in earlier versions of Unity.
#if UNITY_5_3_OR_NEWER && ! UNITY_2021_OR_NEWER
// https://stackoverflow.com/a/62656145
namespace System.Runtime.CompilerServices {
  using System.ComponentModel;
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class IsExternalInit{}
}
#endif
