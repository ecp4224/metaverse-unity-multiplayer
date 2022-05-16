using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class LoadImageFromURL : MonoBehaviour
{
    // Start is called before the first frame update
    public string BaseUrl;
    public GameObject loading;
    public RawImage Image;

    void Start()
    {
        //StartCoroutine(LoadImage(BaseUrl));
    }

    /*IEnumerator LoadImage(string ImageUrl)
    {
        WWW www = new WWW(ImageUrl);
        loading.SetActive(true);
        yield return www;

        {
            if (!File.Exists(Application.persistentDataPath + ImageName))
            {
                //if internet available
                if (www.error == null)
                {
                    //when image downloaded..
                    loading.SetActive(false);
                    Texture2D texture = www.texture;
                    Image.texture = texture;
                    byte[] dataByte = texture.EncodeToPNG();
                    File.WriteAllBytes(Application.persistentDataPath + ImageName, dataByte);
                    Debug.Log("Image Saved");
                }
                //if internet is not available
                else
                {
                    Debug.Log("404")
                }
            }
            else if (!File.Exists(Application.persistentDataPath + ImageName))
            {
                loading.SetActive(false);
                byte[] UploadByte = FileSecurity.ReadAllBytes(Application.persistentDataPath + ImageName);
                Texture2D texture = new Texture2D(10, 10);
                texture.LoadImage(UploadByte);
                Image.texture = texture;
                Debug.Log("Image Loaded");
            }
        }
    }*/
}

