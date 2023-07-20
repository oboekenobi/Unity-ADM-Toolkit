/*
 * Summary:
 * This script provides a manipulator for a VisualElement in Unity's UI Toolkit,
 * allowing the element to be resized by dragging on the edges.
 *
 * Instructions:
 * To apply the manipulator to a VisualElement:
 * 1. If you are using Percentage as a length unit then ensure your panel settings are set to Scale With Screen Size!
 * 2. Create an instance of ResizableWindowManipulator, passing the target VisualElement, edgeSize, lengthUnit and its parent container as parameters.
 * 3. Attach the manipulator to the target element using targetElement.AddManipulator(manipulator).
 * 4. The target element should have a parent element that acts as the container to limit the resizing area.
 */

using ADM.UISystem;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class ResizableWindowManipulator : MouseManipulator
{
    private VisualElement targetElement;
    private Vector2 originalMousePosition;
    private Vector2 originalElementSize;
    private float edgeSize;
    private LengthUnit lengthUnit;
    private float minX;
    private float minY;
    private float maxX;
    private float maxY;
    private bool resizing;
    private VisualElement parent;

    // Constructor that takes the target VisualElement, edge size, length unit and Parent Container if the length unit is percentage 
    public ResizableWindowManipulator(VisualElement target, float edgeSize, LengthUnit lengthUnit, VisualElement parentContainer)
    {
        targetElement = target;
        this.edgeSize = edgeSize;
        this.lengthUnit = lengthUnit;
        parent = parentContainer;
        CalculateResizingBounds();
        activators.Add(new ManipulatorActivationFilter { button = UnityEngine.UIElements.MouseButton.LeftMouse });
    }

    // Register event callbacks on the target element
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseEnter);
        //target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    // Unregister event callbacks from the target element
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        //target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseEnter);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    // Handle the mouse button down event
    private void OnMouseDown(MouseDownEvent evt)
    {
        if (CanStartManipulation(evt))
        {
            // Store the original mouse position and element size
            originalMousePosition = evt.mousePosition;
            originalElementSize = new Vector2(targetElement.resolvedStyle.width, targetElement.resolvedStyle.height);

            // Calculate the bounds of the resizing area based on the edge size and length unit
            CalculateResizingBounds();

            // Capture the mouse to track its movement
            target.CaptureMouse();

            evt.StopPropagation();

            Debug.Log("Mouse down");
        }
    }

    // Handle the mouse move event
    private void OnMouseMove(MouseMoveEvent evt)
    {

        if (target.HasMouseCapture() && evt.button == (int)UnityEngine.UIElements.MouseButton.LeftMouse)
        {

            //Vector2 delta = evt.mousePosition; - originalMousePosition;

            // Calculate the delta movement, the x position is flipped since the UI Elements measure from the right to left
            Vector2 delta = new Vector2((Screen.width - evt.mousePosition.x), evt.mousePosition.y);

            // Calculate the new width and height of the element
            float newWidth = originalElementSize.x;
            float newHeight = originalElementSize.y;

            float mouseX = parent.worldBound.xMax - evt.mousePosition.x;

            // Check if the mouse is within the resizing bounds
            if (delta.x == minX || delta.x == maxX || evt.mousePosition.y == minY || evt.mousePosition.y == maxY|| resizing)
            {
                resizing = true;

                // Determine the resizing direction based on the mouse position
                /*bool resizeRight = delta.x > (maxX - edgeSize);
                bool resizeLeft = delta.x < (minX + edgeSize);
                bool resizeTop = evt.mousePosition.y < (minY + edgeSize);
                bool resizeBottom = evt.mousePosition.y > (maxY - edgeSize);

                // Resize the element based on the resizing direction
                if (resizeRight)
                    newWidth += delta.x;
                else if (resizeLeft)
                    newWidth -= delta.x;
                if (resizeBottom)
                    newHeight += delta.y;
                else if (resizeTop)
                    newHeight -= delta.y;
*/
                // Update the element's width and height in the style
                if (lengthUnit == LengthUnit.Percent)
                {
                    //newWidth = Mathf.Clamp(newWidth, 0f, 100f);
                    //newHeight = Mathf.Clamp(newHeight, 0f, 100f);
                    //
                    float windowHeight = parent.worldBound.height;

                    float windowWidth = parent.worldBound.width;

                    newWidth = delta.x / windowWidth * 100f;
                    newHeight = delta.y / windowHeight * 100f;

                    targetElement.style.minWidth = new Length(newWidth, LengthUnit.Percent);
                    targetElement.style.maxWidth = new Length(newWidth, LengthUnit.Percent);
                    targetElement.style.minHeight = new Length(newHeight, LengthUnit.Percent);
                    targetElement.style.maxHeight = new Length(newHeight, LengthUnit.Percent);
                }
                else
                {
                    newWidth = Mathf.Clamp(newWidth, 0f, float.MaxValue);
                    newHeight = Mathf.Clamp(newHeight, 0f, float.MaxValue);
                    targetElement.style.width = new Length(newWidth, LengthUnit.Pixel);
                    targetElement.style.minWidth = new Length(newWidth, LengthUnit.Pixel);
                    targetElement.style.minHeight = new Length(newHeight, LengthUnit.Pixel);
                }

                evt.StopPropagation();

                //Debug.Log("Mouse move");
            }
        }
    }

    // Handle the mouse button up event
    private void OnMouseUp(MouseUpEvent evt)
    {
        if (target.HasMouseCapture() && evt.button == (int)UnityEngine.UIElements.MouseButton.LeftMouse)
        {
            // Release the mouse capture
            target.ReleaseMouse();
            resizing= false;

            leftResizing = false;
            rightResizing = false;
            bottomResizing = false;
            topResizing = false;
            topleftResizing = false;
            bottomleftResizing = false;
            topRightResizing = false;
            bottomRightResizing = false;

            evt.StopPropagation();

            Debug.Log("Mouse up");
        }
    }

    // Calculate the resizing bounds based on the edge size and length unit
   
    private void CalculateResizingBounds()
    {
        Rect rect = targetElement.worldBound;
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


    bool leftResizing;
    bool rightResizing;
    bool bottomResizing;
    bool topResizing;
    bool topleftResizing;
    bool bottomleftResizing;
    bool topRightResizing;
    bool bottomRightResizing;

    /*    private void OnMouseEnter(MouseMoveEvent evt)
        {
            Vector2 delta = new Vector2(evt.mousePosition.x, evt.mousePosition.y);
            Rect rect = targetElement.worldBound;
            CalculateResizingBounds();


            resizeBottomRightCorner = (delta.x >= rect.xMin && delta.x <= minX) && (delta.y >= rect.yMin && delta.y <= minY);
            resizeBottomLeftCorner = (delta.x <= rect.xMax && delta.x >= maxX) && (delta.y >= rect.yMin && delta.y <= minY);
            resizeTopRightCorner = (delta.x >= rect.xMin && delta.x <= minX) && (delta.y <= rect.yMax && delta.y >= maxY);
            resizeTopLeftCorner = (delta.x <= rect.xMax && delta.x >= maxX) && (delta.y <= rect.yMax && delta.y >= maxY);

            if (!resizeTopRightCorner && !resizeBottomRightCorner && !resizeTopLeftCorner && !resizeBottomLeftCorner)
            {
                resizeLeft = delta.x >= rect.xMin && delta.x <= minX;
                resizeRight = delta.x <= rect.xMax && delta.x >= maxX;
                resizeTop = delta.y >= rect.yMin && delta.y <= minY;
                resizeBottom = delta.y <= rect.yMax && delta.y >= maxY;
            }


            #region Edge Detection
            //Resize to the right 
            if (resizeLeft)
            {
                Debug.Log("Mouse detected on the left");
            }

            //Resize to the left
            if (resizeRight)
            {
                Debug.Log("Mouse detected on the right");
            }

            //Resize to the Bottom
            if (resizeTop)
            {
                Debug.Log("Mouse detected on the Top");
            }

            //Reise to the Top
            if (resizeBottom)
            {
                Debug.Log("Mouse detected on the Bottom");
            }
            #endregion

            #region Corner Detection

            if (resizeTopLeftCorner)
            {
                Debug.Log("Mouse detected in the top-left corner");
            }
            if (resizeBottomLeftCorner)
            {
                Debug.Log("Mouse detected in the bottom-left corner");
            }

            if (resizeTopRightCorner)
            {
                Debug.Log("Mouse detected in the top-right corner");
            }
            else if (resizeBottomRightCorner)
            {
                Debug.Log("Mouse detected in the bottom-right corner");
            }

            #endregion
        }*/

    private void OnMouseEnter(MouseMoveEvent evt)
    {
        Vector2 delta = new Vector2(evt.mousePosition.x, evt.mousePosition.y);
        Rect rect = targetElement.worldBound;
        CalculateResizingBounds();

        resizeBottomRightCorner = (delta.x >= rect.xMin && delta.x <= minX) && (delta.y >= rect.yMin && delta.y <= minY);
        resizeBottomLeftCorner = (delta.x <= rect.xMax && delta.x >= maxX) && (delta.y >= rect.yMin && delta.y <= minY);
        resizeTopRightCorner = (delta.x >= rect.xMin && delta.x <= minX) && (delta.y <= rect.yMax && delta.y >= maxY);
        resizeTopLeftCorner = (delta.x <= rect.xMax && delta.x >= maxX) && (delta.y <= rect.yMax && delta.y >= maxY);

        if (!resizeTopRightCorner && !resizeBottomRightCorner && !resizeTopLeftCorner && !resizeBottomLeftCorner)
        {
            resizeLeft = delta.x >= rect.xMin && delta.x <= minX;
            resizeRight = delta.x <= rect.xMax && delta.x >= maxX;
            resizeTop = delta.y >= rect.yMin && delta.y <= minY;
            resizeBottom = delta.y <= rect.yMax && delta.y >= maxY;
        }

        ResizeWindow(evt.mousePosition);

        
    }


    private void ResizeWindow(Vector2 mousePosition)   
    {
        Vector2 delta = new Vector2((Screen.width - mousePosition.x), mousePosition.y);

        if (resizeLeft || leftResizing)
        {
            Debug.Log("Mouse detected on the left");

            if (target.HasMouseCapture())
            {
                leftResizing = true;
                HandleResizeLeft(delta);
            }
        }
        else if (resizeRight || rightResizing)
        {
            Debug.Log("Mouse detected on the right");

            if (target.HasMouseCapture())
            {
                rightResizing = true;
                HandleResizeRight(delta);
            }
        }
        else if (resizeTop || topResizing)
        {
            Debug.Log("Mouse detected on the top");

            if (target.HasMouseCapture())
            {
                topResizing = true;
                HandleResizeTop(delta);
            }
                
        }
        else if (resizeBottom || bottomResizing)
        {
            Debug.Log("Mouse detected on the bottom");

            if (target.HasMouseCapture())
            {
                bottomResizing = true;
                HandleResizeBottom(delta);
            }
        }
        else if (resizeTopLeftCorner || topleftResizing)
        {
            Debug.Log("Mouse detected in the top-left corner");
            if (target.HasMouseCapture())
            {
                topleftResizing = true;
                HandleResizeTopLeftCorner(delta);
            }
            
        }
        else if (resizeBottomLeftCorner || bottomleftResizing)
        {
            Debug.Log("Mouse detected in the bottom-left corner");

            if (target.HasMouseCapture())
            {
                bottomleftResizing = true;
                HandleResizeBottomLeftCorner(delta);
            }
        }
        else if (resizeTopRightCorner || topRightResizing)
        {
            Debug.Log("Mouse detected in the top-right corner");
            
            if (target.HasMouseCapture())
            {
                topRightResizing = true;
                HandleResizeTopRightCorner(delta);
            }
        }
        else if (resizeBottomRightCorner || bottomRightResizing)
        {
           
            Debug.Log("Mouse detected in the bottom-right corner");
            if (target.HasMouseCapture())
            {
                bottomRightResizing = true;
                HandleResizeBottomRightCorner(delta);
            }
        }
    }

    private void HandleResizeLeft(Vector2 delta)
    {
        // Update the element's width in the style
        float offset = Screen.width - targetElement.worldBound.xMax;
        if (lengthUnit == LengthUnit.Percent)
        {
            float newWidth = (delta.x + offset)/ parent.worldBound.width * 100;
            Debug.Log(new Vector2(newWidth, offset));

            targetElement.style.minWidth = new Length(newWidth, LengthUnit.Percent);
            targetElement.style.maxWidth = new Length(newWidth, LengthUnit.Percent);
        }
        else
        {
            float newWidth = delta.x;

            targetElement.style.width = new Length(newWidth, LengthUnit.Pixel);
            targetElement.style.minWidth = new Length(newWidth, LengthUnit.Pixel);
        }
    }

    private void HandleResizeRight(Vector2 delta)
    {
        float newWidth = 0;

        // Update the element's width in the style
        if (lengthUnit == LengthUnit.Percent)
        {
            newWidth = Mathf.Clamp(newWidth, 0f, 100f);

            targetElement.style.minWidth = new Length(newWidth, LengthUnit.Percent);
            targetElement.style.maxWidth = new Length(newWidth, LengthUnit.Percent);
        }
        else
        {
            newWidth = Mathf.Clamp(newWidth, 0f, float.MaxValue);

            targetElement.style.width = new Length(newWidth, LengthUnit.Pixel);
            targetElement.style.minWidth = new Length(newWidth, LengthUnit.Pixel);
        }
    }

    private void HandleResizeTop(Vector2 delta)
    {
        float newHeight = 0;

        // Update the element's height in the style
        if (lengthUnit == LengthUnit.Percent)
        {
            newHeight = Mathf.Clamp(newHeight, 0f, 100f);

            targetElement.style.minHeight = new Length(newHeight, LengthUnit.Percent);
            targetElement.style.maxHeight = new Length(newHeight, LengthUnit.Percent);
        }
        else
        {
            newHeight = Mathf.Clamp(newHeight, 0f, float.MaxValue);

            targetElement.style.height = new Length(newHeight, LengthUnit.Pixel);
            targetElement.style.minHeight = new Length(newHeight, LengthUnit.Pixel);
        }
    }

    private void HandleResizeBottom(Vector2 delta)
    {
        float newHeight = 0;

        // Update the element's height in the style
        if (lengthUnit == LengthUnit.Percent)
        {
            newHeight = Mathf.Clamp(newHeight, 0f, 100f);

            targetElement.style.minHeight = new Length(newHeight, LengthUnit.Percent);
            targetElement.style.maxHeight = new Length(newHeight, LengthUnit.Percent);
        }
        else
        {
            newHeight = Mathf.Clamp(newHeight, 0f, float.MaxValue);

            targetElement.style.height = new Length(newHeight, LengthUnit.Pixel);
            targetElement.style.minHeight = new Length(newHeight, LengthUnit.Pixel);
        }
    }

    private void HandleResizeTopLeftCorner(Vector2 delta)
    {
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


}









