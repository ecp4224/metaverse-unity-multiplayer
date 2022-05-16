using UnityEngine;

namespace Epibyte.ConceptVR
{
    public class TabPanelManager : MonoBehaviour
    {
        public Transform tabs;
        public TabButton initialActivatedTab;
        [HideInInspector]
        public TabButton currentSelectedTab;
        public bool autoEnableTriggerOnChildrenCollider = true;
        TabButton[] tabButtons;

        void Awake()
        {
            tabButtons = tabs.GetComponentsInChildren<TabButton>();
        }

        void Start()
        {

            foreach (var item in tabButtons)
            {
                item.OnTabSelected += SelectTab;
            }

            if (null != initialActivatedTab)
            {
                initialActivatedTab.OnClicked();
                currentSelectedTab = initialActivatedTab;
            }

            if (autoEnableTriggerOnChildrenCollider)
            {
                Collider[] colliders = GetComponentsInChildren<Collider>(true);
                foreach (Collider collider in colliders)
                {
                    collider.isTrigger = true;
                }
            }
        }

        public void SelectTab(TabButton tab)
        {
            DeselectCurrentItem();
            tab.Activate();
            tab.isClicked = true;
            currentSelectedTab = tab;
        }

        public void DeselectCurrentItem()
        {
            if (null != currentSelectedTab)
            {
                currentSelectedTab.isClicked = false;
                currentSelectedTab.OnReleased();
            }
            currentSelectedTab = null;
        }
    }
}
