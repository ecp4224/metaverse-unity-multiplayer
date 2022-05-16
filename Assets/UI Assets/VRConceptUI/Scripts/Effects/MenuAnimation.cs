using UnityEngine;
using System.Collections;

namespace Epibyte.ConceptVR
{
    public class MenuAnimation : MonoBehaviour
    {
        public float duration = 0.3f;
        bool isScaling = false;

        void OnEnable()
        {
            if (isScaling)
            {
                return;
            }
            transform.localScale = Vector3.zero;
            StartCoroutine(ScaleTo(Vector3.one));
        }

        IEnumerator ScaleTo(Vector3 toValue)
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
            transform.localScale = toValue;
            isScaling = false;
        }
    }
}