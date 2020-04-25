using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ImageFetcher : MonoBehaviour
{
    public RawImage rawImage;
    public string _url = "localhost:3333/api/image/link-14";
    private UnityWebRequest request;
    // Start is called before the first frame update
    void Start(){
        StartCoroutine(DownloadImage(_url));
        rawImage.color = new Color(256, 256, 256, 0);
    }

    IEnumerator DownloadImage(string MediaUrl)
    {   
        request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError) 
            Debug.Log(request.error);
        else
            rawImage.color = new Color(256, 256, 256, 1);
            rawImage.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    public string url {
        set {
            // if (value != _url) {
                // abort old request
            if (!request.isDone) {
                request.Abort();
            }

            _url = value;
            StartCoroutine(DownloadImage(_url));
            // }
            
        }
        get {
            return _url;
        }
    }
}
