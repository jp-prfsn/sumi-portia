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
    public TextMeshProUGUI annouceText;

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

    public int BlockFallCount = 0;

    public List<Block> flamingBlocks = new List<Block>();
    public List<Block> emberedBlocks = new List<Block>();

    // Start is called before the first frame update

    void Awake(){
        gm = this;
        portalFreq = Random.Range(3,6);
        fireSpeed = Random.Range(0.1f,0.7f);
    }

    public void Annouce(string s){
        annouceText.text = s;
    }

    void Update(){
        livesScore.text = $"{livesSaved.ToString()}/{GameManager.gm.totalCitizens}";


        if (Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("Gameplay");
        }
        if (Input.GetKeyDown(KeyCode.P)){
            BlockFallCount = 0;
        }
        
    }

    void Start()
    {
        StartCoroutine(StartGame());
    }

    public void CheckIfOver(){
        if(remainingCitizens == 0){

            if(livesSaved > 0 && totalCitizens > 0){
                ScoreHolder.Instance.ratingNumber = Mathf.RoundToInt(livesSaved/totalCitizens)*100;
            }
            

            SceneManager.LoadScene("Success");
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

    private IEnumerator SaveMe(){
        if(BlockFallCount > 0){
            yield return new WaitForSeconds(2);
            if(BlockFallCount > 0){
                BlockFallCount = 0;
            }
        }
        yield return null;
    }

    public IEnumerator TurnSequence(){

        StartCoroutine(SaveMe());

        yield return new WaitUntil(()=> BlockFallCount == 0 );
        Portal.p.Turn();
        PlayerTurnEnd = false;

        // Nature Turn
        foreach(Block b in flamingBlocks){
            // Spread Fire
            if(Random.value > 1-fireSpeed){

                Block nextFireBlock = b.GetRandomNeighborBlock();

                if(nextFireBlock != null){
                    nextFireBlock.SetFire();
                }
            }

            // Hurt Citizens
            if(b.isInterior){
                b.Damage();
                b.BreakCheck();
            }
        }
        foreach(Block b in emberedBlocks){
            flamingBlocks.Add(b);
            
        }
        
        emberedBlocks.Clear();

        


        // Player Turn ------------------------

        // Moving blocks

        
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
