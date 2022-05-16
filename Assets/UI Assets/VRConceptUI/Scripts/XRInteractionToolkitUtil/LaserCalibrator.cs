using UnityEngine;
#if VRCONCEPT_XR_INTERACTION
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace Epibyte.ConceptVR
{
    // LaserCalibrator will make sure the XR Laser pointer is at the same length with the concept ui laser pointer.
    // It will also make sure the raycast trigger interaction is the Collide type;
    public class LaserCalibrator : MonoBehaviour
    {
#if VRCONCEPT_XR_INTERACTION
        public XRRayInteractor xRRayInteractor;
        public LaserPointer conceptUILaserPointer;

        float originXRMaxRaycastDistance;
        QueryTriggerInteraction originXRRaycastTriggerInteraction;

        void Awake()
        {
            if (null != xRRayInteractor)
            {
                originXRMaxRaycastDistance = xRRayInteractor.maxRaycastDistance;
                originXRRaycastTriggerInteraction = xRRayInteractor.raycastTriggerInteraction;
            }
        }

        void OnEnable()
        {
            Calibrate();
        }

        void OnDisable()
        {
            Reset();
        }

        public void Calibrate()
        {
            if (null != xRRayInteractor)
            {
                xRRayInteractor.maxRaycastDistance = conceptUILaserPointer.length;
                xRRayInteractor.raycastTriggerInteraction = QueryTriggerInteraction.Collide;
            }
        }

        public void Reset()
        {
            if (null != xRRayInteractor)
            {
                xRRayInteractor.maxRaycastDistance = originXRMaxRaycastDistance;
                xRRayInteractor.raycastTriggerInteraction = originXRRaycastTriggerInteraction;
            }
        }
#endif
    }
}
