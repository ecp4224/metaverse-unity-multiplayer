using System;
using Nethereum.Web3;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition
{
    public interface IWeb3Provider
    {
        void OnProviderReady(Action<Web3> callback);
    }
}