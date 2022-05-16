using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition.Providers;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Core.Models;

public class WalletChooseScreen : MonoBehaviour
{
    public Button createWalletButton;

    public GameObject connectScreen;
    public GameObject walletConnectScreen;
    public Wallet web3Obj;

    public ClientMeta walletConnectClientData;

    public Text walletConnectSubText;

    // Start is called before the first frame update
    void Start()
    {
        if (ProfileManager.Instance.AllLocalUsernames.Count > 0 && HasAutoWallet())
        {
            createWalletButton.GetComponentInChildren<Text>().text = "Load Wallet";
        }
    }

    private bool HasAutoWallet()
    {
        return ProfileManager.Instance.AllLocalUsernames.Any(username => username.StartsWith("--AUTO--"));
    }

    public void CreateOrLoadWallet()
    {
        web3Obj.SetupListeners(new PrivateKeyStoreWeb3Provider());
        
        gameObject.SetActive(false);
        connectScreen.SetActive(true);
    }

    public void ConnectWallet()
    {
        var wcprovider = new WalletConnectWeb3Provider(walletConnectClientData);
        web3Obj.SetupListeners(wcprovider);

        walletConnectSubText.text =
            "Scan the QR code with your MetaMask mobile app to connect.\nPlease make sure you are connected to the " +
            Wallet.Current.network + " network!";

        wcprovider.WalletConnect.ConnectedEvent.AddListener(() =>
        {
            walletConnectScreen.SetActive(false);
            connectScreen.SetActive(true);
        });
        
        gameObject.SetActive(false);
        walletConnectScreen.SetActive(true);
    }
}
