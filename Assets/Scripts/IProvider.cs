using System;
using Nethereum.Web3;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition
{
    public interface IProvider
    {
        void Start(Wallet wallet);
        
        bool Connected { get; }
        int ChainId { get; }
        string Address { get; }
        
        void OnProviderReady(Action<Web3> callback);
    }
}