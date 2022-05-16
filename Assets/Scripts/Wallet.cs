using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DefaultNamespace;
using ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition;
using Nethereum.Contracts;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Newtonsoft.Json;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.Networking;
using WalletConnectSharp.NEthereum;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Unity.Models;
using WalletConnectSharp.Unity.Utils;

[Serializable]
public class NFTList : List<NFTTokenData>
{
    public NFTList() : base()
    {
    }

    public NFTList(NFTTokenData[] og) : base(og)
    {
    }
}

[Serializable]
public class NFTDictionary : SerializableDictionaryBase<string, NFTList>
{
    public NFTDictionary() : base()
    {
    }

    public NFTDictionary(Dictionary<string, NFTList> og) : base()
    {
        CopyFrom(og);
    }
}

public abstract class Wallet : BindableMonoBehavior
{
    public static readonly ReadOnlyDictionary<int, string> MulticallAddressMap = new ReadOnlyDictionary<int, string>(
        new Dictionary<int, string>()
        {
            {1, "0xeefba1e63905ef1d7acba5a8513c70307c1ce441"}, // Mainnet
            {42, "0x2cc8688c5f75e365aaeeb4ea8d6a480405a48d2a"}, // Kovan
            {4, "0x42ad527de7d4e9d9d011ac45b31d8551f8fe9821"}, // Rinkeby
            {5, "0x77dca2c955b15e9de4dbbcf1246b4b85b651e50e"}, // Görli
            {3, "0x53c43764255c17bd724f74c4ef150724ac50a3ed"}, // Ropsten
            {100, "0xb5b692a88bdfc81ca69dcb1d924f59f0413a602a"}, // xDai
            {137, "0x11ce4B23bD875D7F5C6a31084f55fDe1e9A87507"}, // Polygon
            {80001, "0x08411ADd0b5AA8ee47563b146743C13b3556c9Cc"} // Mumbai
        });
    
    public static readonly ReadOnlyDictionary<int, string> ChainIdToName = new ReadOnlyDictionary<int, string>(
        new Dictionary<int, string>()
        {
            {1, "Ethereum Main Network"}, // Mainnet
            {42, "Ethereum Kovan Network"}, // Kovan
            {4, "Ethereum Rinkeby Network"}, // Rinkeby
            {5, "Ethereum Görli Network"}, // Görli
            {3, "Ethereum Ropsten Network"}, // Ropsten
            {100, "Gnosis Main Network"}, // xDai
            {137, "Polygon Main Network"}, // Polygon
            {80001, "Polygon Mumbai Network"} // Mumbai
        });
    
    public static readonly ReadOnlyDictionary<string, int> ChainNameToId = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>()
    {
        {"Mainnet".ToLower(), 1}, // Mainnet
        {"Kovan".ToLower(), 42}, // Kovan
        {"Rinkeby".ToLower(), 4}, // Rinkeby
        {"Görli".ToLower(), 5}, // Görli
        {"Ropsten".ToLower(), 3}, // Ropsten
        {"xDai".ToLower(), 100}, // xDai
        {"Polygon".ToLower(), 137}, // Polygon
        {"Mumbai".ToLower(), 80001} // Mumbai
    });

    public static Wallet Instance { get; private set; }

    public static Wallet Current
    {
        get
        {
            return Instance;
        }
    }

    public Web3 Web3 { get; private set; }
    
    public abstract bool Connected { get; }
    
    public abstract string Address { get; }
    
    public abstract int ChainId { get; }

    public string InfuraURL
    {
        get
        {
            return "https://" + network + ".infura.io/v3/" + infuraId;
        }
    }

    public StringVariable NetworkName;

    public string infuraId;
    public string network = "mainnet";

    //TODO Somehow automatically import these addresses
    //TODO For demo this should be fine and more predictable
    public string[] NFTTokenContractAddresses;
    public string[] ERC20TokenContractAddresses;

    public NFTDictionary NFTS = new NFTDictionary();

    public EventHandler<Wallet> WalletConnected;

    public int NFTCount
    {
        get
        {
            return NFTS.Keys.Sum(k => NFTS[k].Count);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupListeners();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected abstract void OnProviderReady(Action<Web3> callback);

    void SetupListeners()
    {
        OnProviderReady(delegate(Web3 web3)
        {
            this.Web3 = web3;
            
            Debug.Log("Wallet " + Address + " connected");
            
            RefreshTokensAsync();
            RefreshNFTsAsync();

            NetworkName.Value = ChainIdToName[ChainId];

            if (WalletConnected != null)
            {
                WalletConnected(this, this);
            }
        });
    }

    public void RefreshNFTsAsync()
    {
        StartCoroutine(PerformRefreshNFTs());
    }

    public void RefreshTokensAsync()
    {
        //StartCoroutine(PerformRefreshTokens());
    }

    private IEnumerator PerformRefreshNFTs()
    {
        var refreshTask = PerformRefreshNFTsAsync();

        yield return new WaitForTask(refreshTask);
        
        foreach (var nftTokenDatas in refreshTask.Result.Values)
        {
            foreach (var nftToken in nftTokenDatas)
            {
                yield return nftToken.DownloadImageSprite();
            }
        }

        NFTS = new NFTDictionary(refreshTask.Result);

        foreach (var contract in NFTS.Keys)
        {
            var nftList = NFTS[contract];

            Debug.Log("User has " + nftList.Count + " NFTs from contract " + contract);
        }

        Player.LocalPlayer.WalletCanvas.UpdateNFTView();
    }
    
    private IEnumerator CoroutineWebRequest(string url, BigInteger tokenId, TaskCompletionSource<NFTTokenData> task)
    {
        lock (GlobalNFTStore.CacheLock)
        {
            if (GlobalNFTStore.HasToken(url, tokenId))
            {
                //Return that object
                task.SetResult(GlobalNFTStore.Fetch(url, tokenId));
            }
            else
            {
                if (url.StartsWith("ipfs://"))
                {
                    url = url.Replace("ipfs://", "https://ipfs.io/ipfs/");
                }

                UnityWebRequest uwr = UnityWebRequest.Get(url);

                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError)
                {
                    task.SetException(new IOException(uwr.error));
                }
                else
                {
                    var json = uwr.downloadHandler.text;

                    var result = JsonConvert.DeserializeObject<NFTTokenData>(json);

                    result.tokenId = tokenId.ToString();

                    yield return result.DownloadImageSprite();

                    GlobalNFTStore.Save(url, tokenId, result);

                    task.SetResult(result);
                }
            }
        }
    }

    private async Task<NFTTokenData> AsyncWebRequest(string url, BigInteger tokenId)
    {
        TaskCompletionSource<NFTTokenData> dataSource =
            new TaskCompletionSource<NFTTokenData>(TaskCreationOptions.None);

        MainThreadUtil.Run(CoroutineWebRequest(url, tokenId, dataSource));

        await dataSource.Task;

        return dataSource.Task.Result;
    }

    private async Task<NFTTokenData[]> FetchNFTsAsync(string contract, int amount, int start = 0)
    {
        var queryHandler = Web3.Eth.GetContractQueryHandler<AggregateFunction>();
        
        var tokenIdAggregatorFunction = new AggregateFunction()
        {
            Calls = Enumerable.Range(start, amount).Select(index => new Call()
            {
                Target = contract,
                CallData = new TokenOfOwnerByIndexFunction()
                {
                    Owner = Address,
                    Index = index
                }.GetCallData()
            }).ToList()
        };
        
        var returnCalls = await queryHandler.QueryDeserializingToObjectAsync<AggregateOutputDTO>(
            tokenIdAggregatorFunction,
            MulticallAddressMap[ChainId]).ConfigureAwait(false);

        var tokenURIAggregatorFunction = new AggregateFunction()
        {
            Calls = returnCalls.ReturnData.Select(b => new TokenOfOwnerByIndexOutputDTO().DecodeOutput(b.ToHex()))
                .Select(tokenId => new Call()
                {
                    Target = contract,
                    CallData = new TokenURIFunction()
                    {
                        TokenId = tokenId.ReturnValue1
                    }.GetCallData()
                }).ToList()
        };

        var uriCalls = await queryHandler
            .QueryDeserializingToObjectAsync<AggregateOutputDTO>(tokenURIAggregatorFunction,
                MulticallAddressMap[ChainId]).ConfigureAwait(false);

        return await Task.WhenAll(tokenURIAggregatorFunction.Calls
            .Select(c => new TokenURIFunction().DecodeInput(c.CallData.ToHex()).TokenId)
            .Zip(uriCalls.ReturnData.Select(b => new TokenURIOutputDTO().DecodeOutput(b.ToHex())),
                (id, uriData) => new {id, uri = uriData.ReturnValue1})
            .Select(uriData => AsyncWebRequest(uriData.uri, uriData.id)));
    }

    private async Task<Dictionary<string, NFTList>> PerformRefreshNFTsAsync()
    { 
        var balanceOfAggregateFunction = new AggregateFunction()
        {
            Calls = NFTTokenContractAddresses.Select(addr => new Call()
            {
                Target = addr,
                CallData = new BalanceOfFunction {Owner = Address}.GetCallData()
            }).ToList()
        };

        var queryHandler = Web3.Eth.GetContractQueryHandler<AggregateFunction>();

        var returnCalls =
            await queryHandler.QueryDeserializingToObjectAsync<AggregateOutputDTO>(balanceOfAggregateFunction,
                MulticallAddressMap[ChainId]).ConfigureAwait(false);

        var pendingResultsGroup = balanceOfAggregateFunction.Calls.Select(c => c.Target)
            .Zip(returnCalls.ReturnData.Select(b => new BalanceOfOutputDTO().DecodeOutput(b.ToHex())),
                (addr, amount) => new {addr, amount}).Select(nd => new
                {nd.addr, FetchTask = FetchNFTsAsync(nd.addr, (int) nd.amount.ReturnValue1)}).ToList();

        await Task.WhenAll(pendingResultsGroup.Select(r => r.FetchTask));

        var results = pendingResultsGroup.Select(pr => new {pr.addr, Result = pr.FetchTask.Result});

        return results.ToDictionary(r => r.addr, r => new NFTList(r.Result));
    }
}
