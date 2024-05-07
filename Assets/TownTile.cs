using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TownTile : MonoBehaviour
{
    public int levelIndex;
    public bool isUnlocked = false;
    public GameObject lockedSprite;
    public GameObject hover;
    public TextMeshProUGUI levelText;

    public void Setup()
    {

        if(ScoreHolder.Instance.levelUnlocked[levelIndex] == 1){
            isUnlocked = true;
            ScoreHolder.Instance.currentLevel = levelIndex;
            lockedSprite.SetActive(false);
        }
        else{
            isUnlocked = false;
        }

        levelText.text = (levelIndex + 1).ToString();
    }

 
    private void OnMouseDown()
    {
        if(isUnlocked){
            
            StartCoroutine(EnterLevel());
            TownGenerator.Instance.PlaySuccess();
        }else{
            StartCoroutine(Shake());
            TownGenerator.Instance.PlayFailure();
        }
    }

    void OnMouseOver(){
        if(isUnlocked){
            //TownGenerator.Instance.WitchSelector.position = new Vector3(transform.position.x, transform.position.y, -1);
            StartCoroutine(TownGenerator.Instance.MoveWitches(transform.position));
            hover.SetActive(true);
        }
    }

    void OnMouseExit(){
        hover.SetActive(false);
    }

    IEnumerator EnterLevel(){
        ScoreHolder.Instance.currentLevel = levelIndex;
        transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        yield return new WaitForSeconds(0.1f);
        transform.localScale = new Vector3(1, 1, 1);
        yield return new WaitForSeconds(0.2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Chat");
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
    }
}
