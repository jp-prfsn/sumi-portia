using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WaitThenLoadGame : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI scoreRANK;
    public TextMeshProUGUI careerRANK;

    public RuntimeAnimatorController aloneAnim;
    
    // Start is called before the first frame update
    void Start()
    {
        if(ScoreHolder.Instance.gameState == GameStates.PortiaMissing){
            GetComponent<Animator>().runtimeAnimatorController = aloneAnim;
        }
        StartCoroutine(WaitNLoad());
        score.text = ScoreHolder.Instance.ratingNumber.ToString() + "% (" + ScoreHolder.Instance.ratingLetter + ")";
        scoreRANK.text = "Review: " + ScoreHolder.Instance.ratingTitle;
        careerRANK.text = "Career Score: " + ScoreHolder.Instance.careerAvg.ToString() + "/100";
    }

    // Update is called once per frame
    IEnumerator WaitNLoad()
    {
        yield return new WaitForSeconds(2);
        yield return new WaitUntil(()=>Input.GetMouseButtonDown(0));


        if(ScoreHolder.Instance.roundCount == ScoreHolder.Instance.roundsPerLevel-1)
        {
            // if the player has completed the last round of the last level
            if(ScoreHolder.Instance.currentLevel == ScoreHolder.Instance.levelsUnlocked)
            {
                // Unlock next level
                ScoreHolder.Instance.levelsUnlocked++;
                ScoreHolder.Instance.levelsUnlocked = Mathf.Clamp(ScoreHolder.Instance.levelsUnlocked, 0, 9);
            }
            if(ScoreHolder.Instance.currentLevel == 8){
                ScoreHolder.Instance.gameState = GameStates.LivingInFantasy;
            }

            SceneManager.LoadScene("LevelSelect");
            
        }
        else
        {
            ScoreHolder.Instance.roundCount++;
            SceneManager.LoadScene("Gameplay");
        }

    }
}
