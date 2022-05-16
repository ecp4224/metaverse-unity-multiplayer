using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using UnityEngine;

namespace UnityTemplateProjects.Profile
{
    public class Profile
    {
        public class RegisterData
        {
            public string username;
            public string address;
        }

        public class RegisterResponse
        {
            public bool error;
            public string message;
        }
        
        public string Username { get; private set; }
        private string json;
        private Account account;

        public Account AccountData
        {
            get
            {
                return account;
            }
        }

        public bool Unlocked
        {
            get
            {
                return account != null;
            }
        }

        public Profile(string username, string json)
        {
            this.Username = username;
            this.json = json;
        }

        [NotNull]
        public async Task<Profile> Unlock(int chainId, string password = "")
        {
            if (this.account != null)
                return this;

            this.account = new Account(await ProfileManager.Instance.DeserializeAndDecrypt(json, password), chainId);

            return this;
        }

        public Web3 Connect()
        {                
            var infuraURL = Wallet.Current.InfuraURL;
            if (this.Username == ProfileManager.Instance.GuestProfile.Username)
            {
                //Guest profile doesn't need an account
                return new Web3(infuraURL);
            }
            if (this.account == null)
            {
                throw new IOException("Profile not unlocked");
            }

            return new Web3(this.account, infuraURL);
        }

        public void Lock()
        {
            account = null;
        }

        public async Task<bool> Register()
        {
            if (this.account == null)
                throw new IOException("Profile not unlocked");
            
            if (await ProfileManager.Instance.DoesUsernameExists(Username))
            {
                throw new IOException("Username already registered");
            }

            var webClient = new WebClient();
            string url = "https://api.decent.edkek.com/register";

            var data = new RegisterData()
            {
                address = this.account.Address,
                username = Username
            };

            var rawData = JsonConvert.SerializeObject(data);

            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            var response = await webClient.UploadStringTaskAsync(new Uri(url), "POST", rawData);

            var responseData = JsonConvert.DeserializeObject<RegisterResponse>(response);

            return !responseData.error;
        }

        public async Task<decimal> FetchCurrentBalance()
        {
            var readonlyWeb3 = ProfileManager.Instance.GuestProfile.Connect();

            var keyStore = await ProfileManager.Instance.Deserialize(json);

            var rawBalance = await readonlyWeb3.Eth.GetBalance.SendRequestAsync(keyStore.Address);

            return Web3.Convert.FromWei(rawBalance);
        }
    }
}