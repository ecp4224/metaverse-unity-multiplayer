using System.Collections.Generic;
using System.Numerics;
using WalletConnectSharp.Unity.Models;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition
{
    public static class GlobalNFTStore
    {
        public static readonly object CacheLock = new object();
        private static Dictionary<string, NFTTokenData> nftGlobalCache = new Dictionary<string, NFTTokenData>();

        public static string KeyFor(string url, BigInteger tokenId)
        {
            return  url + "::::" + tokenId;
        }
        
        public static bool HasToken(string url, BigInteger tokenId)
        {
            return nftGlobalCache.ContainsKey(KeyFor(url, tokenId));
        }

        public static NFTTokenData Fetch(string url, BigInteger tokenId)
        {
            return nftGlobalCache[KeyFor(url, tokenId)];
        }

        public static void Save(string url, BigInteger token, NFTTokenData data)
        {
            nftGlobalCache.Add(KeyFor(url, token), data);
        }
    }
}