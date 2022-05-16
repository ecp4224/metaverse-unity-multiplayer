using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace WalletConnectSharp.Unity.Models
{
    [Serializable]
    public class NFTTokenData
    {
        public string name;
        public string image;
        public string description;
        public string tokenId;

        private Sprite _cache;

        public Sprite imageSprite
        {
            get
            {
                if (_cache == null)
                {
                    Debug.LogWarning("You must run `yield return token.DownloadImageSprite()` first");
                }

                return _cache;
            }
        }

        public IEnumerator DownloadImageSprite()
        {
            if (_cache == null)
            {
                while (true)
                {
                    if (image == null)
                        break;
                    
                    if (image.StartsWith("ipfs://"))
                    {
                        image = image.Replace("ipfs://", "https://ipfs.io/ipfs/");
                    }

                    UnityWebRequest www = UnityWebRequestTexture.GetTexture(image);

                    yield return www.SendWebRequest();

                    if (www.isNetworkError)
                    {
                        throw new IOException(www.error);
                    }
                    else
                    {
                        Texture2D imageTexture = null;
                        try
                        {
                            imageTexture = DownloadHandlerTexture.GetContent(www);
                        }
                        catch (InvalidOperationException e)
                        {
                            Debug.LogError("Got network error for URL " + image + " : " + e.Message +
                                           ". Will retry in 10 seconds");
                        }

                        if (imageTexture == null)
                        {
                            yield return new WaitForSeconds(10);
                            continue;
                        }

                        Rect rect = new Rect(0, 0, imageTexture.width, imageTexture.height);
                        Sprite imageSprite = Sprite.Create(imageTexture, rect, new Vector2(0.5f, 0.5f), 100);

                        _cache = imageSprite;
                        break;
                    }
                }
            }
        }

        protected bool Equals(NFTTokenData other)
        {
            return name == other.name && image == other.image && description == other.description && tokenId == other.tokenId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NFTTokenData) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, image, description, tokenId);
        }

        private sealed class NftTokenDataEqualityComparer : IEqualityComparer<NFTTokenData>
        {
            public bool Equals(NFTTokenData x, NFTTokenData y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.name == y.name && x.image == y.image && x.description == y.description && x.tokenId == y.tokenId;
            }

            public int GetHashCode(NFTTokenData obj)
            {
                return HashCode.Combine(obj.name, obj.image, obj.description, obj.tokenId);
            }
        }

        public static IEqualityComparer<NFTTokenData> NftTokenDataComparer { get; } = new NftTokenDataEqualityComparer();
    }
}