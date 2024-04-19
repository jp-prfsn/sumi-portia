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

    public int gamesPerLevel = 2;
    public int minPortalTurns = 2;
    private Transform camTransform;

    [Header("Rescue Counter")]
    public int rescueKillCounter = 0;
    public Sprite savedIcon,killedIcon;
    public Transform PeepHolder;
    public GameObject peepscoreimage;



    [Header("Camera Shake")]
    public int ShakeLength = 4;
    public float shakeDistance = 0.3f;
    public float duration = 0.1f;
    public AnimationCurve ShakeCurve;




    void Awake(){
        gm = this;
        camTransform = Camera.main.transform;
        
    }
    void Start()
    {
        //portalFreq = Random.Range(2 , 3 + Mathf.RoundToInt((ScoreHolder.Instance.gameCount) / (float)gamesPerLevel));
        portalFreq = Random.Range(2 , 4) + Mathf.RoundToInt((float)ScoreHolder.Instance.gameCount/5);

        fireSpeed = Random.Range(0.3f,0.9f);

        float f = fireSpeed;
        f = Mathf.Round(f * 10.0f) * 0.1f; 
        fireText.text = f.ToString();
        StartCoroutine(StartGame());
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
        PeepHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(12 * totalCitizens, sizeD.y);
        for(int i=0; i< totalCitizens; i++)
        {
            GameObject psi = Instantiate(peepscoreimage);
            psi.transform.SetParent(PeepHolder);
        }
    }
    public void UpdatePeeps(bool alive){
        
        StartCoroutine(MexicanWave(alive));
    }

    private IEnumerator MexicanWave(bool alive){
        for(int i=0; i< totalCitizens; i++)
        {
            RectTransform ch = PeepHolder.transform.GetChild(i).GetComponent<RectTransform>();
            ch.anchoredPosition -= new Vector2(0,2);
            yield return new WaitForSeconds(0.1f);
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
        
    }

    

    public void CheckIfOver(){
        if(remainingCitizens == 0){

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

            ScoreHolder.Instance.gameCount++;

            Invoke("LoadSuccess",2);
        }
    }

    private void LoadSuccess(){
        SceneManager.LoadScene("Success");
    }

    IEnumerator StartGame()
    {
        yield return new WaitUntil(()=> GridGenerator.gridder.ready);

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

        /*-- Player Turn Starts --*/
        clickIndicator.enabled = true;
        WaitingForPlaceBlock = true;
        yield return new WaitUntil(()=> !WaitingForPlaceBlock );
        clickIndicator.enabled = false;
        yield return new WaitForSeconds(0.5f); 
        /*-- Player Turn Over --*/

        yield return null;

        // Nature Turn
        foreach(Block b in flamingBlocks){
            // Spread Fire
            b.FireSpread();
            b.Damage("fire");
            b.BreakCheck();
            
        }
        foreach(Block b in emberedBlocks){
            flamingBlocks.Add(b);
        }

        emberedBlocks.Clear();

        int RoundsOfDestruction = 1;
        while(destructionList.Count > 0){
            yield return new WaitUntil(()=> NoBlocksMoving ); // When blocks have fallen into place //
            BlockDestroyRound();
            Debug.Log("Destruction Round " + RoundsOfDestruction);
            yield return null;
            //yield return new WaitForSeconds(1);
        }
        
        SkipTurn--;
        TurnCount++;
        TurnWithinPortalLoop++;
        if(TurnWithinPortalLoop >= portalFreq){
            TurnWithinPortalLoop = 0;
        }

        yield return new WaitUntil(()=> NoBlocksMoving );
        Portal.p.Turn();
        

        
        
        StartCoroutine(TurnSequence());
    }
}
