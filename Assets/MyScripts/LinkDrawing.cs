using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkDrawing : MonoBehaviour
{
    private Vector3 _start;
    private Vector3 _end;

    public GameObject cylinder;
    // Start is called before the first frame update
    void Start()
    {
        cylinder.transform.localPosition = new Vector3(0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector3 start, Vector3 end) {
        _start = start;
        _end = end;

        this.gameObject.transform.localPosition = new Vector3(0,0, -1) + Vector3.Lerp(start, end, 0.5f);

        this.gameObject.transform.up = end-start;

        Vector3 newScale = this.cylinder.transform.localScale;
        newScale.y = Vector3.Distance(start,end) / 2;
        this.cylinder.transform.localScale = newScale;

        // this.gameObject.transform.localScale = new Vector3(1, 1, Vector3.Distance(start, end) / 2f);
        // this.gameObject.transform.LookAt(_end);
    }
}
