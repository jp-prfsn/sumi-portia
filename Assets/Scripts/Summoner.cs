using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : MonoBehaviour
{
    public static Summoner magic;
    public bool blockSelected = false;
    public bool cellSelected = false;

    public Color SummonMagicColor;

    public Block heldBlock;

    public AudioSource aSource;
    public AudioClip clickSound;
    public AudioClip summonSound;

    public SpriteRenderer indicator;



    // Start is called before the first frame update
    void Awake()
    {
        magic = this;
    }


    public void SelectBlock(Block b)
    {
        // Highlight the block
        blockSelected = true;
        b.select.SetActive(true);
        heldBlock = b;
        indicator.color = Color.white;


        aSource.PlayOneShot(clickSound,1);
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
        indicator.color = Color.white;
        if(blockSelected){

            // Move the block and destroy any pre-contained block there.
            if(c.containedBlock){
                c.containedBlock.Break(false, true);
            }

            indicator.color = SummonMagicColor;


            Cell ba = heldBlock.CellAbove();

            heldBlock.ReassignHostCell( heldBlock.hostCell, c);

            heldBlock.transform.position = c.transform.position;

            if(heldBlock.gameObject.activeSelf){
                StartCoroutine( heldBlock.FlashCol(SummonMagicColor) ); 
            }


            if(ba){
                if(ba.containedBlock){
                    if(ba.containedBlock.gameObject.activeSelf){
                        ba.containedBlock.StartFall(); // drop block that was supported by this block
                    }
                }
            }

            if(heldBlock){
                if(heldBlock.gameObject.activeSelf){
                    heldBlock.StartFall(); // drop this block if placed in midair
                }
            }

            blockSelected = false;
            cellSelected = false;
            heldBlock.select.SetActive(false);
            heldBlock = null;

            aSource.PlayOneShot(summonSound,1);

            GameManager.gm.WaitingForPlaceBlock = false;
            

        }
    }
}
