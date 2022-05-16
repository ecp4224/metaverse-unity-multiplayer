using System;
using Nethereum.Web3;
using Sirenix.OdinInspector;
using UnityTemplateProjects.Profile;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition.Providers
{
    public class PrivateKeyStoreWeb3Provider : Wallet
    {
        [InfoBox("Create a new Wallet on Start if no previous Wallet was found.")]
        public bool createNewWalletOnStart = true;

        private async void Start()
        {
            //TODO Get username from somewhere

            if (createNewWalletOnStart && ProfileManager.Instance.AllLocalUsernames.Count == 0)
            {
                string username = "--AUTO--" + Guid.NewGuid().ToString();

                await ProfileManager.Instance.CreateProfile(username, "", ChainId);
            }
        }

        public override bool Connected
        {
            get
            {
                return ProfileManager.Instance.CurrentProfile != null && Web3 != null;
            }
        }

        public override string Address
        {
            get
            {
                return ProfileManager.Instance.CurrentProfile.AccountData.Address;
            }
        }

        public override int ChainId
        {
            get
            {
                return ChainNameToId[network];
            }
        }

        protected override void OnProviderReady(Action<Web3> callback)
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