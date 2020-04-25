using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// based on HoverOverIndicator.cs in module-omicron
public class NodeInteraction : CAVE2Interactable
{
    private Node _info;
    public Node info {
        get { return _info; }
        set {
            label.GetComponent<TextMesh>().text = value.snippetInfo.name;

            _info = value;
        }
    }

    public GameObject label; 
    private DataflowManager df;

    // Start is called before the first frame update
    [SerializeField]
    bool showHoverOver = true;

    [SerializeField]
    bool showPointingOver = true;

    [SerializeField]
    float highlightScaler = 1.05f;

    [SerializeField]
    Mesh defaultMesh;

    [SerializeField]
    Material hoverOverMaterial;


    GameObject hoverOverHighlight;
    new MeshRenderer renderer;

    bool progressUp = true;
    float strobeProgress = 0.5f;

    Color originalHoverMatColor;


    // Use this for initialization
    void Start()
    {
        hoverOverHighlight = new GameObject("wandOverHighlight");
        hoverOverHighlight.transform.parent = transform;
        hoverOverHighlight.transform.position = transform.position;
        hoverOverHighlight.transform.rotation = transform.rotation;
        hoverOverHighlight.transform.localScale = Vector3.one * highlightScaler;

        if (defaultMesh == null)
        {
            defaultMesh = GetComponent<MeshFilter>().mesh;
        }
        hoverOverHighlight.AddComponent<MeshFilter>().mesh = defaultMesh;
        renderer = hoverOverHighlight.AddComponent<MeshRenderer>();

        if (hoverOverMaterial == null)
        {
            // Create a basic highlight material
            hoverOverMaterial = new Material(Shader.Find("Standard"));
            hoverOverMaterial.SetColor("_Color", new Color(0, 1, 1, 0.25f));
            hoverOverMaterial.SetFloat("_Mode", 3); // Transparent
            hoverOverMaterial.SetFloat("_Glossiness", 0);
        }
        else
        {
            hoverOverMaterial = new Material(hoverOverMaterial);
        }
        originalHoverMatColor = hoverOverMaterial.color;
        renderer.material = hoverOverMaterial;

        renderer.enabled = false;

        df = GameObject.Find("RequestManager").GetComponent<DataflowManager>();
        // label = this.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWandOverTimer();

        if((showHoverOver && wandTouching) || (showPointingOver && wandPointing))
        {
            hoverOverMaterial.color = originalHoverMatColor;
            renderer.enabled = true;
            label.SetActive(true);
        }
        else
        {
            renderer.enabled = false;
            label.SetActive(false);
        }
    }
    private void OnWandButtonDown(CAVE2.WandEvent evt)
    {
        if (info.hasImage && evt.button == CAVE2.Button.Button3) {
            Debug.Log("Selected at: " + _info.linkID);
            df.UpdateChartImage(info.linkID);
        }
        //CAVE2PlayerIdentity playerID = (CAVE2PlayerIdentity)evt[0];
        //int wandID = (int)evt[1];
        CAVE2.Button button = (CAVE2.Button)evt.button;


        Debug.Log("OnWandButtonDown: " + button);
    }
}
