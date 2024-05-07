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
        if(ScoreHolder.Instance.PortiaMissing){
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
            if(ScoreHolder.Instance.currentLevel == ScoreHolder.Instance.levelUnlocked.Length-1)
            {
                ScoreHolder.Instance.roundCount = 0;
                SceneManager.LoadScene("LevelSelect");

            }else{
                ScoreHolder.Instance.roundCount = 0;
                ScoreHolder.Instance.levelUnlocked[ScoreHolder.Instance.currentLevel+1] = 1;
                SceneManager.LoadScene("LevelSelect");
            }
        }
        else
        {
            ScoreHolder.Instance.roundCount++;
            SceneManager.LoadScene("Gameplay");
        }

    }
}
