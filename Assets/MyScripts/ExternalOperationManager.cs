using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
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
    

    public GameObject optionsMenu;
    public GameObject menuSectionObject;
    public GameObject menuOptionObject;
    private int sizeAttribute;

    public string url {
        set {
            if (value != _url) { 
                // abort old request
                if (request != null && !request.isDone) {
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

        // StartCoroutine(FetchInformation(_url));
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
            CreateMenuOptions();
            PlotData();
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

    private void CreateMenuOptions() {
        foreach (Transform child in optionsMenu.transform.Find("MenuSections")) {
            GameObject.Destroy(child.gameObject);
        }

        int numSubMenus = 1;
        Debug.Log(optionsMenu.GetComponent<OMenu>().menuItems.Length);
        System.Array.Resize(ref optionsMenu.GetComponent<OMenu>().menuItems, data.fields.Length * numSubMenus);
        Debug.Log(optionsMenu.GetComponent<OMenu>().menuItems.Length);
        // myArray[myArray.GetUpperBound(0)] = newValue;
        
        // create menu section
        var section = Instantiate(menuSectionObject);
        // menuSectionObject.transform.localPosition.y = -50;
        section.transform.Find("MenuSectionLabel").gameObject.GetComponent<Text>().text = "Sphere Size & Color";
        section.transform.parent = optionsMenu.transform.Find("MenuSections");
        // section.transform.localPosition = Vector3.zero;
        section.transform.localPosition = new Vector3(.0325f, 0.24f, 0);
        section.transform.localScale = new Vector3(1, 1, 1);

        for (int i = 0; i < data.fields.Length; i++) {
            Debug.Log("Create Option " + data.fields[i]);
            var option = Instantiate(menuOptionObject);
            option.transform.Find("Label").gameObject.GetComponent<Text>().text = data.fields[i];

            option.transform.parent = section.transform;
            option.transform.localScale = new Vector3(1, 1, 1);
            option.transform.localPosition = new Vector3(0, i * -0.08f, 0);

            option.GetComponent<Toggle>().group = option.GetComponentInParent<ToggleGroup>(); 

            optionsMenu.GetComponent<OMenu>().menuItems[i] = (
                option.GetComponent<Toggle>()
            );

            var attrIndex = i;
            option.GetComponent<Toggle>().onValueChanged.AddListener((bool isOn) => {
                if (isOn)
                    SelectSize(attrIndex, isOn);
            });

            // option.transform.localPosition = new Vector3(0, 7.5f * (i + 1), 0);
        }
    }

    private void SelectSize(int number, bool isSelected) {
        Debug.Log("Toggle: " + data.fields[number] + " " + isSelected );
        sizeAttribute = number;
        ScaleDataPoints();
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

        int i = 0;
        for (i = 0; i < data.data.Length; i++) {
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

        for (i = i; i < numExisting; i++) {
            Destroy(dataGroup.transform.GetChild(i).gameObject);
        }
    }

    private float ScaleValue(float value, int index) {
        return ((
            (value - extrema[index, 0])
            / (extrema[index, 1] - extrema[index, 0])
        ) * (range[1] - range[0])) + range[0];
    }

    private void ScaleDataPoints() {
        var numExisting = dataGroup.transform.childCount;

        for (var ind = 0; ind < numExisting; ind++) {
            var child = dataGroup.transform.GetChild(ind);
            var d = child.GetComponent<DataPointSelector>().data;

            float scale = System.Math.Max(ScaleValue(d.values[sizeAttribute], sizeAttribute) / 12.5f, 0.1f);
            child.transform.localScale = 
                new Vector3(scale, scale, scale);
            
            float[] colorVals = {1, 1, 1};

            for (var c = 0; c < 3;  c++) {
                if (c != sizeAttribute) {
                    colorVals[c] = 1 - (scale / 2f);
                }
            }


            child.GetComponent<Renderer>().material.color = new Color(
                colorVals[0], 
                colorVals[1], 
                colorVals[2]
            );

            child.transform.localScale = new Vector3(scale, scale, scale);
            child.transform.Find("DataLabel").localScale = new Vector3(
                0.1f / scale, 
                0.1f / scale, 
                0.1f / scale
            );
            
            child.transform.Find("DataLabel").localPosition = new Vector3(0, scale / 2 + 0.1f, 0);
        }
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
    public string label;
}