using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionController : MonoBehaviour
{

    public Transform cnv;
    int page = 0;

    public AudioSource aSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Presentation());
    }

    // Update is called once per frame
    private IEnumerator Presentation()
    {

        
        
        yield return new WaitUntil(()=>Input.GetMouseButtonDown(0));
        aSource.Play();
        yield return null;
        cnv.GetChild(page).gameObject.SetActive(false);
        page++;



        if(page == cnv.childCount){



            SceneManager.LoadScene("LevelSelect");
            
        }else{
            cnv.GetChild(page).gameObject.SetActive(true);
            yield return null;
            StartCoroutine(Presentation());
        }
    }
}
