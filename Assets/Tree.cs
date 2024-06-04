using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private Animator anim;
    private bool Clickable = false;

    bool mouseOver = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("PlayRandomAnimation", Random.Range(5, 10));

        if(Random.value <= 0.02f){
            // get child 0
            Invoke("SetCameraSpriteActive", Random.Range(5, 10));
        }
    }

    void SetCameraSpriteActive(){
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void PlayRandomAnimation(){
        anim.SetTrigger("Shrug");

        Invoke("PlayRandomAnimation", Random.Range(5, 10));
    }

    public void BecomeClickable(){
        Clickable = true;
        GetComponent<SpriteRenderer>().color = new Color(0.8f,1f,0.8f,1);
        StartCoroutine(Shake());

    }


    IEnumerator Shake(){
        float shakeAmount = 0.1f;
        float decreaseFactor = 1.0f;
        Vector3 originalPos = transform.position;
        while(shakeAmount > 0){
            transform.position = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeAmount -= Time.deltaTime * decreaseFactor;
            yield return null;
        }
        transform.position = originalPos;

        yield return new WaitForSeconds(mouseOver?0.1f:Random.Range(2, 4));
        StartCoroutine(Shake());
    }




    private void OnMouseDown()
    {
        if(Clickable){
            FoundObject.Instance.Activate();
            Clickable = false;

            StopAllCoroutines();

            Destroy(this.gameObject);
        }

        
    }

    void OnMouseEnter(){
        if(Clickable){
            StopAllCoroutines();
            StartCoroutine(Shake());
            mouseOver = true;
        }
    }
    void OnMouseExit(){
        if(Clickable){
            mouseOver = false;
        }
    }

    private void OnDrawGizmos()
    {
        if(Clickable){
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0.1f));
        }
    }
}
