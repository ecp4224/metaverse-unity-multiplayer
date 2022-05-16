using UnityEngine;
#if VRCONCEPT_XR_INTERACTION
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace Epibyte.ConceptVR
{
    public class JoystickController : MonoBehaviour
    {
#if VRCONCEPT_OCULUS
        public OVRInput.Controller controller;
#endif

#if VRCONCEPT_XR_INTERACTION
        public XRController controller;
#endif

        public ThumbstickStatus thumbstickStatus;
        public delegate void OnJoystickMoveHandler(ThumbstickStatus status);

        // The event which other objects can subscribe to
        // Uses the function defined above as its type
        public event OnJoystickMoveHandler OnJoystickMove;

        void LateUpdate()
        {
#if VRCONCEPT_OCULUS
            ThumbstickProgress(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller));
#endif

#if VRCONCEPT_XR_INTERACTION
            controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axisValue);
            ThumbstickProgress(axisValue);
#endif

        }

        void ThumbstickProgress(Vector2 v)
        {
            ThumbstickStatus ret = ThumbstickStatus.Stayput;

            if (0.5 < v.x)
            {
                ret = ThumbstickStatus.Right;
            }

            if (-0.5 > v.x)
            {
                ret = ThumbstickStatus.Left;
            }

            if (0.5 < v.y)
            {
                ret = ThumbstickStatus.Up;
            }

            if (-0.5 > v.y)
            {
                ret = ThumbstickStatus.Down;
            }

            thumbstickStatus = ret;

            if (null != OnJoystickMove)
            {
                OnJoystickMove(thumbstickStatus);
            }
        }
    }

    public enum ThumbstickStatus
    {
        Stayput,
        Left,
        Right,
        Up,
        Down
    }
}
