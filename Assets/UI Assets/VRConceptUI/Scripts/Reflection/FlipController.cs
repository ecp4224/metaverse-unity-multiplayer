using UnityEngine;

namespace Epibyte.ConceptVR
{

    public class FlipController : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 initPosition;
        public Vector3 flippedPosition;

        void Awake()
        {
            initPosition = transform.localPosition;
        }

        public void SetToInit()
        {
            transform.localPosition = initPosition;
        }

        public void SetToFlipped()
        {
            transform.localPosition = flippedPosition;
        }
    }
}