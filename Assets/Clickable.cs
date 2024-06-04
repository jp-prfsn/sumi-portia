using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnMouseDown()
    {
        DoThing();
    }

    // Update is called once per frame
    public void DoThing()
    {
        Debug.Log("Clicked.");
        return;
    }

    public void OnDrawGizmos()
    {
        // Draw a box around the clickable object
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
    }
}
