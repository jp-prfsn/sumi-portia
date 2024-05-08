using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{

    public static GameManager gm;
    public bool WaitingForPlaceBlock = false;
    public TextMeshProUGUI annouceText;
    public TextMeshProUGUI fireText;
    public TextMeshProUGUI turnText;



    /* Count Of How Many Blocks are in transition state */
    public int BlockFallCount = 0;
    public bool NoBlocksMoving => BlockFallCount == 0;
    public int SkipTurn = 0;

    public SpriteRenderer clickIndicator;

    



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
    public List<Block> destructionList = new List<Block>();
    public int minPortalTurns = 2;
    private Transform camTransform;



    [Header("Rescue Counter")]
    public int rescueKillCounter = 0;
    public Sprite savedIcon,killedIcon;
    public Transform PeepHolder;
    public GameObject peepscoreimage;


    [Header("Tutorial")]
    bool tutLevel = false;
    public GameObject canv;
    public bool movingBlocksTutflag = false;
    public bool portalTutflag = false;
    public bool AngerTutFlag = false;
    public Transform TutorialFocus;
    public TextMeshProUGUI commandText;
    public void TutorialFocusOn(string command, Transform focusOnThis){
        TutorialFocus.gameObject.SetActive(true);
        TutorialFocus.position = focusOnThis.position;
        commandText.text = command;
        canv.SetActive(false);
    }
    public void LoseTutorialFocus(){
        TutorialFocus.gameObject.SetActive(false);
        canv.SetActive(true);
    }



    



    [Header("Camera Shake")]
    public int ShakeLength = 4;
    public float shakeDistance = 0.3f;
    public float duration = 0.1f;
    public AnimationCurve ShakeCurve;


    [Header("Audio")]
    public AudioSource aSource;
    public AudioSource livesAudio;
    public AudioClip completeSound;

    [Space(10)]

    [Header("Portia Death Sequence")]

    public SpriteRenderer Portia;
    bool PortiaAflame = false;
    bool portiaDiesThisTurn = false;
    bool PortiaMissing => ScoreHolder.Instance.gameState == GameStates.PortiaMissing?true:false;
    public List<GameObject> PortiaRelatedThings = new List<GameObject>();
    public Sprite PortiaDeathPose;
    public GameObject PortiaFlames;
    


    void Awake(){
        gm = this;
        camTransform = Camera.main.transform;
        aSource = GetComponent<AudioSource>();
        
    }
    void Start()
    {

        SetGameStates();

        CheckMissingAndDisablePortia();
        
        if(ScoreHolder.Instance.currentLevel == 8 && PortiaMissing){
            // If Portia is gone, play multiple rounds in a daze.
            ScoreHolder.Instance.roundsPerLevel = 5;
        }else{
            ScoreHolder.Instance.roundsPerLevel = 1;
        }

        portalFreq = Mathf.Max( minPortalTurns, ScoreHolder.Instance.currentLevel + 1 );

        ScoreHolder.Instance.PlayGameMusic();

        fireSpeed = Random.Range( 0.3f, 0.9f );
        if(PortiaMissing){
            fireSpeed = 1;
        }

        float f = fireSpeed;
        f = Mathf.Round(f * 10.0f) * 0.1f; 
        fireText.text = f.ToString();
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitUntil(()=> GridGenerator.gridder.ready);

        if(ScoreHolder.Instance.roundCount == 0){
            movingBlocksTutflag = false;
            portalTutflag = false;
            AngerTutFlag = false;
        }else{
            movingBlocksTutflag = true;
            portalTutflag = true;
            AngerTutFlag = true;
        }

        // Set Fire
        var allBlocks = FindObjectsOfType<Block>();
        allBlocks[Random.Range(0, allBlocks.Length)].SetFire();
        foreach(Block b in emberedBlocks){
            flamingBlocks.Add(b);
        }
        emberedBlocks.Clear();
        StartCoroutine(TurnSequence());
    }



    public IEnumerator TurnSequence(){

        if(ScoreHolder.Instance.currentLevel == 7 && !PortiaAflame){
            if(TurnCount == 3){
                PortiaCatchesFire();
                StartCoroutine(ConstantCameraShake());
                Annouce("Help! I'm on fire!");
            }
        }

        if(tutLevel){
            if(!portalTutflag && TurnWithinPortalLoop == portalFreq-1){
                // Select a citizen
                TutorialFocusOn("When the portal is ready, target an occupied block", GridGenerator.gridder.RandomBlockWithCitizen().transform);
                yield return new WaitUntil(()=> Summoner.magic.blockSelected );
                yield return new WaitForSeconds(0.5f);
                // Click the portal
                TutorialFocusOn("Click to send them safely into the portal", Portal.p.transform);
                
            }
            if(!movingBlocksTutflag){
                // click a random block
                Block firstBlock = GridGenerator.gridder.RandomBlock();
                TutorialFocusOn("Click to target a block", firstBlock.transform);
                yield return new WaitUntil(()=> Summoner.magic.blockSelected );
                yield return new WaitForSeconds(0.5f);
                // click a random block
                TutorialFocusOn("Click to summon it to a new location.", GridGenerator.gridder.RandomBlock(firstBlock).transform);
            }
        }

        if(ScoreHolder.Instance.currentLevel == 8 && PortiaMissing && !AngerTutFlag){
            // Select a citizen
            Block firstBlock = GridGenerator.gridder.RandomBlock();
            TutorialFocusOn("...", firstBlock.transform);
            yield return new WaitUntil(()=> Summoner.magic.blockSelected );
            yield return new WaitForSeconds(0.5f);
            // Click the Citizen
            TutorialFocusOn("You didn't deserve her", GridGenerator.gridder.RandomBlockWithCitizen().transform);

        }


        /*-- Player Turn Starts --*/
        clickIndicator.enabled = true;
        WaitingForPlaceBlock = true;

        yield return new WaitUntil(()=> !WaitingForPlaceBlock );

        if(tutLevel){
            if(!movingBlocksTutflag){
                yield return new WaitForSeconds(0.5f);
                LoseTutorialFocus();
                movingBlocksTutflag = true;
            }
            if(!portalTutflag && TurnWithinPortalLoop == portalFreq-1){
                yield return new WaitForSeconds(0.5f);
                LoseTutorialFocus();
                portalTutflag = true;
            }
        }
        if(ScoreHolder.Instance.currentLevel == 8 && PortiaMissing && !AngerTutFlag){
            yield return new WaitForSeconds(0.5f);
            LoseTutorialFocus();
            AngerTutFlag = true;
        }


        clickIndicator.enabled = false;
        yield return new WaitForSeconds(0.5f); 
        /*-- Player Turn Over --*/

        yield return null;

        /* -- NON PLAYER ACTIONS -- */
        // FIRE SPREAD
        Debug.Log("Spreading Fire");
        foreach(Block b in flamingBlocks){
            b.FireSpread();
            b.Damage("fire");
            b.BreakCheck();
        }
        foreach(Block b in emberedBlocks){
            Debug.Log("Embering Blocks");
            flamingBlocks.Add(b);
        }
        emberedBlocks.Clear();

        // DESTROY BLOCKS
        int RoundsOfDestruction = 1;
        while(destructionList.Count > 0){
            yield return new WaitUntil(()=> NoBlocksMoving ); // When blocks have fallen into place //
            BlockDestroyRound();
            Debug.Log("Destruction Round " + RoundsOfDestruction);
            yield return null;
        }


        
        SkipTurn--;
        TurnCount++;
        TurnWithinPortalLoop++;
        if(TurnWithinPortalLoop >= portalFreq){
            TurnWithinPortalLoop = 0;
        }

        yield return new WaitUntil(()=> NoBlocksMoving );
        
        
        if(!PortiaMissing){
            Debug.Log("Starting Next Turn");
            Portal.p.Turn();
        }else{
            Debug.Log("Portia Missing. Skipping Portal actions.");
        }

        StartCoroutine(TurnSequence());
    }



    void SetGameStates(){
        if(ScoreHolder.Instance.currentLevel == 0){

            tutLevel = true;

        }else if(ScoreHolder.Instance.currentLevel == 8){

            ScoreHolder.Instance.gameState = GameStates.PortiaMissing;

        }else if(ScoreHolder.Instance.currentLevel == 9){

            ScoreHolder.Instance.gameState = GameStates.LivingInFantasy;
        }
    }

    void PortiaCatchesFire(){
        
        // Set Portia sprite to flaming
        PortiaAflame = true;
        PortiaFlames.SetActive(true);
        Portia.sprite = PortiaDeathPose;

    }

    void CheckMissingAndDisablePortia(){

        // if she is missing, disable her and related things
        if(PortiaMissing){
            foreach(GameObject go in PortiaRelatedThings){
                go.SetActive(false);
            }
        }
    }
    public void CameraBounce(){
        StartCoroutine(CamBounce());
    }
    private IEnumerator CamBounce(){
        float timeElapsed = 0;
        Vector3 originalPosition = camTransform.position;
        Vector3 ShakePoint = originalPosition + new Vector3(0,shakeDistance,0);
        while (timeElapsed < duration)
        {
            float percent = Mathf.Clamp01( timeElapsed / duration);
            float curvePercent = ShakeCurve.Evaluate( percent);
            camTransform.position = Vector3.LerpUnclamped( originalPosition, ShakePoint, curvePercent);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        camTransform.position = originalPosition;
    }

    public void Annouce(string s){
        annouceText.text = s;
    }

    public void DrawPeepScoreboard(){
        Vector2 sizeD = PeepHolder.GetComponent<RectTransform>().sizeDelta;
        
        for(int i=0; i< totalCitizens; i++)
        {
            GameObject psi = Instantiate(peepscoreimage);
            psi.transform.SetParent(PeepHolder);
            psi.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        PeepHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(12 * totalCitizens, sizeD.y);
    }
    public void UpdatePeeps(bool alive){
        
        StartCoroutine(MexicanWave(alive));
    }

    private IEnumerator MexicanWave(bool alive){
        livesAudio.pitch = 1;
        for(int i=0; i< totalCitizens; i++)
        {
            RectTransform ch = PeepHolder.transform.GetChild(i).GetComponent<RectTransform>();
            ch.anchoredPosition -= new Vector2(0,2);
            livesAudio.Play();
            yield return new WaitForSeconds(0.1f);
            livesAudio.pitch += 0.2f;
            ch.anchoredPosition += new Vector2(0,2);
            if(i == rescueKillCounter){
                ch.GetComponent<Image>().sprite = alive?savedIcon:killedIcon;
                
                rescueKillCounter++;
                yield break;
            }
        }
    }

    public void BlockDestroyRound(){

        for(int i= destructionList.Count-1; i >= 0; i--){
            Block workingOnBlock = destructionList[i];
            destructionList.RemoveAt(i);
            workingOnBlock.Break(false, false);
        }

        destructionList.Clear();
    }

    

    void Update(){


        turnText.text = TurnCount.ToString();

        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene("Gameplay");
        }

        if (Input.GetKeyDown(KeyCode.X)){
            remainingCitizens = 0;
            CheckIfOver();
        }
        
    }

    

    public void CheckIfOver(){
        if(remainingCitizens == 0){

            if(ScoreHolder.Instance.currentLevel == 8){
                portiaDiesThisTurn = true;
            }
            

            int origRating = ScoreHolder.Instance.careerAvg;

            if(totalCitizens > 0){
                ScoreHolder.Instance.ratingNumber = Mathf.RoundToInt(((float)livesSaved/(float)totalCitizens)*100);
            }

            ScoreHolder.Instance.careerAvg = Mathf.RoundToInt(((float)origRating + (float)ScoreHolder.Instance.ratingNumber)/(float)2);

            float TimeFactor = (float)(100 - Mathf.Clamp(TurnCount, 0, 100)) / 100f;

            if(ScoreHolder.Instance.ratingNumber < 20){
                ScoreHolder.Instance.ratingLetter = "F";
                ScoreHolder.Instance.ratingTitle = "Sadistic";
            }else if(ScoreHolder.Instance.ratingNumber < 40){
                ScoreHolder.Instance.ratingLetter = "D";
                ScoreHolder.Instance.ratingTitle = "Incompetent";
            }else if(ScoreHolder.Instance.ratingNumber < 60){
                ScoreHolder.Instance.ratingLetter = "C";
                ScoreHolder.Instance.ratingTitle = "Middling";
            }else if(ScoreHolder.Instance.ratingNumber < 80){
                ScoreHolder.Instance.ratingLetter = "A";
                ScoreHolder.Instance.ratingTitle = "Rather good!";
            }else if(ScoreHolder.Instance.ratingNumber <= 100){
                ScoreHolder.Instance.ratingLetter = "A+";
                ScoreHolder.Instance.ratingTitle = "Top notch!";
            }

 
            aSource.PlayOneShot(completeSound, 1);


            if(portiaDiesThisTurn){
                ScoreHolder.Instance.gameState = GameStates.PortiaMissing;
                foreach(GameObject go in PortiaRelatedThings){
                    go.SetActive(false);
                }
            }

            Invoke("LoadSuccess",2);
            

            
        }
    }

    private void LoadSuccess(){
        SceneManager.LoadScene("Success");
    }

    private IEnumerator ConstantCameraShake(){
        while(true){
            yield return new WaitForSeconds(0.1f);
            CameraBounce();
        }
    }

    
    public IEnumerator moveTutPointer(){
        yield return null;
    }
}
