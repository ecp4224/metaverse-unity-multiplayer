using System;
using Nethereum.Web3;
using Sirenix.OdinInspector;
using UnityTemplateProjects.Profile;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition.Providers
{
    public class PrivateKeyStoreWeb3Provider : IProvider
    {
        public bool createNewWalletOnStart = true;

        public async void Start(Wallet wallet)
        {
            if (createNewWalletOnStart && ProfileManager.Instance.AllLocalUsernames.Count == 0)
            {
                string username = "--AUTO--" + Guid.NewGuid().ToString();

                await ProfileManager.Instance.CreateProfile(username, "", ChainId);
            }
        }

        public bool Connected
        {
            get
            {
                return ProfileManager.Instance.CurrentProfile != null;
            }
        }

        public string Address
        {
            get
            {
                return ProfileManager.Instance.CurrentProfile.AccountData.Address;
            }
        }

        public int ChainId
        {
            get
            {
                return Wallet.ChainNameToId[Wallet.Current.network];
            }
        }

        public void OnProviderReady(Action<Web3> callback)
        {
            var pm = ProfileManager.Instance;
            
            pm.Load();

            if (pm.CurrentProfile != null)
            {
                callback(pm.CurrentProfile.Connect());
            }
            
            pm.CurrentProfileUpdated += delegate(object sender, Profile profile)
            {
                callback(profile.Connect());
            };

            if (pm.AllLocalUsernames.Count > 0)
            {
                //Check if any of them have --AUTO--
                foreach (var username in pm.AllLocalUsernames)
                {
                    if (username.StartsWith("--AUTO--"))
                    {
                        //Attempt to login now
                        //No need to await the task, the proper callback will be
                        //triggered above
                        pm.UnlockAndUse(username, "", ChainId);
                        break;
                    }
                }
            }
        }
    }
}