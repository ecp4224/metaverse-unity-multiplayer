using System;
using UnityEngine;

public class CanvasLinker : BindableMonoBehavior
{
    [BindComponent] private Canvas _canvas;

    private void Start()
    {
        _canvas.worldCamera = Camera.current;
    }
}