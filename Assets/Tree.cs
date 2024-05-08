using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("PlayRandomAnimation", Random.Range(5, 10));
        
    }

    void PlayRandomAnimation(){
        anim.SetTrigger("Shrug");

        Invoke("PlayRandomAnimation", Random.Range(5, 10));
    }
}
