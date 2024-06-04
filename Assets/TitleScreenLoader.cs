using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreenLoader : MonoBehaviour
{
    public List<GameObject> ObjectsToReveal;
    public Image fader;

    public TextMeshProUGUI titleText;


    

    void Start()
    {
        foreach (var obj in ObjectsToReveal)
        {
            obj.SetActive(false);
        }
        StartCoroutine(LoadTitleScreen());
    }

    private IEnumerator LoadTitleScreen()
    {
        yield return new WaitForSeconds(1);
        foreach (var obj in ObjectsToReveal)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(0.6f);
        }
        StartCoroutine(BlinkText());
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        // PlaySound
        GetComponent<AudioSource>().Play();
        // increase the alpha of the fader
        while (fader.color.a < 1)
        {
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, fader.color.a + 0.25f);
            yield return new WaitForSeconds(0.25f);
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions");
    }

    private IEnumerator BlinkText(){
        while(true){
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0);
            yield return new WaitForSeconds(0.5f);
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 1);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
