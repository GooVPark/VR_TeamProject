using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SelectedEffectOutline;

public class OutlineHandler : MonoBehaviour
{
    private Outline outline;

    public Color hoverColor;
    public Color selectColor;

    private Color origin;

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void OnHoverEntered()
    {
        outline.OutlineEnable();
    }

    public void OnHoverExited()
    {
        outline.OutlineDisable();
    }

    public void OnSelectEntered()
    {
        outline.m_OutlineColor= selectColor;
        outline.SetOutLine();
    }

    public void OnSelectExit()
    {
        outline.m_OutlineColor = hoverColor;
        outline.SetOutLine();
    }
}
