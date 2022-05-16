using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epibyte.ConceptVR
{
    public class TabButton : TransitionEffect
    {
        public delegate void OnTabSelectedHandler(TabButton tab);
        public event OnTabSelectedHandler OnTabSelected;
        public GameObject relatedPanel;
        [HideInInspector]
        public bool isClicked = false;

        public override void Awake()
        {
            base.Awake();
            if (null != relatedPanel)
            {
                relatedPanel.SetActive(false);
            }
        }

        public void OnHovered()
        {
            ActivateMaterial();
        }

        public void OnClicked()
        {
            if (null != OnTabSelected)
            {
                OnTabSelected(this);
            }
        }

        public void OnReleased()
        {
            if (!isClicked)
            {
                Deactivate();
            }
        }

        public void OnLeave()
        {
            if (!isClicked)
            {
                DeactivateMaterial();
            }
        }

        public void Activate()
        {
            ActivateAllEffects();
            if (null != relatedPanel)
            {
                relatedPanel.SetActive(true);
            }
        }

        public void Deactivate()
        {
            DeactivateAllEffects();
            if (null != relatedPanel)
            {
                relatedPanel.SetActive(false);
            }
        }
    }
}