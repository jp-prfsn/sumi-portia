using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    public float bobAmount = 0.1f;
    public float bobSpeed = 1;

    public int order;
    bool up;

    // Start is called before the first frame update
    void Start()
    {
        
        Invoke("StartIt", (float)order/4);
    }

    void StartIt(){
        if(this.gameObject.activeSelf){
            StartCoroutine(bob());
        }

    }


    private IEnumerator bob()
    {
        if(up){
            transform.position += new Vector3(0,bobAmount,0);
            up = false;
        }else{
            transform.position -= new Vector3(0,bobAmount,0);
            up = true;
        }
        yield return new WaitForSeconds(bobSpeed);
        if(this.gameObject.activeSelf){
            StartCoroutine(bob());
        }
    }
}
