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
    
    // Start is called before the first frame update
    void Start()
    {
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
        SceneManager.LoadScene("Gameplay");

    }
}
