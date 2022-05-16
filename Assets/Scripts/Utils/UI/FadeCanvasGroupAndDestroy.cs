using System;
using UnityEngine;

public class FadeCanvasGroupAndDestroy : BindableNetworkBehavior
{
    public float duration;

    public event EventHandler FadedAndDestroyed;

    [BindComponent]
    private CanvasGroup _group;

    private float startTime;

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (_group == null)
        {
            Destroy(gameObject); //Destroy ourselves if we can't fade away
            return;
        }

        float alpha = Mathf.Lerp(1f, 0f, (Time.time - startTime) / duration);

        _group.alpha = alpha;

        if (alpha == 0f)
        {
            if (FadedAndDestroyed != null)
            {
                FadedAndDestroyed(gameObject, EventArgs.Empty);
            }
            Destroy(gameObject);
        }
    }
}