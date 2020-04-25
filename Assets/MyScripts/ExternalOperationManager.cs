using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExternalOperationManager : MonoBehaviour
{
    public string _url = "http://localhost:3333/api/external-operation/link-10";
    private UnityWebRequest request;

    public ExternalOperationData data;
    public string url {
        set {
            if (value != _url) {
                // abort old request
                if (!request.isDone) {
                    request.Abort();
                }

                _url = value;
                StartCoroutine(FetchInformation(_url));
            }
            
        }
        get {
            return _url;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FetchInformation(_url));
    }

    IEnumerator FetchInformation(string DataUrl)
    {   
        request = UnityWebRequest.Get(DataUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError) 
            Debug.Log(request.error);
        else
            data = JsonUtility.FromJson<ExternalOperationData>(request.downloadHandler.text);
            Debug.Log("fetched data: " + data.data.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class ExternalOperationData {
    public string name;
    public string operation;
    public string[] fields;

    public string dataFormat; // xy, xyz, or latlng (for now)
    public DataElement[] data;
}

[System.Serializable]
public class DataElement {
    public int __key__;
    public int[] values;
}