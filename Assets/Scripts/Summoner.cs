using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : MonoBehaviour
{
    public static Summoner magic;
    public bool blockSelected = false;
    public bool cellSelected = false;

    

    public Block heldBlock;
    // Start is called before the first frame update
    void Awake()
    {
        magic = this;
    }

    // Update is called once per frame
    public void SelectBlock(Block b)
    {
        // Highlight the block
        blockSelected = true;
        b.select.SetActive(true);
        heldBlock = b;
    }

    public void SelectCell(Cell c)
    {
        if(c.containedBlock == heldBlock){
            blockSelected = false;
            cellSelected = false;
            heldBlock.select.SetActive(false);
            heldBlock = null;
            return;
        }
        cellSelected = true;
        if(blockSelected){

            // Move the block and destroy any pre-contained block there.
            if(c.containedBlock){
                Destroy(c.containedBlock.gameObject);
            }

            Block ba = heldBlock.BlockAbove();

            heldBlock.ReassignHostCell( heldBlock.hostCell, c);

            heldBlock.transform.position = c.transform.position;

            StartCoroutine(heldBlock.FlashGreen());

            if(ba != null){
                StartCoroutine( ba.BlockFall() ); // drop block that was supported by this block
            }

            StartCoroutine( heldBlock.BlockFall() ); // drop this block if placed in midair

            blockSelected = false;
            cellSelected = false;
            heldBlock.select.SetActive(false);
            heldBlock = null;


            GameManager.gm.PlayerTurnEnd = true;
            

        }
    }
}
