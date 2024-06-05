using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoundObject : MonoBehaviour
{
    public static FoundObject Instance;
    public bool ActiveStatus = false;
    public Image blocker;
    public Image itemImage;
    public TextMeshProUGUI youFoundText;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescText;
    public AnimationCurve curve;

    public AudioSource audioSource;

    public Transform indicator;
    public Sprite filledBox;


    void Awake(){
        Instance = this;
        FillBoxes();
    }

    public void FillBoxes(){
        if(ScoreHolder.Instance.foundObjects == 0){
            indicator.gameObject.SetActive(false);
        }else{
            for(int i=0; i<ScoreHolder.Instance.foundObjectSprites.Count; i++){
                if(ScoreHolder.Instance.foundObjects > i){
                    indicator.GetChild(i).GetChild(0).GetComponent<Image>().sprite = filledBox;
                    indicator.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(true);

                }
            }
        }
    }
    public void Activate()
    {
        itemImage.sprite = ScoreHolder.Instance.foundObjectSprites[ScoreHolder.Instance.foundObjects];
        itemNameText.text = ScoreHolder.Instance.foundObjectNames[ScoreHolder.Instance.foundObjects];
        itemDescText.text = ScoreHolder.Instance.foundObjectsDescriptions[ScoreHolder.Instance.foundObjects];

        ScoreHolder.Instance.foundObjects++;
        FillBoxes();
        StartCoroutine(Process());
    }

    public IEnumerator Process(){
        ActiveStatus = true;
        float duration = 0.5f;
        float t = 0;
        Color startColor = blocker.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.85f);
        while(t < duration)
        {
            t += Time.deltaTime;
            blocker.color = Color.Lerp(startColor, endColor, curve.Evaluate(t/duration));
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        youFoundText.gameObject.SetActive(true);
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        itemImage.gameObject.SetActive(true);
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        itemNameText.gameObject.SetActive(true);
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        itemDescText.gameObject.SetActive(true);
        audioSource.Play();

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        audioSource.Play();

        yield return new WaitForSeconds(0.5f);
        youFoundText.gameObject.SetActive(false);
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        itemImage.gameObject.SetActive(false);
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        itemNameText.gameObject.SetActive(false);
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        itemDescText.gameObject.SetActive(false);
        audioSource.Play();

        t = 0;
        startColor = blocker.color;
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        while(t < duration)
        {
            t += Time.deltaTime;
            blocker.color = Color.Lerp(startColor, endColor, curve.Evaluate(t/duration));
            yield return null;
        }

        ActiveStatus = false;



    }

    
}
