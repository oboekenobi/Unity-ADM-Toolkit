using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using ADM.UISystem;

public class ToolTipManipulator : IManipulator
{
    private VisualElement element;
    private VisualElement _target;
    private VisualElement Root;
    public VisualElement toolTipContainer;
    private bool Drag;
    public ToolTipManipulator(VisualElement root, bool drag, VisualElement ToolTip)
    {
        Drag = drag;
        toolTipContainer = ToolTip;
        Root = root;
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
                //_target.RegisterCallback<MouseEnterEvent>(MouseIn);
                //_target.RegisterCallback<MouseOutEvent>(MouseOut);
                _target.UnregisterCallback<PointerDownEvent>(RegisterLaserToolTip);
                _target.UnregisterCallback<PointerMoveEvent>(UpdateLaserToolTip);
                _target.UnregisterCallback<PointerUpEvent>(UnRegisterToolTip);
            }
            _target = value;
            //_target.UnregisterCallback<MouseEnterEvent>(MouseIn);
            //_target.UnregisterCallback<MouseOutEvent>(MouseOut);
            _target.RegisterCallback<PointerDownEvent>(RegisterLaserToolTip);
            _target.RegisterCallback<PointerMoveEvent>(UpdateLaserToolTip);
            _target.RegisterCallback<PointerUpEvent>(UnRegisterToolTip);
        }
    }

    public void UpdateLaserToolTip(PointerMoveEvent ev)
    {
        /*if (!isDragging)
        {
            return;
        }*/
        Debug.Log(ev.position);

        Vector3 tooltipPosition = new Vector3(ev.position.x, ev.position.y, ev.position.z);
        toolTipContainer.transform.position = tooltipPosition;
    }

    public void RegisterLaserToolTip(PointerDownEvent ev)
    {
        /*if (isDragging)
        {
            return;
        }
        Vector3 tooltipPosition = new Vector3(ev.position.x, ev.position.y + mouseOffset, ev.position.z);
        tooltipContainer.transform.position = tooltipPosition;*/
        toolTipContainer.style.opacity = 100;
        //isDragging = true;

        target.CaptureMouse();

        toolTipContainer.CapturePointer(ev.pointerId);
    }
    public void UnRegisterToolTip(PointerUpEvent ev)
    {

        toolTipContainer.style.opacity = 0;
        //isDragging = false;
        target.ReleaseMouse();
        toolTipContainer.ReleasePointer(ev.pointerId);
    }


    private void MouseIn(MouseEnterEvent e)
    {
        if (element == null)
        {
            element = new VisualElement();
            element.style.backgroundColor = Color.blue;
            element.style.position = Position.Absolute;
            element.style.left = target.worldBound.center.x;
            element.style.top = target.worldBound.yMin;
            var label = new Label(target.tooltip);
            label.style.color = Color.white;

            element.Add(label);
            //var root = (VisualElement)UiHelper.FindRootElement(this.target);
            target.Add(element);

        }
        element.style.visibility = Visibility.Visible;
        element.BringToFront();
    }

    private void MouseOut(MouseOutEvent e)
    {
        element.style.visibility = Visibility.Hidden;
    }
}
