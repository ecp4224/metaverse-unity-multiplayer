using System;
using Nethereum.Web3;
using UnityEngine;
using WalletConnectSharp.NEthereum;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Unity.Utils;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition.Providers
{
    [RequireComponent(typeof(WalletConnect))]
    public class WalletConnectWeb3Provider : Wallet
    {
        [BindComponent]
        private WalletConnect _walletConnect;
        
        public override bool Connected
        {
            get
            {
                return _walletConnect.Connected && Web3 != null;
            }
        }

        public override string Address
        {
            get
            {
                if (!_walletConnect.Connected || _walletConnect.Session.Accounts.Length == 0)
                    return "";
            
                return _walletConnect.Session.Accounts[0];
            }
        }

        public override int ChainId
        {
            get
            {
                return _walletConnect.Session.ChainId;
            }
        }
        
        protected override void OnProviderReady(Action<Web3> callback)
        {
            _walletConnect.ConnectedEvent.AddListener(delegate
            {
                var web3 = _walletConnect.Session.BuildWeb3(infuraId, network).AsWalletAccount();

                callback(web3);
            });
        }
        
    }
}