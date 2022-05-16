using System.Collections;
using UnityEngine;

namespace Epibyte.ConceptVR
{
    public class Grabble : MonoBehaviour, IInteractable
    {
        #region Public Variables
        [Header("Rotation")]
        public bool isRotatable = true;
        public float rotationSpeed = 100f;
        [Header("Scale")]
        public bool isScalable = true;
        public float scaleStep = 0.5f;
        public float minScaleFactor = 0.5f;
        public float maxScaleFactor = 2f;
        #endregion

        #region Private Variables
        Vector3 originSize, minSize, maxSize;

        bool isGrabbing = true;
        bool isRotating = false;
        bool isScaling = false;

        Rigidbody rb;
        bool isKinematic;
        CollisionDetectionMode collisionDetectionMode;
        #endregion

        #region Monobehaviour Callbacks
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (null != rb)
            {
                isKinematic = rb.isKinematic;
                collisionDetectionMode = rb.collisionDetectionMode;
            }
        }

        void Start()
        {
            // originSize = transform.localScale;
            minSize = transform.localScale * minScaleFactor;
            maxSize = transform.localScale * maxScaleFactor;
        }

        void LateUpdate()
        {
            if (isGrabbing)
            {
                transform.position = LaserPointer.instance.pointer.position;
            }
        }
        #endregion

        #region IInteractable Implementation
        public void OnClicked()
        {
            isGrabbing = true;
            EnableKinematic();
        }

        public void OnReleased()
        {
            StopAll();
            isGrabbing = false;
            ResetRigidbody();
        }

        public void OnHovered() { }
        public void OnLeave() { }
        #endregion

        public void Rotate(int direction)
        {
            StopScaling();

            if (!isRotatable)
            {
                return;
            }

            if (isRotating)
            {
                return;
            }

            StartCoroutine(RotateTo(direction * rotationSpeed, 1f));
        }

        public void StopRotating()
        {
            isRotating = false;
        }


        public void Scale(int direction)
        {
            StopRotating();

            if (!isScalable)
            {
                return;
            }

            if (isScaling)
            {
                return;
            }

            Vector3 toValue;
            if (1 == direction)
            {
                toValue = transform.localScale + (transform.localScale * scaleStep);
            }
            else if (-1 == direction)
            {
                toValue = transform.localScale - (transform.localScale * scaleStep);
            }
            else
            {
                return;
            }

            toValue = new Vector3(
                Mathf.Clamp(toValue.x, minSize.x, maxSize.x),
                Mathf.Clamp(toValue.y, minSize.y, maxSize.y),
                Mathf.Clamp(toValue.z, minSize.z, maxSize.z)
            );

            StartCoroutine(ScaleTo(toValue, 0.5f));
        }

        public void StopScaling()
        {
            isScaling = false;
        }

        public void StopAll()
        {
            StopRotating();
            StopScaling();
        }

        IEnumerator RotateTo(float toValue, float duration)
        {
            // Coroutine starts running.
            isRotating = true;
            float elapsedTime = 0f;

            while (isRotating && elapsedTime < duration)
            {
                transform.Rotate(Vector3.up * toValue * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            // Coroutine finishes running.
            isRotating = false;
        }

        IEnumerator ScaleTo(Vector3 toValue, float duration)
        {
            // Coroutine starts running.
            isScaling = true;
            Vector3 from = transform.localScale;

            float elapsedTime = 0f;

            while (isScaling && elapsedTime < duration)
            {
                transform.localScale = Vector3.Lerp(from, toValue, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Coroutine finishes running.
            isScaling = false;
        }

        void ResetRigidbody()
        {
            if (null != rb)
            {
                rb.isKinematic = isKinematic;
                rb.collisionDetectionMode = collisionDetectionMode;
            }
        }

        void EnableKinematic()
        {
            if (null != rb)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.isKinematic = true;
            }
        }
    }
}
