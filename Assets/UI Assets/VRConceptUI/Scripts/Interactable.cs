using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Epibyte.ConceptVR
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public UnityEvent onHoveredEvent;
        public UnityEvent onClickedEvent;
        public UnityEvent onReleasedEvent;
        public UnityEvent onLeaveEvent;

        public void OnHovered()
        {
            onHoveredEvent.Invoke();
        }

        public void OnClicked()
        {
            onClickedEvent.Invoke();
        }

        public void OnReleased()
        {
            onReleasedEvent.Invoke();
        }

        public void OnLeave()
        {
            onLeaveEvent.Invoke();
        }
    }
}