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

    public GameObject portalTile;

    public List<string> RescueQuips;

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

        if(GameManager.gm.TurnWithinPortalLoop == GameManager.gm.portalFreq-2){
            GameManager.gm.Annouce("Portal Incoming!");
        }

        if(GameManager.gm.TurnWithinPortalLoop == GameManager.gm.portalFreq-1){
            GameManager.gm.Annouce("Portal ready!");
        }
        if(GameManager.gm.TurnWithinPortalLoop == 0){
            GameManager.gm.Annouce("");
        }

        for(int i=0; i < boxHolder.childCount; i++)
        {
            if(i == GameManager.gm.TurnWithinPortalLoop){
                boxHolder.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
                portalTile.SetActive(true);
            }else{
                boxHolder.GetChild(i).GetComponent<SpriteRenderer>().color = portalColor;
                portalTile.SetActive(false);
            }
        }


        portalAnim.SetBool("Open", ActivePortal);
        //bc.enabled = ActivePortal;
        if(ActivePortal){
            aSource.PlayOneShot(portalActiveSound,1);
        }
    }

    void OnMouseDown()
    {
        if(ActivePortal){
            if(Summoner.magic.blockSelected){
                // Send it into the portal
                Block dropLater;
                if(Summoner.magic.heldBlock.isInterior){

                    Summoner.magic.heldBlock.ReassignHostCell(Summoner.magic.heldBlock.hostCell, null);
                    dropLater = Summoner.magic.heldBlock.callLater;

                    
                    GameManager.gm.CameraBounce();

                    GameManager.gm.Annouce(RescueQuips[Random.Range(0,RescueQuips.Count)]);

                    if(dropLater){
                        if(dropLater.gameObject.activeSelf){
                            dropLater.StartFall();
                        }
                    }
                }


                Summoner.magic.heldBlock.Break(Summoner.magic.heldBlock.isInterior, true);

                aSource.PlayOneShot(portalAction,1);
                StartCoroutine(flash());

                

                Summoner.magic.heldBlock = null;
                Summoner.magic.blockSelected = false;

                GameManager.gm.WaitingForPlaceBlock = false;
            }
        }else{
            GameManager.gm.Annouce("The portal's not ready!");
        }
    }


    IEnumerator flash(){
        sr.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        sr.color = portalColor;
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        sr.color = portalColor;
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        sr.color = portalColor;
        yield return new WaitForSeconds(0.2f);
    }

    
}
