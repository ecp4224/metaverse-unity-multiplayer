using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using DefaultNamespace;
using ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition;
using ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition.UI;
using Mirror;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using RootMotion.FinalIK;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.XR;
using WalletConnectSharp.Unity.Models;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Player : BindableNetworkBehavior
{
    public static Player LocalPlayer
    {
        get;
        private set;
    }

    public static string LocalUsername;
    
    #region Local Player Items

    [BindComponentInChildren, NonSerialized]
    public WalletUI WalletCanvas;

    public StringVariable addressText;
    public StringVariable usernameText;
    public StringVariable balanceText;
    public StringVariable transactionToName;
    public StringVariable transactionToAddress;
    public StringVariable transactionToAmount;
    public StringVariable errorMessage;

    public GameObject head;

    [BindComponent()]
    private ThirdPersonController _controller;

    [BindComponent()]
    private StarterAssetsInputs _inputs;

    [BindComponent()]
    private PlayerInput _pInput;

    [BindComponent()]
    private CharacterController _character;
    
    [BindComponent]
    [NonSerialized]
    public CameraChange CameraChange;

    [BindComponent]
    private FullBodyBipedIK _ik;

    [Inject]
    private PlayerCameraSetup CameraSetup;

    [BindComponent]
    private Animator _animator;

    public Transform CameraRoot;
    #endregion

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    [SyncVar, NonSerialized]
    public string PlayerAddress;

    [SyncVar, NonSerialized]
    public string Username;

    public Transform FPSTarget;
    
    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    [SyncVar]
    private float animationBlend;
    [SyncVar]
    private float inputMag;
    [SyncVar]
    private bool isGrounded;
    [SyncVar]
    private bool freeFall;
    [SyncVar]
    private bool jumping;

    [SyncVar]
    private float targetWeight;
    private float startWeight;

    [Inject]
    private ServerWallet _serverWallet;

    private bool isLerping;
    private float startTime;
    public float ikDuration = 0.5f;
    [SerializeField] private double interactDistance;

    public static bool IsUsingVR()
    {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        _ik.solver.leftHandEffector.positionWeight = 0;
        _ik.solver.leftHandEffector.rotationWeight = 0;
        _ik.solver.leftShoulderEffector.positionWeight = 0;
        
        if (!isLocalPlayer)
        {
            _controller.enabled = false;
            _inputs.enabled = false;
            _pInput.enabled = false;
            CameraChange.enabled = false;
            _character.enabled = false;

            AssignAnimationIDs();
        }
        else
        {
            LocalPlayer = this;
            
            CameraSetup.LocalPlayer = this;
            CameraChange.StartCamera();

            StartCoroutine(RestartInput());

            if (Wallet.Current.Connected)
            {
                CmdSetAddressAndUsername(Wallet.Current.Address, LocalUsername);
            }
            else
            {
                Wallet.Current.WalletConnected += (_, wallet) => CmdSetAddressAndUsername(wallet.Address, LocalUsername);
            }

            StartCoroutine(WalletUpdateTask());
        }
    }
    
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            var selfTransform = transform;
            var nearestPlayer = FindObjectsOfType<Player>()
                .Where(p => p != this)
                .Where(p => (p.transform.position - selfTransform.position).magnitude <= interactDistance)
                .OrderBy(p => (p.transform.position - selfTransform.position).magnitude)
                .FirstOrDefault();

            WalletCanvas.sendEthButton.interactable = nearestPlayer != null;
            
            //Check for vending machines near me
            var nearestVendingMachine = FindObjectsOfType<VendingMachine>()
                .Where(v => (v.transform.position - selfTransform.position).magnitude <= interactDistance)
                .OrderBy(v => (v.transform.position - selfTransform.position).magnitude)
                .FirstOrDefault();
            
            var nearestNFT = FindObjectsOfType<NFTHolder>()
                .Where(v => (v.transform.position - selfTransform.position).magnitude <= interactDistance)
                .OrderBy(v => (v.transform.position - selfTransform.position).magnitude)
                .FirstOrDefault();

            WalletCanvas.buyButton.interactable = nearestVendingMachine != null;

            WalletCanvas.dropNFT.interactable = Wallet.Current.NFTCount > 0;

            WalletCanvas.pickupNFT.interactable = nearestNFT != null;

            //TODO Grab NFT Button interactable
        }
    }

    public void PickupNearestNFT()
    {        
        var selfTransform = transform;
        var nearestNFT = FindObjectsOfType<NFTHolder>()
            .Where(v => (v.transform.position - selfTransform.position).magnitude <= interactDistance)
            .OrderBy(v => (v.transform.position - selfTransform.position).magnitude)
            .FirstOrDefault();

        if (nearestNFT == null)
            return;
        
        WalletCanvas.ShowLoader();
        
        _serverWallet.CmdPickupNFT(Username, nearestNFT.NftTokenData);
    }

    [TargetRpc]
    public void TargetCompletePickup(NetworkConnection target)
    {
        WalletCanvas.ShowScreen("successPickup");
        WalletCanvas.HideLoader();
    }
    
    [TargetRpc]
    public void TargetFailPickup(NetworkConnection target)
    {
        WalletCanvas.ShowScreen(WalletCanvas.defaultPage);
        WalletCanvas.HideLoader();
    }

    private IEnumerator WalletUpdateTask()
    {
        while (isActiveAndEnabled)
        {
            if (!Wallet.Current.Connected || string.IsNullOrWhiteSpace(PlayerAddress))
            {
                yield return new WaitForSeconds(3);
                continue;
            }

            addressText.Value = PlayerAddress.Substring(0, 4) + "..." +
                                            PlayerAddress.Substring(PlayerAddress.Length - 4, 4);

            usernameText.Value = Username;

            var balanceCheckTask = Wallet.Current.Web3.Eth.GetBalance.SendRequestAsync(PlayerAddress);

            yield return new WaitForTaskResult<HexBigInteger>(balanceCheckTask);

            var balance = balanceCheckTask.Result;

            var etherAmount = Web3.Convert.FromWei(balance);

            balanceText.Value = etherAmount + " ETH";
            
            //Update all NFTs
            Wallet.Current.RefreshNFTsAsync();
            
            //TODO
            //Update all tokens
            //Wallet.Current.RefreshTokensAsync();

            yield return new WaitForSeconds(5);
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            _animator.SetBool(_animIDGrounded, isGrounded);
            _animator.SetFloat(_animIDSpeed, animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMag);
            _animator.SetBool(_animIDJump, jumping);
            _animator.SetBool(_animIDFreeFall, freeFall);
        }

        float current = _ik.solver.leftHandEffector.positionWeight;
        if (Math.Abs(current - targetWeight) > 0.03)
        {
            if (!isLerping)
            {
                isLerping = true;
                startTime = Time.time;
                startWeight = current;
            }
            
            float newCurrent = Mathf.Lerp(startWeight, targetWeight, (Time.time - startTime) / ikDuration);
            
            _ik.solver.leftHandEffector.positionWeight = newCurrent;
            _ik.solver.leftHandEffector.rotationWeight = newCurrent;
            _ik.solver.leftShoulderEffector.positionWeight = newCurrent;
        }
        else
        {
            isLerping = false;
            
            _ik.solver.leftHandEffector.positionWeight = targetWeight;
            _ik.solver.leftHandEffector.rotationWeight = targetWeight;
            _ik.solver.leftShoulderEffector.positionWeight = targetWeight;
            WalletCanvas.gameObject.SetActive(targetWeight > 0f);
            _inputs.SetCursorState(targetWeight <= 0f && isLocalPlayer);
        }

        if (isLocalPlayer)
        {
            if (!CameraChange.thirdPersonView)
            {
                CameraChange.FirstCam.transform.SetPositionAndRotation(FPSTarget.position, FPSTarget.rotation);
            }
        }
    }

    [Command]
    public void CmdSetAnimationValues(float animationBlend, float inputMag)
    {
        this.animationBlend = animationBlend;
        this.inputMag = inputMag;
    }

    [Command]
    public void CmdSetJumpBool(bool jumping)
    {
        this.jumping = jumping;
    }

    [Command]
    public void CmdSetFreefall(bool freefall)
    {
        this.freeFall = freefall;
    }

    [Command]
    public void CmdSetGrounded(bool grounded)
    {
        this.isGrounded = grounded;
    }

    [Command]
    public void CmdSetAddressAndUsername(string address, string username)
    {
        PlayerAddress = address;
        Username = username;
    }

    private IEnumerator RestartInput()
    {
        _pInput.enabled = false;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _pInput.enabled = true;
    }

    [Command]
    public void CmdShowWrist()
    {
        targetWeight = 1f;
    }

    [Command]
    public void CmdHideWrist()
    {
        targetWeight = 0f;
    }

    public void SendDefaultEthAction()
    {
        SendEthToNearestPlayer();
    }

    public void UseNearestVendingMachine()
    {
        var selfTransform = transform;
        //Check for vending machines near me
        var nearestVendingMachine = FindObjectsOfType<VendingMachine>()
            .Where(v => (v.transform.position - selfTransform.position).magnitude <= interactDistance)
            .OrderBy(v => (v.transform.position - selfTransform.position).magnitude)
            .FirstOrDefault();

        if (nearestVendingMachine != null)
        {
            nearestVendingMachine.CmdDoTransaction(Username);
        }
        else
        {
            WalletCanvas.ShowScreen("main");
        }
    }

    public void SendEthToNearestPlayer(decimal amount = 0.1m)
    {
        var selfTransform = transform;
        var nearestPlayer = FindObjectsOfType<Player>()
            .Where(p => p != this)
            .Where(p => (p.transform.position - selfTransform.position).magnitude <= interactDistance)
            .OrderBy(p => (p.transform.position - selfTransform.position).magnitude)
            .FirstOrDefault();

        if (nearestPlayer != null)
        {
            //TODO Show confirmation screen
            transactionToName.Value = nearestPlayer.Username;
            transactionToAddress.Value = nearestPlayer.PlayerAddress.Substring(0, 4) + "..." +
                                         nearestPlayer.PlayerAddress.Substring(nearestPlayer.PlayerAddress.Length - 4, 4);
            transactionToAmount.Value = amount + " ETH";
            
            WalletCanvas.ShowConfirm(delegate
            {
                StartCoroutine(SendEth(nearestPlayer, amount));
            });
        }
        else
        {
            WalletCanvas.ShowScreen("main");
        }
    }

    private IEnumerator SendEth(Player to, decimal amount)
    {
        var transactionTask = Wallet.Current.Web3.Eth.GetEtherTransferService()
            .TransferEtherAndWaitForReceiptAsync(to.PlayerAddress, amount);
        
        WalletCanvas.ShowLoader();
        
        yield return new WaitForTaskResult<TransactionReceipt>(transactionTask);

        if (transactionTask.IsFaulted)
        {
            errorMessage.Value = "Error:\n" + transactionTask.Exception.Message;
            WalletCanvas.ShowScreen("error");
        }
        else
        {
            var transactionReceipt = transactionTask.Result;

            WalletCanvas.ShowScreen("success");
            WalletCanvas.HideLoader();
        }
    }

    public void ShowNFTList()
    {
        WalletCanvas.ShowScreen("dropNFT");
    }

    public void SendNFT(NFTTokenData nft, string nftAddress)
    {
        WalletCanvas.ShowLoader();
        _serverWallet.CmdCommitNFT(nft, transform.position + (transform.forward * 3f));

        StartCoroutine(SendNFTOnchain(nftAddress, nft));
    }

    private void CompleteDropNFT()
    {
        WalletCanvas.ShowScreen("successPickup");
        WalletCanvas.HideLoader();
    }
    
    private IEnumerator SendNFTOnchain(string nftAddress, NFTTokenData nft)
    {
        var transferHandler = Wallet.Current.Web3.Eth.GetContractTransactionHandler<TransferFromFunction>();
        var transferCall = new TransferFromFunction()
        {
            From = PlayerAddress,
            To = _serverWallet.Address,
            TokenId = BigInteger.Parse(nft.tokenId)
        };

        var transactionTask = transferHandler.SendRequestAndWaitForReceiptAsync(nftAddress, transferCall);

        yield return new WaitForTaskResult<TransactionReceipt>(transactionTask);
            
        CompleteDropNFT();
    }
}
