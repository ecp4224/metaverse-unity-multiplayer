using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epibyte.ConceptVR
{
    public interface IInteractable
    {
        void OnHovered();
        void OnClicked();
        void OnReleased();
        void OnLeave();
    }
}
