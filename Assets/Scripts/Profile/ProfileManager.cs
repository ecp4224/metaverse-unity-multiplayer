using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.KeyStore;
using Nethereum.KeyStore.Model;
using Newtonsoft.Json;
using PlayerDirectory;
using UnityEngine;
using UnityTemplateProjects.Profile;

public class ProfileManager : Singleton<ProfileManager>
{
    private const string PLAYER_DIRECTORY_ADDRESS = "0x300AbC76A75bC45eC0009Fc246f89a809b265B83";
    
    private const string PROFILE_PLAYERPREF_KEY = "__PROFILE__";
    public const string GUEST_PROFILE = "__GUEST__";
    
    private Dictionary<string, string> usernameToJson = new Dictionary<string, string>();

    private KeyStoreScryptService keyStoreService;

    private Profile currentProfile;
    private Profile guest;

    public EventHandler<Profile> CurrentProfileUpdated;



    public async Task<Profile> CreateProfile(string username, string password, int chainId)
    {
        if (usernameToJson.ContainsKey(username))
        {
            //TODO Show error
            throw new Exception("Username already taken");
        }
        
        var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
        
        var scryptParams = new ScryptParams {Dklen = 32, N = 262144, R = 1, P = 8};

        TaskCompletionSource<string> task = new TaskCompletionSource<string>(TaskCreationOptions.None);
        
        new Thread(new ThreadStart(delegate
        {
            try
            {
                var keyStore = keyStoreService.EncryptAndGenerateKeyStore(password, ecKey.GetPrivateKeyAsBytes(),
                    ecKey.GetPublicAddress(), scryptParams);
                var json = keyStoreService.SerializeKeyStoreToJson(keyStore);

                task.SetResult(json);
            }
            catch (Exception e)
            {
                task.SetException(e);
            }
        })).Start();

        var json = await task.Task;
        
        usernameToJson.Add(username, json);
        
        Save();

        return await UnlockAndUse(username, password, chainId);
    }

    public async Task<Profile> UnlockAndUse(string username, string password, int chainId)
    {
        if (currentProfile != null)
        {
            currentProfile.Lock();
        }

        var profile = ProfileFromUsername(username);
        if (profile == null)
        {
            throw new IOException("Profile not found");
        }
        
        currentProfile = await profile.Unlock(chainId, password);

        currentProfile.Connect();

        if (CurrentProfileUpdated != null)
        {
            CurrentProfileUpdated(this, currentProfile);
        }

        return profile;
    }

    public Task<KeyStore<ScryptParams>> Deserialize(string json)
    {
        TaskCompletionSource<KeyStore<ScryptParams>> task = new TaskCompletionSource<KeyStore<ScryptParams>>(TaskCreationOptions.None);
        
        new Thread(new ThreadStart(delegate
        {
            try
            {
                var keyStore = keyStoreService.DeserializeKeyStoreFromJson(json);

                task.SetResult(keyStore);
            }
            catch (Exception e)
            {
                task.SetException(e);
            }
        })).Start();
        
        return task.Task;
    }

    public Task<byte[]> DeserializeAndDecrypt(string json, string password)
    {
        TaskCompletionSource<byte[]> task = new TaskCompletionSource<byte[]>(TaskCreationOptions.None);
        
        new Thread(new ThreadStart(delegate
        {
            try
            {
                var keyStore = keyStoreService.DeserializeKeyStoreFromJson(json);
                var result = keyStoreService.DecryptKeyStore(password, keyStore);

                task.SetResult(result);
            }
            catch (Exception e)
            {
                task.SetException(e);
            }
        })).Start();
        
        return task.Task;
    }

    public Profile ProfileFromUsername(string username)
    {
        if (!usernameToJson.ContainsKey(username))
            return null;
        
        return new Profile(username, usernameToJson[username]);
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(usernameToJson);

        if (!string.IsNullOrEmpty(json))
        {
            PlayerPrefs.SetString(PROFILE_PLAYERPREF_KEY, json);
        }
    }

    public void Load()
    {
        var json = PlayerPrefs.GetString(PROFILE_PLAYERPREF_KEY);

        usernameToJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        if (usernameToJson == null)
        {
            usernameToJson = new Dictionary<string, string>();
        }
    }

    public ReadOnlyCollection<string> AllLocalUsernames
    {
        get
        {
            return usernameToJson.Keys.ToList().AsReadOnly();
        }
    }

    public ReadOnlyCollection<Profile> AllLocalProfiles
    {
        get
        {
            return AllLocalUsernames.Select(ProfileFromUsername).ToList().AsReadOnly();
        }
    }

    public Profile GuestProfile
    {
        get
        {
            if (guest == null)
                guest = ProfileFromUsername(GUEST_PROFILE);
            return guest;
        }
    }

    public Profile CurrentProfile
    {
        get
        {
            return currentProfile;
        }
    }

    private async void Awake()
    {
        keyStoreService = new KeyStoreScryptService();
        
        Load();

        if (usernameToJson.Count == 0)
        {
            await CreateProfile(GUEST_PROFILE, "", Wallet.Current.ChainId);
        }
        else
        {
            await GuestProfile.Unlock(Wallet.Current.ChainId);
        }
    }

    public async Task<bool> DoesUsernameExists(string username)
    {
        var web3 = GuestProfile.Connect();

        var contractHandler = web3.Eth.GetContractHandler(PLAYER_DIRECTORY_ADDRESS);
        
        var usernameTakenFunction = new UsernameTakenFunction();
        usernameTakenFunction.ReturnValue1 = username;
        return await contractHandler.QueryAsync<UsernameTakenFunction, bool>(usernameTakenFunction);
    }
}
