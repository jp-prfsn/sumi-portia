using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    public static DialogueManager dude;
    private AudioSource audioS;
    public AudioClip clicker;

    public AudioSource continueS;
    public AudioClip continueBeeper;

    public bool gameOver;

 

    public string text;
    public int speaker;


    public Sprite portrait;
    public Sprite fallbackPortrait;

 
    public Image speaker1;
    public Image speaker2;

    public GameObject continueArrow;

    public bool fullyTyped;


    public int currentChoice = 0;



    private bool off;

    public GameObject CurrentlyDraggedNode;

    private TMP_Text currentTB;

    public int ActiveSliders;


    public bool WaitingForAnswer;

    public Transform TextNodeParent;
    public Transform AnswerNodeParent;
    public RectTransform Pointer;
    public AudioClip MessageArrival;    

    [Header("Prefabs")]
    public GameObject TextNodePrefab;
    public TextNode heldNode;
    public GameObject whitescreen;
    public Image WhiteOut;
    public AnimationCurve movementCurve;

    public Texture2D defaultcursor;

    public GameObject clickpoint;

    void Awake(){
        
    }

    // Start is called before the first frame update
    void Start()
    {
        dude = this;
        audioS = gameObject.GetComponent<AudioSource>();

        fullyTyped = true;

        
        continueArrow.SetActive(false);
        Cursor.SetCursor(defaultcursor, Vector2.zero, CursorMode.Auto);
        StartCoroutine("CreateOneMessage");
    }

    void Update(){
        //move to next beat
        if (Input.GetMouseButtonDown(0) && fullyTyped && !WaitingForAnswer && !gameOver){

            if(Author.dude.currentNode.outpoints.Count == 0){
                // TRANSITION TO LEVEL SELECT SCREEN
                SceneManager.LoadScene("Gameplay");
            }else{
                StartCoroutine("CreateOneMessage");
            }
        }

        if(Input.GetMouseButtonDown(0) && !WaitingForAnswer){
            DrawClickpoint();
        }
        
        if (Input.GetKeyDown("r")){

            Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
        }
    }

    private void DrawClickpoint(){
        // convert mouse position to world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        //Instantiate(clickpoint, mousePos, Quaternion.identity);
    }

    

    private GameObject CreateNewNode(GameObject prefab, Transform Prnt, WriteTextNode ident){
        //create new message
        GameObject newMessage = Instantiate(prefab, Prnt);
        RectTransform newMessageRect = newMessage.GetComponent<RectTransform>();
        TextNode newMessageNode = newMessage.GetComponent<TextNode>();
        newMessageRect.localScale = new Vector3(1, 1, 1);
        newMessage.transform.SetAsFirstSibling();
        newMessageRect.anchoredPosition = new Vector2(1000, 0);
        newMessageNode.SetUp(ident);
        
        newMessageNode.isAnswer = (Prnt == AnswerNodeParent) ? true : false;
        return newMessage;
    }


    public void ArrangeMessages(){
        float cumulativeHeight = 0;
        
        // vertically distribute all children in the answer node parent, centered in the middle
        for(int i = 0; i < TextNodeParent.childCount; i++){
            
            // call the MoveNode coroutine for each child,
            RectTransform rt = TextNodeParent.GetChild(i).GetComponent<RectTransform>();

            Vector2 newPosition = new Vector2(0, -cumulativeHeight);
            if(TextNodeParent.GetChild(i).GetComponent<TextNode>().speaker == Speaker.Sumi){
                newPosition = new Vector2(-10, -cumulativeHeight);
            }else{
                newPosition = new Vector2(10, -cumulativeHeight);
            }

            StartCoroutine(MoveNode(newPosition, TextNodeParent.GetChild(i).gameObject));
            //rt.anchoredPosition = newPosition;

            // add this child's height to the cumulative height
            cumulativeHeight += TextNodeParent.GetChild(i).GetComponent<TextNode>().textSize.y + 5;
        }
    }


    public void ArrangeAnswers(){

         
    }

    private IEnumerator MoveNode(Vector2 end, GameObject node){

        RectTransform rt = node.GetComponent<RectTransform>();
        Vector2 start = rt.anchoredPosition;

        float elapsedTime = 0;
        float duration = 0.1f;
        
        while(elapsedTime < 1){

            elapsedTime += Time.deltaTime;
            float doneness = Mathf.Clamp01(elapsedTime / duration);
            float donenessCurve = movementCurve.Evaluate(doneness);
            rt.anchoredPosition = Vector2.Lerp(start, end, donenessCurve);
            yield return null;

        }

        rt.anchoredPosition = end;
    }



    public IEnumerator CreateOneMessage()
    {
        if(fullyTyped){

            fullyTyped = false;

            continueArrow.SetActive(false);
            currentChoice = 0;
            yield return new WaitForSeconds(0.1f);
            Author.dude.NextNode();

            if(Author.dude.currentNode.speaker == Speaker.Sumi){
                speaker1.color = new Color(speaker1.color.r, speaker1.color.g, speaker1.color.b, 1);
                speaker2.color = new Color(speaker2.color.r, speaker2.color.g, speaker2.color.b, 0.5f);
            }else{
                speaker1.color = new Color(speaker1.color.r, speaker1.color.g, speaker1.color.b, 0.5f);
                speaker2.color = new Color(speaker2.color.r, speaker2.color.g, speaker2.color.b, 1);
            }


            //create new message
            GameObject newMessage = CreateNewNode(TextNodePrefab, TextNodeParent, Author.dude.currentNode );
            RectTransform newMessageRect = newMessage.GetComponent<RectTransform>();
            TextNode newMessageNode = newMessage.GetComponent<TextNode>();
            WaitingForAnswer = Author.dude.currentNode.isDecisionNode;

            //move all existing messages down, but not the new one
            float height = newMessage.GetComponent<TextNode>().textSize.y + 5;
            
            ArrangeMessages();
            yield return new WaitUntil(() => ActiveSliders == 0);
            
            audioS.PlayOneShot(MessageArrival, 0.7f);



            yield return new WaitForSeconds(0.3f);
            StartCoroutine(newMessageNode.TypeText());
 

            // Wait until the text has been fully typed, then we can press space to continue
            yield return new WaitUntil(() => fullyTyped);
 
            yield return new WaitForSeconds(0.1f);
            if(!WaitingForAnswer){
                continueArrow.SetActive(true);
            }
        }
    }

    
    

    public bool DoesThisOverlapDecisionNode(RectTransform draggedRect)
    {
        // Convert the RectTransforms to screen space
        Rect rect1 = RectTransformToScreenSpace(draggedRect);
        Rect rect2 = RectTransformToScreenSpace(TextNodeParent.GetChild(0).GetComponent<RectTransform>());

        // Check if the rectangles overlap
        return rect1.Overlaps(rect2);
    }

    Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
        rect.x -= (transform.pivot.x * size.x);
        rect.y -= ((1.0f - transform.pivot.y) * size.y);
        return rect;
}

    public void ClickIntoPlace(){
        audioS.PlayOneShot(MessageArrival, 1f);
    }

}
