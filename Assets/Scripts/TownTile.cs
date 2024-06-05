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

        bool isFantasyIsland = levelIndex == TownGenerator.Instance.cols * TownGenerator.Instance.rows;

        if(this.levelIndex <= ScoreHolder.Instance.levelsUnlocked || ScoreHolder.Instance.gameState == GameStates.PortiaMissing || (ScoreHolder.Instance.gameState == GameStates.LivingInFantasy && isFantasyIsland)){
            isUnlocked = true;
            ScoreHolder.Instance.currentLevel = levelIndex;
            lockedSprite.SetActive(false);
        }
        else{
            isUnlocked = false;
        }

        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = TownGenerator.Instance.houses[Random.Range(0, TownGenerator.Instance.houses.Count)];

        if(isFantasyIsland){
            // heart character
            levelText.text = "♥";
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        }else{
            levelText.text = (levelIndex + 1).ToString();
        }

    }

 
    private void OnMouseDown()
    {
        if(!FoundObject.Instance.ActiveStatus){
            if(isUnlocked){
            
                StartCoroutine(EnterLevel());
                TownGenerator.Instance.PlaySuccess();
            }else{
                StartCoroutine(Shake());
                TownGenerator.Instance.PlayFailure();
            }
        }
        
    }

    void OnMouseOver(){
        if(isUnlocked && levelIndex == ScoreHolder.Instance.currentLevel){
            TownGenerator.Instance.WitchSelector.position = new Vector3(transform.position.x, transform.position.y, -1);
            //StartCoroutine(TownGenerator.Instance.MoveWitches(transform.position));
            hover.SetActive(true);
        }
    }

    void OnMouseExit(){
        hover.SetActive(false);
    }

    IEnumerator EnterLevel(){
        // Reset round count
        ScoreHolder.Instance.roundCount = 0;
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
