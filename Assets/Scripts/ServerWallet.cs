using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DefaultNamespace;
using Mirror;
using Nethereum.Contracts;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using WalletConnectSharp.Unity.Models;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition
{
    public static class NFTDictionaryNetworkSerializer
    {
        public static void WriteNFTDictionary(this NetworkWriter writer, NFTDictionary dictionary)
        {
            writer.WriteBool(dictionary == null);
            if (dictionary == null)
                return;
            
            //Write key count -> key -> value length -> all values
            writer.WriteInt(dictionary.Keys.Count);
            foreach (var key in dictionary.Keys)
            {
                writer.WriteString(key);
                var values = dictionary[key];
                writer.WriteInt(values.Count);

                foreach (var value in values)
                {
                    writer.WriteNFTTokenData(value);
                }
            }
        }

        public static void WriteNFTTokenData(this NetworkWriter writer, NFTTokenData nft)
        {
            writer.WriteBool(nft == null);
            if (nft == null)
                return;
            
            writer.WriteString(nft.name);
            writer.WriteString(nft.description);
            writer.WriteString(nft.image);
            writer.WriteString(nft.tokenId);
        }

        public static NFTTokenData ReadNFTTokenData(this NetworkReader reader)
        {
            var isNull = reader.ReadBool();
            if (isNull)
                return null;
            
            var name = reader.ReadString();
            var description = reader.ReadString();
            var image = reader.ReadString();
            var tokenId = reader.ReadString();

            return new NFTTokenData()
            {
                name = name,
                description = description,
                image = image,
                tokenId = tokenId
            };
        }

        public static NFTDictionary ReadNFTDictionary(this NetworkReader reader)
        {
            var isNull = reader.ReadBool();
            if (isNull)
                return null;
            
            var keyCount = reader.ReadInt();
            NFTDictionary dictionary = new NFTDictionary();
            for (int i = 0; i < keyCount; i++)
            {
                var key = reader.ReadString();
                var valueCount = reader.ReadInt();

                NFTList list = new NFTList();
                for (int j = 0; j < valueCount; j++)
                {
                    var value = reader.ReadNFTTokenData();
                    list.Add(value);
                }
                
                dictionary.Add(key, list);
            }

            return dictionary;
        }
    }

    public class ServerWallet : BindableNetworkBehavior
    {
        public string privateKey = "0x9c9bef815e4161be3cc4f09e8c2b95f386ba2c2f1034f90ccaadd980098f6697";
        public Web3 Web3 { get; private set; }
        public string[] NFTTokenContractAddresses;

        [SyncVar] public NFTDictionary NFTS = new NFTDictionary();

        private Dictionary<NFTTokenData, GameObject> floorItems = new Dictionary<NFTTokenData, GameObject>();

        [HideInInspector]
        public List<string> vendingItems = new List<string>(); 
        
        [HideInInspector]
        public List<NFTTokenData> pendingItems = new List<NFTTokenData>(); 
        
        private Dictionary<NFTTokenData, Vector3> commitedItems = new Dictionary<NFTTokenData, Vector3>();

        public override void OnStartServer()
        {
            base.OnStartServer();

            SetupServerWallet();
        }

        [Command(requiresAuthority = false)]
        public void CmdPickupNFT(string username, NFTTokenData nft)
        {
            Player p = FindObjectsOfType<Player>().FirstOrDefault(p => p.Username == username);

            if (p == null)
            {
                Debug.LogError("NFT not found");
                return;
            }
            
            if (!floorItems.ContainsKey(nft))
            {
                Debug.LogError("NFT not on floor");
                p.TargetFailPickup(p.netIdentity.connectionToClient);
                return;
            }

            //now find the nft address
            string nftAddress = null;
            foreach (var address in NFTS.Keys)
            {
                var values = NFTS[address];
                foreach (var possibleNFT in values)
                {
                    if (Equals(possibleNFT, nft))
                    {
                        nftAddress = address;
                        break;
                    }
                }

                if (nftAddress != null)
                    break;
            }

            if (nftAddress == null)
            {
                Debug.LogError("Failed to find nft address");
                p.TargetFailPickup(p.netIdentity.connectionToClient);
                return;
            }

            //Grab the original object
            var obj = floorItems[nft];

            var holder = obj.GetComponent<NFTHolder>();
            
            pendingItems.Add(nft);
            
            DespawnNFT(holder);

            StartCoroutine(SendNFT(p, nftAddress, nft));
        }

        [Server]
        void SetupServerWallet()
        {
            var account = new Account(privateKey, Wallet.Current.ChainId);
            var infuraURL = Wallet.Current.InfuraURL;
            Web3 = new Web3(account, infuraURL);

            Address = account.Address;
            ChainId = Wallet.Current.ChainId;
            
            RefreshNFTsAsync();

            StartCoroutine(WalletUpdateTask());
        }
        
        [Server]
        private IEnumerator WalletUpdateTask()
        {
            while (isActiveAndEnabled)
            {
                if (Web3 == null || string.IsNullOrWhiteSpace(Address))
                {
                    yield return new WaitForSeconds(3);
                    continue;
                }

                //Update all NFTs
                RefreshNFTsAsync();
            
                //TODO
                //Update all tokens
                //Wallet.Current.RefreshTokensAsync();

                yield return new WaitForSeconds(3);
            }
        }

        [SyncVar] public string Address;

        [SyncVar] public int ChainId;

        public GameObject nftObjectToSpawn;


        [Server]
        public void RefreshNFTsAsync()
        {
            StartCoroutine(PerformRefreshNFTs());
        }

        [Command(requiresAuthority = false)]
        public void CmdCommitNFT(NFTTokenData nft, Vector3 position)
        {
            commitedItems.Add(nft, position);
        }

        [Server]
        public void SpawnNFT(NFTTokenData nft, Vector3 spawnLocation)
        {
            if (floorItems.ContainsKey(nft))
                return;

            if (pendingItems.Contains(nft))
                return;
            
            var nftObject = Instantiate(nftObjectToSpawn, spawnLocation, nftObjectToSpawn.transform.rotation);
            var nftData = nftObject.GetComponent<NFTHolder>();

            nftData.NftTokenData = nft;
            NetworkServer.Spawn(nftObject);
            
            floorItems.Add(nft, nftObject);
        }

        [Server]
        public void DespawnNFT(NFTHolder nft)
        {
            if (!floorItems.ContainsKey(nft.NftTokenData))
                return;
            
            NetworkServer.Destroy(nft.gameObject);

            floorItems.Remove(nft.NftTokenData);
        }

        private Vector3 PickRandomNFTSpawnPoint()
        {
            var spawns = GameObject.FindGameObjectsWithTag("NFTSpawnPoint");

            var position = spawns[Random.Range(0, spawns.Length)].transform.position;

            var randomPos = position + new Vector3(Random.Range(-4f, 4f), Random.Range(1f, 4f), Random.Range(-4f, 4f));

            return randomPos;
        }

        private IEnumerator SendNFT(Player p, string nftAddress, NFTTokenData nft)
        {
            var transferHandler = Web3.Eth.GetContractTransactionHandler<TransferFromFunction>();
            var transferCall = new TransferFromFunction()
            {
                From = Address,
                To = p.PlayerAddress,
                TokenId = BigInteger.Parse(nft.tokenId)
            };

            var transactionTask = transferHandler.SendRequestAndWaitForReceiptAsync(nftAddress, transferCall);

            yield return new WaitForTaskResult<TransactionReceipt>(transactionTask);
            
            p.TargetCompletePickup(p.netIdentity.connectionToClient);

            pendingItems.Remove(nft);
        }

        [Server]
        private IEnumerator PerformRefreshNFTs()
        {
            var refreshTask = PerformRefreshNFTsAsync();

            yield return new WaitForTask(refreshTask);

            NFTS = new NFTDictionary(refreshTask.Result);

            foreach (var contract in NFTS.Keys)
            {
                var nftList = NFTS[contract];

                foreach (var nft in nftList)
                {
                    if (vendingItems.Contains(nft.tokenId))
                        continue; // Skip items being vended

                    Vector3 position;
                    if (commitedItems.ContainsKey(nft))
                    {
                        position = commitedItems[nft];
                        commitedItems.Remove(nft);
                    }
                    else
                        position = PickRandomNFTSpawnPoint();
                    
                    SpawnNFT(nft, position);
                }

                Debug.Log("Server has " + nftList.Count + " NFTs from contract " + contract);
            }
            
            
        }

        [Server]
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

        [Server]
        private async Task<NFTTokenData> AsyncWebRequest(string url, BigInteger tokenId)
        {
            TaskCompletionSource<NFTTokenData> dataSource =
                new TaskCompletionSource<NFTTokenData>(TaskCreationOptions.None);

            MainThreadUtil.Run(CoroutineWebRequest(url, tokenId, dataSource));

            await dataSource.Task;

            return dataSource.Task.Result;
        }
        
        [Server]
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
                Wallet.MulticallAddressMap[ChainId]).ConfigureAwait(false);

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
                    Wallet.MulticallAddressMap[ChainId]).ConfigureAwait(false);

            return await Task.WhenAll(tokenURIAggregatorFunction.Calls
                .Select(c => new TokenURIFunction().DecodeInput(c.CallData.ToHex()).TokenId)
                .Zip(uriCalls.ReturnData.Select(b => new TokenURIOutputDTO().DecodeOutput(b.ToHex())),
                    (id, uriData) => new {id, uri = uriData.ReturnValue1})
                .Select(uriData => AsyncWebRequest(uriData.uri, uriData.id)));
        }

        [Server]
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
                    Wallet.MulticallAddressMap[ChainId]).ConfigureAwait(false);

            var pendingResultsGroup = balanceOfAggregateFunction.Calls.Select(c => c.Target)
                .Zip(returnCalls.ReturnData.Select(b => new BalanceOfOutputDTO().DecodeOutput(b.ToHex())),
                    (addr, amount) => new {addr, amount}).Select(nd => new
                    {nd.addr, FetchTask = FetchNFTsAsync(nd.addr, (int) nd.amount.ReturnValue1)}).ToList();

            await Task.WhenAll(pendingResultsGroup.Select(r => r.FetchTask));

            var results = pendingResultsGroup.Select(pr => new {pr.addr, Result = pr.FetchTask.Result});

            return results.ToDictionary(r => r.addr, r => new NFTList(r.Result));
        }
    }
}