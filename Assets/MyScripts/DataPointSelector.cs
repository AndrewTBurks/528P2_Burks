using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPointSelector : CAVE2Interactable
{
    public DataElement data;
    public GameObject label; 

    [SerializeField]
    bool showHoverOver = true;

    [SerializeField]
    bool showPointingOver = true;

    [SerializeField]
    float highlightScaler = 1.1f;

    [SerializeField]
    Mesh defaultMesh;

    [SerializeField]
    Material hoverOverMaterial;


    GameObject hoverOverHighlight;
    new MeshRenderer renderer;

    bool progressUp = true;
    float strobeProgress = 0.5f;

    Color originalHoverMatColor;
    private ExternalOperationManager controller;

    // Start is called before the first frame update
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
            hoverOverMaterial.SetColor("_Color", new Color(117 / 255f, 107 / 255f, 177 / 255f, 0.25f));
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

        controller = GameObject.Find("ExternalOperationManager").GetComponent<ExternalOperationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWandOverTimer();

        if((showHoverOver && wandTouching) || (showPointingOver && wandPointing))
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
            controller.SelectValue(data);
            Debug.Log("Point Selected: " + data.__key__);
        }
        //CAVE2PlayerIdentity playerID = (CAVE2PlayerIdentity)evt[0];
        //int wandID = (int)evt[1];
    }

    public void setData(DataElement d) {
        data = d;

        if (label != null) {
            label.GetComponent<TextMesh>().text = d.label;
        }


    }
}
