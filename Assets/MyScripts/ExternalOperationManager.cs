using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ExternalOperationManager : MonoBehaviour
{
    public string _url = "http://localhost:3333/api/external-operation/link-10";
    private UnityWebRequest request;

    public GameObject dataPointObject;

    public ExternalOperationData data;
    public float[,] extrema;

    public int[] range = { 0, 50 };

    public GameObject dataGroup;
    private TextMesh xLabel;
    private TextMesh yLabel;
    private TextMesh zLabel;
    

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
        xLabel = GameObject.Find("Axes/X/Label").GetComponent<TextMesh>();
        yLabel = GameObject.Find("Axes/Y/Label").GetComponent<TextMesh>();
        zLabel = GameObject.Find("Axes/Z/Label").GetComponent<TextMesh>();

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
            ProcessExtrema();
            PlotData();
            SelectValue(data.data[56]);
    }

    private void ProcessExtrema() {
        extrema = new float[data.fields.Length, 2];

        for (int i = 0; i < data.fields.Length; i++) {
            var vals = data.data.Select(d => d.values[i]);
            extrema[i, 0] = vals.Min();
            extrema[i, 1] = vals.Max();
            Debug.Log(data.fields[i] + " " + extrema[i, 0] + " " + extrema[i, 1]);
        }
    }

    private void PlotData() {
        var numExisting = dataGroup.transform.childCount;

        // update axis labels
        xLabel.text = string.Format("{0}\n[{1}, {2}]",
            data.fields[0], 
            extrema[0, 0], 
            extrema[0, 1]
        );

        yLabel.text = string.Format("{0}\n[{1}, {2}]",
            data.fields[1], 
            extrema[1, 0], 
            extrema[1, 1]
        );

        zLabel.text = string.Format("{0}\n[{1}, {2}]",
            data.fields[2], 
            extrema[2, 0], 
            extrema[2, 1]
        );


        for (int i = 0; i < data.data.Length; i++) {
            GameObject newObj;
            if (i >= numExisting) {
                // create new child
                newObj = Instantiate(dataPointObject);
                newObj.transform.parent = dataGroup.transform;
            } else {
                // reuse child
                newObj = dataGroup.transform.GetChild(i).gameObject;
            }

            newObj.name = "" + data.data[i].__key__;
            newObj.GetComponent<DataPointSelector>().setData(data.data[i]);
            // Debug.Log(newObj.GetComponent<DataPointSelector>().data);
            // Debug.Log(data.data[i]);
            // .data = data.data[i];

            // xyz for now
            newObj.transform.position = new Vector3(
                ScaleValue(data.data[i].values[0], 0),
                ScaleValue(data.data[i].values[1], 1),
                ScaleValue(data.data[i].values[2], 2)
            );
        }
    }

    private float ScaleValue(float value, int index) {
        return ((
            (value - extrema[index, 0])
            / (extrema[index, 1] - extrema[index, 0])
        ) * (range[1] - range[0])) + range[0];
    }

    public void SelectValue(DataElement el) {
        string payload = JsonUtility.ToJson(el);

        StartCoroutine(PostSelection(_url, payload));
    }

    IEnumerator PostSelection(string DataUrl, string jsonPayload)
    {   

        request = new UnityWebRequest(DataUrl, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError) 
            Debug.Log(request.error);
        else
            // data = JsonUtility.FromJson<ExternalOperationData>(request.downloadHandler.text);
            Debug.Log("posted data: " + request.responseCode);
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
    public float[] values;
}