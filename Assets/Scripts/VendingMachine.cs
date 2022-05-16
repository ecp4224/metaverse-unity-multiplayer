using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DefaultNamespace;
using ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition.UI;
using Mirror;
using Nethereum.Contracts;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity.Models;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition
{
    public class VendingMachine : BindableNetworkBehavior
    {
        public string contractAddress = "0x262a4160921Bf1492ECFFE6888a69300F43229Ad";
        public string[] NFTURLs;

        public decimal payment = 0.2m;

        public string displayName = "Vending Machine";

        public GameObject objectToSpawn;
        public Transform spawnLocation;

        [Inject]
        private ServerWallet _serverWallet;

        [Command(requiresAuthority = false)]
        public void CmdDoTransaction(string username)
        {
            Player p = FindObjectsOfType<Player>().FirstOrDefault(p => p.Username == username);

            if (p == null)
                throw new ArgumentException("Could not find username " + username + " for vending machine buy command");
            
            p.transactionToName.Value = displayName;
            p.transactionToAddress.Value = contractAddress.Substring(0, 4) + "..." +
                                           contractAddress.Substring(contractAddress.Length - 4, 4);
            p.transactionToAmount.Value = payment + " ETH";
            
            p.WalletCanvas.ShowConfirm(delegate
            {
                StartCoroutine(SendTransaction(p, payment));
            });
        }
        
        private IEnumerator SendTransaction(Player p, decimal amount)
        {
            var randomURI = NFTURLs[Random.Range(0, NFTURLs.Length)];
            
            var buyHandler = Wallet.Current.Web3.Eth.GetContractTransactionHandler<MintNewNFTFunction>();
            var buyCall = new MintNewNFTFunction()
            {
                To = _serverWallet.Address,
                MyUri = randomURI,
                AmountToSend = Web3.Convert.ToWei(amount)
            };

            var transactionTask = buyHandler.SendRequestAndWaitForReceiptAsync(contractAddress, buyCall);
        
            p.WalletCanvas.ShowLoader();
        
            yield return new WaitForTaskResult<TransactionReceipt>(transactionTask);

            if (transactionTask.IsFaulted)
            {
                p.errorMessage.Value = "Error:\n" + transactionTask.Exception.Message;
                p.WalletCanvas.ShowScreen("error");
                p.WalletCanvas.HideLoader();
            }
            else
            {
                var transactionReceipt = transactionTask.Result;
                
                //Grab tokenId from recipet
                var transferEventOutput = transactionReceipt.DecodeAllEvents<TransferEventDTO>();

                var tokenURIHandler = Wallet.Current.Web3.Eth.GetContractQueryHandler<TokenURIFunction>();
                foreach (var nftOutput in transferEventOutput)
                {
                    var tokenId = nftOutput.Event.TokenId;
                    var nftAddress = nftOutput.Log.Address;
                    
                    Debug.Log("Minted new NFT: " + tokenId.ToString());

                    var tokenUriCall = new TokenURIFunction()
                    {
                        TokenId = tokenId
                    };
                    
                    _serverWallet.vendingItems.Add(tokenId.ToString());

                    yield return new WaitForSeconds(2);

                    var tokenURITask =
                        tokenURIHandler.QueryDeserializingToObjectAsync<TokenURIOutputDTO>(tokenUriCall,
                            nftAddress);

                    yield return new WaitForTaskResult<TokenURIOutputDTO>(tokenURITask);

                    var uri = tokenURITask.Result.ReturnValue1;

                    var nftDataTask = AsyncWebRequest(uri, tokenId);

                    yield return new WaitForTaskResult<NFTTokenData>(nftDataTask);
                    
                    _serverWallet.SpawnNFT(nftDataTask.Result, spawnLocation.position);

                    _serverWallet.vendingItems.Remove(tokenId.ToString());
                }

                p.WalletCanvas.ShowScreen("successVending");
                p.WalletCanvas.HideLoader();
            }
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
    }
}