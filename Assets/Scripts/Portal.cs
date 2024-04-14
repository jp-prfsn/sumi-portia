using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private BoxCollider2D bc;
    public SpriteRenderer sr;
    public Animator portalAnim;
    public bool ActivePortal;

    public AudioSource aSource;
    public AudioClip portalActiveSound;
    public AudioClip portalAction;

    public Transform boxHolder;
    public GameObject box;

    public float boxSpacer = 0.1f;

    public static Portal p;

    public Sprite filledBox;

    public Color portalColor;

    void Awake(){
        p = this;
    }

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        for(int i=GameManager.gm.portalFreq-1 ; i >= 0; i--){
            GameObject boxI = Instantiate(box);
            boxI.transform.SetParent(boxHolder);
        }

        // Position each element accordingly
        for(int i = 0; i < GameManager.gm.portalFreq; i++)
        {
            boxHolder.GetChild(i).localPosition = new Vector3( -(boxSpacer * ((GameManager.gm.portalFreq-1) - i)), 0, 0) ;

            if(i == GameManager.gm.portalFreq-1){
                boxHolder.GetChild(i).GetComponent<SpriteRenderer>().sprite = filledBox;
            }
        }
    }

    public void Turn(){
        
        ActivePortal = (GameManager.gm.TurnCount > 0) && (GameManager.gm.TurnWithinPortalLoop == GameManager.gm.portalFreq-1);

        for(int i=0; i < boxHolder.childCount; i++)
        {
            if(i == GameManager.gm.TurnWithinPortalLoop){
                boxHolder.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
            }else{
                boxHolder.GetChild(i).GetComponent<SpriteRenderer>().color = portalColor;
            }
        }


        portalAnim.SetBool("Open", ActivePortal);
        bc.enabled = ActivePortal;
        if(ActivePortal){
            aSource.PlayOneShot(portalActiveSound,1);
        }
    }

    void OnMouseDown()
    {
        if(ActivePortal){
            if(Summoner.magic.blockSelected){
                // Send it into the portal
                if(Summoner.magic.heldBlock.isInterior){

                    Block dropLater = Summoner.magic.heldBlock.CellAbove().containedBlock;
                    Summoner.magic.heldBlock.ReassignHostCell(Summoner.magic.heldBlock.hostCell, null);


                    GameManager.gm.livesSaved ++;
                    GameManager.gm.remainingCitizens--;

                    GameManager.gm.CheckIfOver();
                    
                    
                    if(dropLater){
                        StartCoroutine(Summoner.magic.heldBlock.CellAbove().containedBlock.BlockFall());
                    }
                }

                aSource.PlayOneShot(portalAction,1);

                Summoner.magic.heldBlock.Break();
                Summoner.magic.heldBlock = null;
                Summoner.magic.blockSelected = false;

                GameManager.gm.PlayerTurnEnd = true;
            }
        }
    }

    
}
