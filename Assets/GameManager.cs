using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager gm;
    public bool PlayerTurnEnd = false;

    public int SkipTurn = 0;

    [Range(0.1f,0.9f)]
    public float fireSpeed = 0.5f;

    public List<Block> flamingBlocks = new List<Block>();
    public List<Block> emberedBlocks = new List<Block>();

    // Start is called before the first frame update

    void Awake(){
        gm = this;

    }
    void Start()
    {
        StartCoroutine(StartGame());
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

        PlayerTurnEnd = false;

        // Nature Turn
        

        // Spread Fire
        foreach(Block b in flamingBlocks){
            if(Random.value > 1-fireSpeed){
                b.GetRandomNeighborBlock().SetFire();
            }
        }
        foreach(Block b in emberedBlocks){
            flamingBlocks.Add(b);
        }

        yield return new WaitUntil(()=> PlayerTurnEnd );


        // Player Turn
        yield return new WaitUntil(()=> PlayerTurnEnd );
        SkipTurn--;
        StartCoroutine(TurnSequence());
    }
}
