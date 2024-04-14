using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private BoxCollider2D bc;
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    
}
