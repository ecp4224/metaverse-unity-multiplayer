using System;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition.UI
{
    [Serializable]
    public class GameObjectNamed : SerializableDictionaryBase<string, GameObject>
    {
    }

    public class WalletUI : BindableMonoBehavior
    {
        [BindComponentInParent]
        private Player _player;

        [BindComponent]
        private GraphicRaycaster _graphicRaycaster;

        public GameObject backCanvas;
        
        public Button sendEthButton;
        public Button buyButton;
        public Button dropNFT;
        public Button pickupNFT;

        public Transform nftView;
        public GameObject nftButton;

        public string defaultPage;
        public GameObject loadingScreen;
        public GameObjectNamed screens = new GameObjectNamed();

        [NonSerialized]
        public GameObject currentScreen;

        public Action confirmAction;

        public void UpdateNFTView()
        {
            if (!nftView.gameObject.activeSelf)
                return;

            foreach (Transform child in nftView)
            {
                if (child == nftView)
                    continue;
                
                Destroy(child.gameObject);
            }
            
            foreach (var nftAddress in Wallet.Current.NFTS.Keys)
            {
                var nftList = Wallet.Current.NFTS[nftAddress];

                foreach (var nft in nftList)
                {
                    string addr = nftAddress;
                    
                    var btnObj = Instantiate(nftButton, nftView);

                    var btn = btnObj.GetComponentInChildren<Button>();
                    var img = btnObj.GetComponentInChildren<Image>();

                    img.sprite = nft.imageSprite;
                    
                    btn.onClick.AddListener(delegate
                    {
                        Player.LocalPlayer.SendNFT(nft, addr);
                    });
                }
            }
        }

        private void OnEnable()
        {
            backCanvas.SetActive(true);
        }

        private void OnDisable()
        {
            backCanvas.SetActive(false);
        }

        public void ShowLoader()
        {
            currentScreen.SetActive(false);
            loadingScreen.SetActive(true);
        }

        public void HideLoader()
        {
            currentScreen.SetActive(true);
            loadingScreen.SetActive(false);
        }

        public void ShowScreen(string name)
        {
            if (currentScreen != null)
            {
                currentScreen.SetActive(false);
            }
            
            currentScreen = screens[name];
            currentScreen.SetActive(true);
        }

        private void Start()
        {
            ShowScreen(defaultPage);

            _graphicRaycaster.enabled = _player.isLocalPlayer;
        }

        public void CopyAddress()
        {
            GUIUtility.systemCopyBuffer = Player.LocalPlayer.PlayerAddress;
        }

        public void CancelConfirmation()
        {
            ShowScreen(defaultPage);
        }

        public void ShowConfirm(Action action)
        {
            confirmAction = action;
            ShowScreen("confirm");
        }

        public void ClickConfirm()
        {
            if (confirmAction != null)
                confirmAction();
        }
    }
}