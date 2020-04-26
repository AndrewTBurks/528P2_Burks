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
    public bool isSelected;
    private DataflowManager df;

    // Start is called before the first frame update
    [SerializeField]
    bool showHoverOver = true;

    [SerializeField]
    bool showPointingOver = true;

    [SerializeField]
    float highlightScaler = 1.5f;

    [SerializeField]
    Mesh defaultMesh;

    [SerializeField]
    Material hoverOverMaterial;


    GameObject hoverOverHighlight;
    new MeshRenderer renderer;

    Color originalHoverMatColor;


    // Use this for initialization
    void Start()
    {
        isSelected = false;

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
            hoverOverMaterial.SetColor("_Color", new Color(117 / 255f, 107 / 255f, 177 / 255f, 0.5f));
            hoverOverMaterial.SetFloat("_Mode", 3); // Transparent
            hoverOverMaterial.SetFloat("_Glossiness", 0);
        }
        else
        {
            hoverOverMaterial = new Material(hoverOverMaterial);
        }
        originalHoverMatColor = hoverOverMaterial.color;
        renderer.material = hoverOverMaterial;
        renderer.material.color = new Color(117 / 255f, 107 / 255f, 177 / 255f, 0.5f);

        renderer.enabled = false;

        df = GameObject.Find("RequestManager").GetComponent<DataflowManager>();
        // label = this.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWandOverTimer();

        if((showHoverOver && wandTouching) || (showPointingOver && wandPointing) || isSelected)
        {
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
        if (evt.button == CAVE2.Button.Button3) {
            if (info.hasImage) {
                Debug.Log("Image Selected at: " + _info.linkID);
                df.UpdateChartImage(info.linkID, this);

            } else if (info.externalOperation) {
                Debug.Log("ExtOp Selected at: " + _info.linkID);
                df.UpdateSelectedExternalOperation(info.linkID, this);
            }
        }
        //CAVE2PlayerIdentity playerID = (CAVE2PlayerIdentity)evt[0];
        //int wandID = (int)evt[1];
    }
}
