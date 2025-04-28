using UnityEngine;

public static class OutlineHelper
{
    // Sets the outline color on a GameObject if it has an Outline component
    public static void SetOutlineColor(GameObject target, Color color)
    {
        if (target == null) return;

        Outline outline = target.GetComponent<Outline>();
        if (outline != null)
        {
            outline.OutlineColor = color;
        }
    }

    // Optional overload: Set color using Renderer or Collider etc.
    public static void SetOutlineColor(Component target, Color color)
    {
        if (target == null) return;

        Outline outline = target.GetComponent<Outline>();
        if (outline != null)
        {
            outline.OutlineColor = color;
        }
    }
}