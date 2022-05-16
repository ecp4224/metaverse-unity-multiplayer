using System;
using Nethereum.Web3;
using UnityEngine;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.NEthereum;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Unity.Utils;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition.Providers
{
    public class WalletConnectWeb3Provider : IProvider
    {
        private WalletConnect _walletConnect;
        private ClientMeta meta;
        
        public WalletConnectWeb3Provider(ClientMeta meta)
        {
            this.meta = meta;
        }

        public WalletConnect WalletConnect
        {
            get
            {
                return _walletConnect;
            }
        }

        public void Start(Wallet wallet)
        {
            _walletConnect = wallet.gameObject.AddComponent<WalletConnect>();
            _walletConnect.AppData = meta;
            _walletConnect.ConnectedEvent = new WalletConnect.WalletConnectEventNoSession();
        }
        
        public bool Connected
        {
            get
            {
                return _walletConnect.Connected;
            }
        }

        public string Address
        {
            get
            {
                if (!_walletConnect.Connected || _walletConnect.Session.Accounts.Length == 0)
                    return "";
            
                return _walletConnect.Session.Accounts[0];
            }
        }

        public int ChainId
        {
            get
            {
                return _walletConnect.Session.ChainId;
            }
        }
        
        public void OnProviderReady(Action<Web3> callback)
        {
            _walletConnect.ConnectedEvent.AddListener(delegate
            {
                var web3 = _walletConnect.Session.BuildWeb3(Wallet.Current.infuraId, Wallet.Current.network).AsWalletAccount();

                callback(web3);
            });
        }
        
    }
}