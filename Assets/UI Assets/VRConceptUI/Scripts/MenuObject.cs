using UnityEngine;

namespace Epibyte.ConceptVR
{
    [RequireComponent(typeof(Interactable))]
    public class MenuObject : MonoBehaviour
    {
        public GameObject relatedObject;

        void Start()
        {
            if (null != GetComponent<Interactable>())
            {
                GetComponent<Interactable>().onClickedEvent.AddListener(GenerateGO);
            }
        }

        void OnDestroy()
        {
            if (null != GetComponent<Interactable>())
            {
                GetComponent<Interactable>().onClickedEvent.RemoveListener(GenerateGO);
            }
        }

        public void GenerateGO()
        {
            GameObject go = Instantiate(relatedObject, LaserPointer.instance.pointer.position, Quaternion.identity);
            if (null != go.GetComponent<IInteractable>())
            {
                LaserPointer.instance.Target = go.GetComponent<IInteractable>();
                go.GetComponent<IInteractable>().OnClicked();
            }
        }
    }
}