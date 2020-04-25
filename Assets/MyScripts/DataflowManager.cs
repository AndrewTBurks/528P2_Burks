using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;


public class DataflowManager : MonoBehaviour
{
    public string api_root = "localhost:3333/api";
    public NodeResponse nodes;
    public GraphDrawing listener;
    public RawImage chartView;
    public ExternalOperationManager extOp;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FetchNodes());
    }

    IEnumerator FetchNodes()
    {   
        UnityWebRequest request = UnityWebRequest.Get(api_root + "/nodes");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError) 
            Debug.Log(request.error);
        else
            nodes = JsonUtility.FromJson<NodeResponse>(request.downloadHandler.text);

            listener.CreateGraph(nodes.layout);
            // Debug.Log(nodes.layout);
    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateChartImage(string linkID) {
        string newurl = api_root + "/image/" + linkID;
        // if (chartView.GetComponent<ImageFetcher>().url != newurl) {
            chartView.GetComponent<ImageFetcher>().url = newurl;
        // }
    }

    public void UpdateSelectedExternalOperation(string linkID) {
        string newurl = api_root + "/external-operation/" + linkID;
        // if (extOp.url !== newurl) {
            extOp.url = newurl;
        // }
    }
}

[Serializable]
public class NodeResponse {
    public string message;
    public Node[] layout;
}

[Serializable]
public class Node {
    public int px;
    public int py;
    public int x;
    public int y;
    public int width;
    public int height;
    public string linkID;
    public string snippetID;
    public SnippetInfo snippetInfo;

    public bool hasResult;
    public bool hasImage;
    public bool requiresExternal;
}

[Serializable]
public class SnippetInfo {
    public string id;
    public string text; // code src
    public string name;
    public string desc;
    public string type;
}
