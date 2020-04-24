using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWarpController : MonoBehaviour
{
    public GameObject player;
    public GameObject start;

    // Start is called before the first frame update
    void Start()
    {
        JumpTo(start);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JumpTo(GameObject loc)
    {
        player.transform.position = loc.transform.position;
        player.transform.rotation = loc.transform.rotation;

        Debug.Log("Jump to:" + loc.transform.position);
    }
}
