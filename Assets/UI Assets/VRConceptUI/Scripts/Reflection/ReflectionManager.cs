using UnityEngine;
using UnityEngine.Events;
#if VRCONCEPT_XR_INTERACTION
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
#endif
using System.Collections;

namespace Epibyte.ConceptVR
{
    public class ReflectionManager : MonoBehaviour
    {
#if VRCONCEPT_OCULUS
        public OVRInput.Controller controller;
        public OVRInput.Button flipButton = OVRInput.Button.One;
#endif

#if VRCONCEPT_XR_INTERACTION
        public XRController controller;
        public InputHelpers.Button flipButton;
        bool isFlipPreviousPressed = false;
#endif

        public Camera reflectionCamera;
        public Transform reflection;
        public AspectRatio aspectRatio;
        Vector2 zoomDistanceFrontCamera = new Vector2(-1, -0.1f);
        Vector2 zoomDistanceBackCamera = new Vector2(0.5f, -0.3f);
        JoystickController joystickController;
        CameraDirection cameraDirection = CameraDirection.Front;
        bool isRotating = false;

        void Awake()
        {
            joystickController = GetComponent<JoystickController>();

        }

        void Start()
        {
            if (null != joystickController)
            {
                joystickController.OnJoystickMove += OperateTarget;
            }
        }


        void LateUpdate()
        {
#if VRCONCEPT_OCULUS
            if (OVRInput.GetDown(flipButton, controller)) {
                Flip();
            }
#endif

#if VRCONCEPT_XR_INTERACTION
            if (controller.inputDevice.IsPressed(flipButton, out bool isPressed))
            {
                if (isFlipPreviousPressed != isPressed)
                {
                    isFlipPreviousPressed = isPressed;

                    if (isPressed)
                    {
                        Flip();
                    }
                }
            }
#endif
        }

        void Flip()
        {
            if (isRotating)
            {
                return;
            }

            if (cameraDirection == CameraDirection.Back)
            {
                reflectionCamera.GetComponent<FlipController>().SetToInit();
                reflection.GetComponent<FlipController>().SetToInit();

                if (aspectRatio == AspectRatio.Horizontal)
                {
                    StartCoroutine(RotateTo(Vector3.right, -180f, 0.5f));
                }
                else
                {
                    StartCoroutine(RotateTo(Vector3.forward, -180f, 0.5f));
                }

                cameraDirection = CameraDirection.Front;
            }
            else
            {
                reflectionCamera.GetComponent<FlipController>().SetToFlipped();
                reflection.GetComponent<FlipController>().SetToFlipped();
                if (aspectRatio == AspectRatio.Horizontal)
                {
                    StartCoroutine(RotateTo(Vector3.right, 180f, 0.5f));
                }
                else
                {
                    StartCoroutine(RotateTo(Vector3.forward, 180f, 0.5f));
                }

                cameraDirection = CameraDirection.Back;
            }
            reflection.localScale = new Vector3(reflection.localScale.x, reflection.localScale.y, reflection.localScale.z * -1);
        }


        void Zoom(int direction, float limit)
        {
            if (direction == 1)
            {
                if (reflectionCamera.transform.localPosition.y >= limit)
                {
                    return;
                }
            }
            else
            {
                if (reflectionCamera.transform.localPosition.y <= limit)
                {
                    return;
                }
            }

            reflectionCamera.transform.localPosition += Vector3.up * 0.05f * direction;
        }

        void AspectHorizontal()
        {
            if (isRotating)
            {
                return;
            }
            if (aspectRatio == AspectRatio.Vertical)
            {
                StartCoroutine(RotateTo(Vector3.up, -90f, 0.25f));
                aspectRatio = AspectRatio.Horizontal;
            }
        }

        void AspectVertical()
        {
            if (isRotating)
            {
                return;
            }
            if (aspectRatio == AspectRatio.Horizontal)
            {
                StartCoroutine(RotateTo(Vector3.up, 90f, 0.25f));
                aspectRatio = AspectRatio.Vertical;
            }
        }

        void OperateTarget(ThumbstickStatus status)
        {

            switch (status)
            {
                case ThumbstickStatus.Down:
                    if (cameraDirection == CameraDirection.Back)
                    {
                        Zoom(-1, zoomDistanceBackCamera.y);
                    }
                    else
                    {
                        Zoom(1, zoomDistanceFrontCamera.y);
                    }
                    break;
                case ThumbstickStatus.Up:
                    if (cameraDirection == CameraDirection.Back)
                    {
                        Zoom(1, zoomDistanceBackCamera.x);
                    }
                    else
                    {
                        Zoom(-1, zoomDistanceFrontCamera.x);
                    }
                    break;
                case ThumbstickStatus.Left:
                    AspectHorizontal();
                    break;
                case ThumbstickStatus.Right:
                    AspectVertical();
                    break;
                case ThumbstickStatus.Stayput:
                    break;
            }
        }

        IEnumerator RotateTo(Vector3 axis, float toValue, float duration)
        {
            // Coroutine starts running.
            isRotating = true;
            float elapsedTime = 0f;
            Quaternion from, to;
            from = to = transform.localRotation;
            to *= Quaternion.Euler(axis * toValue);

            while (isRotating && elapsedTime < duration)
            {
                transform.localRotation = Quaternion.Slerp(from, to, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.localRotation = to;
            // Coroutine finishes running.
            isRotating = false;
        }

        public enum AspectRatio { Vertical, Horizontal }
        public enum CameraDirection { Front, Back }
    }
}
