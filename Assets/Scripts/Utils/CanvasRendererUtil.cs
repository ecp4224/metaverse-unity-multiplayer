
using UnityEngine;

public static class CanvasRendererUtil
{
    /// <summary>
    /// Set the alpha of this CanvasRenderer and for all child canvas renderers
    /// </summary>
    /// <param name="renderer">The parent renderer to set the alpha</param>
    /// <param name="alpha">The new alpha</param>
    public static void SetAlphaAndChildren(this CanvasRenderer renderer, float alpha)
    {
        renderer.SetAlpha(alpha);

        var children = renderer.GetComponentsInChildren<CanvasRenderer>();

        foreach (var child in children)
        {
            if (child == renderer)
                continue;
            
            child.SetAlphaAndChildren(alpha);
        }
    }
}