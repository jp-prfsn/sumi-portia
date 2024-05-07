using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Moods { Neutral, Squint, Sleepy, Surprise, Sad, Tongue, Happy, Pleased, Displeased, Angry }


public class Author : MonoBehaviour
{
    public static Author dude;
    public WriteTextNode currentNode;

    [TextArea(13, 30)]
    public string Script;
    private string ScriptCheck;

    [Header("Sprites")]
    public Sprite neutralSprite;
    public Sprite squintSprite;
    public Sprite sleepySprite;
    public Sprite surpriseSprite;
    public Sprite sadSprite;
    public Sprite tongueSprite;
    public Sprite happySprite;
    public Sprite pleasedSprite;
    public Sprite displeasedSprite;
    public Sprite angrySprite;

    public Color SumiColor;
    public Color PortiaColor;

    public Sprite DamagedSumi;
    public Sprite DamagedPortia;


    public List<WriteTextNode> StartNodes = new List<WriteTextNode>();
 
    // Start is called before the first frame update
    void Start()
    {
        dude = this;

        if(StartNodes[ScoreHolder.Instance.currentLevel] != null && ScoreHolder.Instance.PortiaMissing == false){
            currentNode = StartNodes[ScoreHolder.Instance.currentLevel];
            
            DialogueManager.dude.speaker1.gameObject.SetActive(true);
            DialogueManager.dude.speaker2.gameObject.SetActive(true);


            if(currentNode.SumiPortrait != null){
                DialogueManager.dude.speaker1.sprite = currentNode.SumiPortrait;
                DialogueManager.dude.speaker2.sprite = currentNode.PortiaPortrait;
            }
 

        }else{
            // Skip ahead to the gameplay scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
        }
    }

    public Sprite face(){
        // a switch statement that returns the sprite that matches the mood
        switch(currentNode.mood){
            case Moods.Neutral:
                return neutralSprite;
            case Moods.Squint:
                return squintSprite;
            case Moods.Sleepy:
                return sleepySprite;
            case Moods.Surprise:
                return surpriseSprite;
            case Moods.Sad:
                return sadSprite;
            case Moods.Tongue:
                return tongueSprite;
            case Moods.Happy:
                return happySprite;
            case Moods.Pleased:
                return pleasedSprite;
            case Moods.Displeased:
                return displeasedSprite;
            case Moods.Angry:
                return angrySprite;
            default:
                return neutralSprite;
        }

    }

    public void NextNode()
    {
        if(currentNode.outpoints.Count == 1){
            // If there is only one option, choose it
            currentNode = currentNode.outpoints[0].GetComponent<WriteTextNode>();
        }else{
            // If there are multiple outpoints, choose the one that matches the player's choice
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(currentNode.transform.position, 0.5f);

        if(Script != ScriptCheck){
            // If the script has changed, update the dialogue tree
            ScriptCheck = Script;
            //currentNode = null;
            RenderTree();
            // Create a new dialogue tree
        }
    }

    void RenderTree(){
        // Destroy Previous Dialogue Tree
        for(int i = transform.childCount-1; i >= 0; i--){
            //Destroy(transform.GetChild(i).gameObject);
        }



        // Render the dialogue tree
    }
}
