using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace PlayerDirectory
{
    
    /*public class PlayerDirectoryConsole
    {
        public static async Task Main()
        {
            var url = "http://testchain.nethereum.com:8545";
            //var url = "https://mainnet.infura.io";
            var privateKey = "0x7580e7fb49df1c861f0050fae31c2224c6aba908e116b8da44ee8cd927b990b0";
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            var web3 = new Web3(account, url);
            
             /* Deployment 
            var playerDirectoryDeployment = new PlayerDirectoryDeployment();

            var transactionReceiptDeployment = await web3.Eth.GetContractDeploymentHandler<PlayerDirectoryDeployment>().SendRequestAndWaitForReceiptAsync(playerDirectoryDeployment);
            var contractAddress = transactionReceiptDeployment.ContractAddress;
             #1# 

            /** Function: initialize*#1#
            /*
            var initializeFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync<InitializeFunction>();
            #1#


            /** Function: owner*#1#
            /*
            var ownerFunctionReturn = await contractHandler.QueryAsync<OwnerFunction, string>();
            #1#


            /** Function: register*#1#
            /*
            var registerFunction = new RegisterFunction();
            registerFunction.User = user;
            registerFunction.Username = username;
            var registerFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(registerFunction);
            #1#


            /** Function: renounceOwnership*#1#
            /*
            var renounceOwnershipFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync<RenounceOwnershipFunction>();
            #1#


            /** Function: setStartAmount*#1#
            /*
            var setStartAmountFunction = new SetStartAmountFunction();
            setStartAmountFunction.Amount = amount;
            var setStartAmountFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(setStartAmountFunction);
            #1#


            /** Function: startAmount*#1#
            /*
            var startAmountFunctionReturn = await contractHandler.QueryAsync<StartAmountFunction, BigInteger>();
            #1#


            /** Function: transferOwnership*#1#
            /*
            var transferOwnershipFunction = new TransferOwnershipFunction();
            transferOwnershipFunction.NewOwner = newOwner;
            var transferOwnershipFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction);
            #1#


            /** Function: userToProfile*#1#
            /*
            var userToProfileFunction = new UserToProfileFunction();
            userToProfileFunction.ReturnValue1 = returnValue1;
            var userToProfileFunctionReturn = await contractHandler.QueryAsync<UserToProfileFunction, string>(userToProfileFunction);
            #1#


            /** Function: usernameTaken*#1#
            /*
            var usernameTakenFunction = new UsernameTakenFunction();
            usernameTakenFunction.ReturnValue1 = returnValue1;
            var usernameTakenFunctionReturn = await contractHandler.QueryAsync<UsernameTakenFunction, bool>(usernameTakenFunction);
            #1#


            /** Function: usernames*#1#
            /*
            var usernamesFunction = new UsernamesFunction();
            usernamesFunction.ReturnValue1 = returnValue1;
            var usernamesFunctionReturn = await contractHandler.QueryAsync<UsernamesFunction, string>(usernamesFunction);
            #1#


            /** Function: withdraw*#1#
            /*
            var withdrawFunction = new WithdrawFunction();
            withdrawFunction.Amount = amount;
            var withdrawFunctionTxnReceipt = await contractHandler.SendRequestAndWaitForReceiptAsync(withdrawFunction);
            #1#
        }

    }*/

    public partial class PlayerDirectoryDeployment : PlayerDirectoryDeploymentBase
    {
        public PlayerDirectoryDeployment() : base(BYTECODE) { }
        public PlayerDirectoryDeployment(string byteCode) : base(byteCode) { }
    }

    public class PlayerDirectoryDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "0x6080604052670de0b6b3a764000060685534801561001c57600080fd5b5061212e8061002c6000396000f3fe608060405260043610620000af5760003560e01c80638da5cb5b116200006d5780638da5cb5b1462000179578063d245a46a14620001a9578063dc738d2714620001ed578063ee91877c146200021b578063f26e818a146200025f578063f2fde38b14620002a357620000b7565b80630f76787214620000b95780632e1a7d4d14620000e957806332434a2e1462000117578063715018a614620001455780638129fc1c146200015f57620000b7565b36620000b757005b005b348015620000c657600080fd5b50620000d1620002d1565b604051620000e0919062001568565b60405180910390f35b348015620000f657600080fd5b506200011560048036038101906200010f9190620011a4565b620002d7565b005b3480156200012457600080fd5b506200014360048036038101906200013d919062001105565b620003fa565b005b3480156200015257600080fd5b506200015d6200072f565b005b3480156200016c57600080fd5b5062000177620007c0565b005b3480156200018657600080fd5b5062000191620008a9565b604051620001a09190620013dd565b60405180910390f35b348015620001b657600080fd5b50620001d56004803603810190620001cf91906200115f565b620008d3565b604051620001e49190620013dd565b60405180910390f35b348015620001fa57600080fd5b50620002196004803603810190620002139190620011a4565b6200091c565b005b3480156200022857600080fd5b50620002476004803603810190620002419190620010d9565b620009ef565b60405162000256919062001434565b60405180910390f35b3480156200026c57600080fd5b506200028b60048036038101906200028591906200115f565b62000a97565b6040516200029a919062001417565b60405180910390f35b348015620002b057600080fd5b50620002cf6004803603810190620002c99190620010d9565b62000acd565b005b60685481565b620002e162000bd1565b73ffffffffffffffffffffffffffffffffffffffff1662000301620008a9565b73ffffffffffffffffffffffffffffffffffffffff16146200035a576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401620003519062001502565b60405180910390fd5b47811015620003a0576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040162000397906200147a565b60405180910390fd5b6000620003ac620008a9565b90508073ffffffffffffffffffffffffffffffffffffffff166108fc839081150290604051600060405180830381858888f19350505050158015620003f5573d6000803e3d6000fd5b505050565b6067816040516200040c9190620013c4565b908152602001604051809103902060009054906101000a900460ff16156200046b576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401620004629062001546565b60405180910390fd5b6000606660008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000208054620004b990620016ea565b905014620004fe576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401620004f590620014e0565b60405180910390fd5b60685447101562000546576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016200053d9062001524565b60405180910390fd5b80606660008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002090805190602001906200059b92919062000f64565b506001606782604051620005b09190620013c4565b908152602001604051809103902060006101000a81548160ff0219169083151502179055508173ffffffffffffffffffffffffffffffffffffffff166108fc6068549081150290604051600060405180830381858888f193505050501580156200061e573d6000803e3d6000fd5b50600030604051620006309062000ff5565b6200063c9190620013dd565b604051809103906000f08015801562000659573d6000803e3d6000fd5b5090508073ffffffffffffffffffffffffffffffffffffffff1663f2fde38b846040518263ffffffff1660e01b8152600401620006979190620013fa565b600060405180830381600087803b158015620006b257600080fd5b505af1158015620006c7573d6000803e3d6000fd5b5050505080606583604051620006de9190620013c4565b908152602001604051809103902060006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550505050565b6200073962000bd1565b73ffffffffffffffffffffffffffffffffffffffff1662000759620008a9565b73ffffffffffffffffffffffffffffffffffffffff1614620007b2576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401620007a99062001502565b60405180910390fd5b620007be600062000bd9565b565b600060019054906101000a900460ff1680620007e7575060008054906101000a900460ff16155b62000829576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016200082090620014be565b60405180910390fd5b60008060019054906101000a900460ff1615905080156200087a576001600060016101000a81548160ff02191690831515021790555060016000806101000a81548160ff0219169083151502179055505b6200088462000c9f565b8015620008a65760008060016101000a81548160ff0219169083151502179055505b50565b6000603360009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905090565b6065818051602081018201805184825260208301602085012081835280955050505050506000915054906101000a900473ffffffffffffffffffffffffffffffffffffffff1681565b6200092662000bd1565b73ffffffffffffffffffffffffffffffffffffffff1662000946620008a9565b73ffffffffffffffffffffffffffffffffffffffff16146200099f576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401620009969062001502565b60405180910390fd5b60008111620009e5576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401620009dc9062001458565b60405180910390fd5b8060688190555050565b6066602052806000526040600020600091509050805462000a1090620016ea565b80601f016020809104026020016040519081016040528092919081815260200182805462000a3e90620016ea565b801562000a8f5780601f1062000a635761010080835404028352916020019162000a8f565b820191906000526020600020905b81548152906001019060200180831162000a7157829003601f168201915b505050505081565b6067818051602081018201805184825260208301602085012081835280955050505050506000915054906101000a900460ff1681565b62000ad762000bd1565b73ffffffffffffffffffffffffffffffffffffffff1662000af7620008a9565b73ffffffffffffffffffffffffffffffffffffffff161462000b50576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040162000b479062001502565b60405180910390fd5b600073ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff16141562000bc3576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040162000bba906200149c565b60405180910390fd5b62000bce8162000bd9565b50565b600033905090565b6000603360009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905081603360006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055508173ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff167f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e060405160405180910390a35050565b600060019054906101000a900460ff168062000cc6575060008054906101000a900460ff16155b62000d08576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040162000cff90620014be565b60405180910390fd5b60008060019054906101000a900460ff16159050801562000d59576001600060016101000a81548160ff02191690831515021790555060016000806101000a81548160ff0219169083151502179055505b62000d6362000d92565b62000d6d62000e71565b801562000d8f5760008060016101000a81548160ff0219169083151502179055505b50565b600060019054906101000a900460ff168062000db9575060008054906101000a900460ff16155b62000dfb576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040162000df290620014be565b60405180910390fd5b60008060019054906101000a900460ff16159050801562000e4c576001600060016101000a81548160ff02191690831515021790555060016000806101000a81548160ff0219169083151502179055505b801562000e6e5760008060016101000a81548160ff0219169083151502179055505b50565b600060019054906101000a900460ff168062000e98575060008054906101000a900460ff16155b62000eda576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040162000ed190620014be565b60405180910390fd5b60008060019054906101000a900460ff16159050801562000f2b576001600060016101000a81548160ff02191690831515021790555060016000806101000a81548160ff0219169083151502179055505b62000f3f62000f3962000bd1565b62000bd9565b801562000f615760008060016101000a81548160ff0219169083151502179055505b50565b82805462000f7290620016ea565b90600052602060002090601f01602090048101928262000f96576000855562000fe2565b82601f1062000fb157805160ff191683800117855562000fe2565b8280016001018555821562000fe2579182015b8281111562000fe157825182559160200191906001019062000fc4565b5b50905062000ff1919062001003565b5090565b6106b98062001a4083390190565b5b808211156200101e57600081600090555060010162001004565b5090565b6000620010396200103384620015ae565b62001585565b9050828152602081018484840111156200105257600080fd5b6200105f848285620016a5565b509392505050565b6000813590506200107881620019f1565b92915050565b6000813590506200108f8162001a0b565b92915050565b600082601f830112620010a757600080fd5b8135620010b984826020860162001022565b91505092915050565b600081359050620010d38162001a25565b92915050565b600060208284031215620010ec57600080fd5b6000620010fc8482850162001067565b91505092915050565b600080604083850312156200111957600080fd5b600062001129858286016200107e565b925050602083013567ffffffffffffffff8111156200114757600080fd5b620011558582860162001095565b9150509250929050565b6000602082840312156200117257600080fd5b600082013567ffffffffffffffff8111156200118d57600080fd5b6200119b8482850162001095565b91505092915050565b600060208284031215620011b757600080fd5b6000620011c784828501620010c2565b91505092915050565b620011db8162001669565b82525050565b620011ec816200160b565b82525050565b620011fd8162001633565b82525050565b60006200121082620015e4565b6200121c8185620015ef565b93506200122e818560208601620016b4565b6200123981620017b4565b840191505092915050565b60006200125182620015e4565b6200125d818562001600565b93506200126f818560208601620016b4565b80840191505092915050565b60006200128a602583620015ef565b91506200129782620017c5565b604082019050919050565b6000620012b1601983620015ef565b9150620012be8262001814565b602082019050919050565b6000620012d8602683620015ef565b9150620012e5826200183d565b604082019050919050565b6000620012ff602e83620015ef565b91506200130c826200188c565b604082019050919050565b600062001326602383620015ef565b91506200133382620018db565b604082019050919050565b60006200134d602083620015ef565b91506200135a826200192a565b602082019050919050565b600062001374602283620015ef565b9150620013818262001953565b604082019050919050565b60006200139b602483620015ef565b9150620013a882620019a2565b604082019050919050565b620013be816200165f565b82525050565b6000620013d2828462001244565b915081905092915050565b6000602082019050620013f46000830184620011e1565b92915050565b6000602082019050620014116000830184620011d0565b92915050565b60006020820190506200142e6000830184620011f2565b92915050565b6000602082019050818103600083015262001450818462001203565b905092915050565b6000602082019050818103600083015262001473816200127b565b9050919050565b600060208201905081810360008301526200149581620012a2565b9050919050565b60006020820190508181036000830152620014b781620012c9565b9050919050565b60006020820190508181036000830152620014d981620012f0565b9050919050565b60006020820190508181036000830152620014fb8162001317565b9050919050565b600060208201905081810360008301526200151d816200133e565b9050919050565b600060208201905081810360008301526200153f8162001365565b9050919050565b6000602082019050818103600083015262001561816200138c565b9050919050565b60006020820190506200157f6000830184620013b3565b92915050565b600062001591620015a4565b90506200159f828262001720565b919050565b6000604051905090565b600067ffffffffffffffff821115620015cc57620015cb62001785565b5b620015d782620017b4565b9050602081019050919050565b600081519050919050565b600082825260208201905092915050565b600081905092915050565b600062001618826200163f565b9050919050565b60006200162c826200163f565b9050919050565b60008115159050919050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000819050919050565b600062001676826200167d565b9050919050565b60006200168a8262001691565b9050919050565b60006200169e826200163f565b9050919050565b82818337600083830152505050565b60005b83811015620016d4578082015181840152602081019050620016b7565b83811115620016e4576000848401525b50505050565b600060028204905060018216806200170357607f821691505b602082108114156200171a576200171962001756565b5b50919050565b6200172b82620017b4565b810181811067ffffffffffffffff821117156200174d576200174c62001785565b5b80604052505050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b6000601f19601f8301169050919050565b7f506c6179657273206d757374207374617274206f7574207769746820736f6d6560008201527f7468696e67000000000000000000000000000000000000000000000000000000602082015250565b7f43616e6e6f742077697468647261772074686174206d75636800000000000000600082015250565b7f4f776e61626c653a206e6577206f776e657220697320746865207a65726f206160008201527f6464726573730000000000000000000000000000000000000000000000000000602082015250565b7f496e697469616c697a61626c653a20636f6e747261637420697320616c72656160008201527f647920696e697469616c697a6564000000000000000000000000000000000000602082015250565b7f54686973206164647265737320616c726561647920686173206120757365726e60008201527f616d650000000000000000000000000000000000000000000000000000000000602082015250565b7f4f776e61626c653a2063616c6c6572206973206e6f7420746865206f776e6572600082015250565b7f526567697374617274696f6e2069732063757272656e746c792064697361626c60008201527f6564000000000000000000000000000000000000000000000000000000000000602082015250565b7f5468697320757365726e616d652068617320616c7265616479206265656e207460008201527f616b656e00000000000000000000000000000000000000000000000000000000602082015250565b620019fc816200160b565b811462001a0857600080fd5b50565b62001a16816200161f565b811462001a2257600080fd5b50565b62001a30816200165f565b811462001a3c57600080fd5b5056fe608060405234801561001057600080fd5b506040516106b93803806106b98339818101604052810190610032919061008e565b80606560006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff16021790555050610100565b600081519050610088816100e9565b92915050565b6000602082840312156100a057600080fd5b60006100ae84828501610079565b91505092915050565b60006100c2826100c9565b9050919050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6100f2816100b7565b81146100fd57600080fd5b50565b6105aa8061010f6000396000f3fe608060405234801561001057600080fd5b50600436106100575760003560e01c8063715018a61461005c5780638da5cb5b14610066578063c41c2f2414610084578063ca7b8704146100a2578063f2fde38b146100c0575b600080fd5b6100646100dc565b005b61006e610164565b60405161007b9190610422565b60405180910390f35b61008c61018e565b6040516100999190610422565b60405180910390f35b6100aa6101b4565b6040516100b7919061047d565b60405180910390f35b6100da60048036038101906100d59190610395565b6101ba565b005b6100e46102b2565b73ffffffffffffffffffffffffffffffffffffffff16610102610164565b73ffffffffffffffffffffffffffffffffffffffff1614610158576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161014f9061045d565b60405180910390fd5b61016260006102ba565b565b6000603360009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905090565b606560009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1681565b60665481565b6101c26102b2565b73ffffffffffffffffffffffffffffffffffffffff166101e0610164565b73ffffffffffffffffffffffffffffffffffffffff1614610236576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161022d9061045d565b60405180910390fd5b600073ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff1614156102a6576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161029d9061043d565b60405180910390fd5b6102af816102ba565b50565b600033905090565b6000603360009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905081603360006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055508173ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff167f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e060405160405180910390a35050565b60008135905061038f8161055d565b92915050565b6000602082840312156103a757600080fd5b60006103b584828501610380565b91505092915050565b6103c7816104a9565b82525050565b60006103da602683610498565b91506103e5826104e5565b604082019050919050565b60006103fd602083610498565b915061040882610534565b602082019050919050565b61041c816104db565b82525050565b600060208201905061043760008301846103be565b92915050565b60006020820190508181036000830152610456816103cd565b9050919050565b60006020820190508181036000830152610476816103f0565b9050919050565b60006020820190506104926000830184610413565b92915050565b600082825260208201905092915050565b60006104b4826104bb565b9050919050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000819050919050565b7f4f776e61626c653a206e6577206f776e657220697320746865207a65726f206160008201527f6464726573730000000000000000000000000000000000000000000000000000602082015250565b7f4f776e61626c653a2063616c6c6572206973206e6f7420746865206f776e6572600082015250565b610566816104a9565b811461057157600080fd5b5056fea2646970667358221220e6fd4ab0f0756781678974a0e19f0644fff2482eec5564f6164df3bfe1ed384a64736f6c63430008040033a264697066735822122019de61dcbca95348d9e5b3b8f59d9caeccfeda7f6ec0a3aab5c9e16d7117938f64736f6c63430008040033";
        public PlayerDirectoryDeploymentBase() : base(BYTECODE) { }
        public PlayerDirectoryDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class InitializeFunction : InitializeFunctionBase { }

    [Function("initialize")]
    public class InitializeFunctionBase : FunctionMessage
    {

    }

    public partial class OwnerFunction : OwnerFunctionBase { }

    [Function("owner", "address")]
    public class OwnerFunctionBase : FunctionMessage
    {

    }

    public partial class RegisterFunction : RegisterFunctionBase { }

    [Function("register")]
    public class RegisterFunctionBase : FunctionMessage
    {
        [Parameter("address", "user", 1)]
        public virtual string User { get; set; }
        [Parameter("string", "username", 2)]
        public virtual string Username { get; set; }
    }

    public partial class RenounceOwnershipFunction : RenounceOwnershipFunctionBase { }

    [Function("renounceOwnership")]
    public class RenounceOwnershipFunctionBase : FunctionMessage
    {

    }

    public partial class SetStartAmountFunction : SetStartAmountFunctionBase { }

    [Function("setStartAmount")]
    public class SetStartAmountFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "amount", 1)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class StartAmountFunction : StartAmountFunctionBase { }

    [Function("startAmount", "uint256")]
    public class StartAmountFunctionBase : FunctionMessage
    {

    }

    public partial class TransferOwnershipFunction : TransferOwnershipFunctionBase { }

    [Function("transferOwnership")]
    public class TransferOwnershipFunctionBase : FunctionMessage
    {
        [Parameter("address", "newOwner", 1)]
        public virtual string NewOwner { get; set; }
    }

    public partial class UserToProfileFunction : UserToProfileFunctionBase { }

    [Function("userToProfile", "address")]
    public class UserToProfileFunctionBase : FunctionMessage
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class UsernameTakenFunction : UsernameTakenFunctionBase { }

    [Function("usernameTaken", "bool")]
    public class UsernameTakenFunctionBase : FunctionMessage
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class UsernamesFunction : UsernamesFunctionBase { }

    [Function("usernames", "string")]
    public class UsernamesFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class WithdrawFunction : WithdrawFunctionBase { }

    [Function("withdraw")]
    public class WithdrawFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "amount", 1)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class OwnershipTransferredEventDTO : OwnershipTransferredEventDTOBase { }

    [Event("OwnershipTransferred")]
    public class OwnershipTransferredEventDTOBase : IEventDTO
    {
        [Parameter("address", "previousOwner", 1, true )]
        public virtual string PreviousOwner { get; set; }
        [Parameter("address", "newOwner", 2, true )]
        public virtual string NewOwner { get; set; }
    }



    public partial class OwnerOutputDTO : OwnerOutputDTOBase { }

    [FunctionOutput]
    public class OwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }


    public partial class StartAmountOutputDTO : StartAmountOutputDTOBase { }

    [FunctionOutput]
    public class StartAmountOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    public partial class UserToProfileOutputDTO : UserToProfileOutputDTOBase { }

    [FunctionOutput]
    public class UserToProfileOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class UsernameTakenOutputDTO : UsernameTakenOutputDTOBase { }

    [FunctionOutput]
    public class UsernameTakenOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class UsernamesOutputDTO : UsernamesOutputDTOBase { }

    [FunctionOutput]
    public class UsernamesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }
    
}