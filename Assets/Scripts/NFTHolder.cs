using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Unity.Models;

namespace ERC721ContractLibrary.Contracts.ERC721PresetMinterPauserAutoId.ContractDefinition
{
    public class NFTHolder : NetworkBehaviour
    {
        [SyncVar]
        public NFTTokenData NftTokenData;
        
        private Image[] images;
        private void Start()
        {
            images = GetComponentsInChildren<Image>();
            StartCoroutine(LoadNFT());
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                var transform1 = transform;
                if (transform1.position.y < 0)
                {
                    var position = transform1.position;
                    position = new Vector3(position.x, 0.5f, position.z);
                    transform1.position = position;
                }
            }
        }

        private IEnumerator LoadNFT()
        {
            if (NftTokenData == null)
            {
                Debug.LogError("NO NFT DATA");
                yield break;
            }

            yield return NftTokenData.DownloadImageSprite();

            foreach (var img in images)
            {
                img.sprite = NftTokenData.imageSprite;
            }
        }
    }
}