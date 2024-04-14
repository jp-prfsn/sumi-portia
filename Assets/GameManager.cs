using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager gm;
    public bool PlayerTurnEnd = false;

    public TextMeshProUGUI livesScore;

    public int SkipTurn = 0;

    [Range(0.1f,0.9f)]
    public float fireSpeed = 0.5f;

    public int portalFreq = 5;

    public int TurnCount = 0;

    public int TurnWithinPortalLoop = 0;
    public int livesSaved = 0;
    public int livesKilled = 0;

    public int remainingCitizens;
    public int totalCitizens;

    public List<Block> flamingBlocks = new List<Block>();
    public List<Block> emberedBlocks = new List<Block>();

    // Start is called before the first frame update

    void Awake(){
        gm = this;
    }

    void Update(){
        livesScore.text = $"{livesSaved.ToString()}/{GameManager.gm.totalCitizens}";
        
    }

    void Start()
    {
        StartCoroutine(StartGame());
    }

    public void CheckIfOver(){
        if(remainingCitizens == 0){
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitUntil(()=> GridGenerator.gridder.ready);

        // Set Fire
        var allBlocks = FindObjectsOfType<Block>();
        allBlocks[Random.Range(0, allBlocks.Length)].SetFire();





        StartCoroutine(TurnSequence());
    }

    public IEnumerator TurnSequence(){

        Portal.p.Turn();

        PlayerTurnEnd = false;

        // Nature Turn
        

        // Spread Fire
        foreach(Block b in flamingBlocks){
            if(Random.value > 1-fireSpeed){

                Block nextFireBlock = b.GetRandomNeighborBlock();

                if(nextFireBlock != null){
                    nextFireBlock.SetFire();
                }
            }
        }
        foreach(Block b in emberedBlocks){
            flamingBlocks.Add(b);
        }

        yield return new WaitUntil(()=> PlayerTurnEnd );


        // Player Turn
        yield return new WaitUntil(()=> PlayerTurnEnd );
        SkipTurn--;
        TurnCount++;
        TurnWithinPortalLoop++;
        if(TurnWithinPortalLoop >= portalFreq){
            TurnWithinPortalLoop = 0;
        }
        StartCoroutine(TurnSequence());
    }
}
