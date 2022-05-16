using System;
using UnityEngine;
using UnityEngine.Events;

namespace Epibyte.ConceptVR
{
    [Serializable]
    public struct ScaleAxis
    {
        public bool scaleX;
        public bool scaleY;
        public bool scaleZ;
    }

    public class TransitionEffect : MonoBehaviour
    {
        public Material activatedMaterial;
        [Header("Scale")]
        public float activatedScaleFactor = 1f;
        public ScaleAxis scaleAxis;
        protected MeshRenderer mesh;
        protected Material idleMaterial;
        protected Vector3 idleScale;

        public virtual void Awake()
        {
            mesh = GetComponent<MeshRenderer>();
            idleMaterial = mesh.material;
            idleScale = transform.localScale;
        }

        public void ActivateMaterial()
        {
            if (null != activatedMaterial)
            {
                mesh.material = activatedMaterial;
            }
        }

        public void ActivateScale()
        {
            if (1f != activatedScaleFactor)
            {
                float _x, _y, _z;

                if (scaleAxis.scaleX)
                {
                    _x = idleScale.x * activatedScaleFactor;
                }
                else
                {
                    _x = idleScale.x;
                }

                if (scaleAxis.scaleY)
                {
                    _y = idleScale.y * activatedScaleFactor;
                }
                else
                {
                    _y = idleScale.y;
                }

                if (scaleAxis.scaleZ)
                {
                    _z = idleScale.z * activatedScaleFactor;
                }
                else
                {
                    _z = idleScale.z;
                }

                transform.localScale = new Vector3(_x, _y, _z);
            }
        }

        public void DeactivateMaterial()
        {
            if (null != activatedMaterial)
            {
                mesh.material = idleMaterial;
            }
        }

        public void DeactivateScale()
        {
            if (1f != activatedScaleFactor)
            {
                transform.localScale = idleScale;
            }
        }

        public void ActivateAllEffects()
        {
            ActivateMaterial();
            ActivateScale();
        }

        public void DeactivateAllEffects()
        {
            DeactivateMaterial();
            DeactivateScale();
        }
    }
}
