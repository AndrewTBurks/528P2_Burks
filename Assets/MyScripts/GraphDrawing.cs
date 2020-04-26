using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDrawing : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject node;
    public GameObject link;
    public Dictionary<string, Color> colors;

    void Start()
    {
        colors = new Dictionary<string, Color>();
        colors.Add("gen", new Color(
            102 / 255f,
            194 / 255f,
            165 / 255f
        ));

        colors.Add("data", new Color(
            141 / 255f,
            160 / 255f,
            203 / 255f
        ));
        
        colors.Add("draw", new Color(
            252 / 255f,
            141 / 255f,
            98 / 255f
        ));

        // 179,226,205
        colors.Add("gen-link", new Color(
            179 / 255f,
            226 / 255f,
            205 / 255f
        ));

        // 203,213,232
        colors.Add("data-link", new Color(
            203 / 255f,
            213 / 255f,
            232 / 255f
        ));
        
        // 253,205,172
        colors.Add("draw-link", new Color(
            253 / 255f,
            205 / 255f,
            172 / 255f
        ));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateGraph(Node[] nodes)
    {
        foreach (Node n in nodes)
        {
            GameObject newnode = Instantiate(node);
            newnode.transform.parent = this.gameObject.transform;
            newnode.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

            newnode.transform.localPosition = new Vector3(n.x / 100f, n.y / 100f, 0);
            newnode.name = n.linkID;

            newnode.GetComponent<Renderer>().material.color = colors[n.snippetInfo.type];
            newnode.GetComponent<NodeInteraction>().info = n; // save info
            
            GameObject newlink = Instantiate(link);
            newlink.transform.parent = this.gameObject.transform;
            newlink.name = n.linkID + "-pipe";

            newlink
                .GetComponent<LinkDrawing>()
                .SetPosition(
                    new Vector3(n.px / 100f, n.py / 100f, 0),
                    new Vector3(n.x / 100f, n.y / 100f, 0)
                    );

            newlink.GetComponent<LinkDrawing>()
                .cylinder.GetComponent<Renderer>()
                    .material.color = colors[n.snippetInfo.type + "-link"];
        }

        this.gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.position - GameObject.Find("CAVE2-PlayerController").transform.position);

    }
}
