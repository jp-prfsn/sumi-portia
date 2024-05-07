using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public RectTransform TextTransform;
    public RectTransform BoxTransform;
    TextMeshProUGUI myText;
    public Sprite whiteBox;
    public Sprite blackBox;
    public Sprite dashBox;
    public Speaker speaker;
    public AudioSource audioS;
    public AudioClip clicker;
    private Image myBox;
    public Vector2 textSize;
    public Sprite Anima;
    public bool loopAnimation;
    public AnimationCurve curve;
    public bool isDecisionNode;
    public bool isAnswer;
    public bool _over = false;
    public bool ChangeCursor = false;
    public bool isBeingDragged = false;
    public List<GameObject> outpoints = new List<GameObject>();

    private Vector2 dragOffset;


    public WriteTextNode NodeData;


    public Texture2D openHandcursor;
    public Texture2D closedHandcursor;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public Color SumiColor;
    public Color PortiaColor;


    void Awake(){
        BoxTransform = GetComponent<RectTransform>();
        myText = TextTransform.GetComponent<TextMeshProUGUI>();
        myBox = GetComponent<Image>();
    }

    public void SetUp(WriteTextNode node){
        NodeData = node;
        speaker = node.speaker;
        myText.text = node.text;
        myText.maxVisibleCharacters = 0;

        myBox.color = (speaker == Speaker.Sumi)?SumiColor:PortiaColor;
        myText.color = (speaker == Speaker.Sumi)?SumiColor:PortiaColor;

        if(node.isDecisionNode){
            myBox.sprite = dashBox;
            myText.color = Color.clear;
        }

        

        //myText.alignment = (speaker == Speaker.Sumi)?TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        myText.alignment = myText.alignment;
        textSize = TextTransform.sizeDelta;
        textSize.y = TextTransform.GetComponent<TextMeshProUGUI>().preferredHeight;
        TextTransform.sizeDelta = textSize;
        isDecisionNode = node.isDecisionNode;
        outpoints = node.outpoints;
    }

    


    public IEnumerator TypeText(){

        if(myText.text != "[ SELECT ANSWER ]"){
            foreach (char letter in myText.text.ToCharArray())
            {
                myText.maxVisibleCharacters++;
                if(!isAnswer){
                    audioS.pitch = Random.Range(0.9f, 1.1f);
                    audioS.PlayOneShot(clicker, 0.3f);
                }
            

                if (letter == '.' || letter == ',')
                {
                    // periods
                    yield return new WaitForSeconds(0.4f);
                }
                else if (letter == ',')
                {
                    // comma
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    yield return new WaitForSeconds(0.05f);
                }
                //play noise?
            }
        }

        
        
        
        DialogueManager.dude.fullyTyped = true;
    }

    public IEnumerator ShiftDown(float distance){

        DialogueManager.dude.ActiveSliders++;
        float time = 0;
        Vector2 original = BoxTransform.anchoredPosition;
        Vector2 newpos = BoxTransform.anchoredPosition - new Vector2(0, distance);

        float duration = 0.2f;

        while(time < duration){
            time += Time.deltaTime;

            float percent = Mathf.Clamp01(time / duration);
            float curvedPercent = curve.Evaluate(percent);
            BoxTransform.anchoredPosition = Vector2.Lerp(original, newpos, curvedPercent);
            yield return null;
        }

        BoxTransform.anchoredPosition = newpos;
        myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, myText.color.a -0.33f);
        myBox.color = new Color(myBox.color.r, myBox.color.g, myBox.color.b, myBox.color.a -0.33f);

        if(myText.color.a <= 0){
            Destroy(gameObject);
        }
        DialogueManager.dude.ActiveSliders--;
    }



    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        _over = true;
    }
 
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _over = false;
    }

    void LateUpdate()
    {
        // check if mouse is down
        if(isAnswer){

            /* Terminate if another node is currently being dragged */
            

            if(_over){
                ChangeCursor = true;
                // Hovering
                Cursor.SetCursor(openHandcursor, hotSpot, cursorMode);

                if(Input.GetMouseButtonDown(0)){
                    // Enter drag mode
                    isBeingDragged = true;
                    DialogueManager.dude.heldNode = this;

                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    dragOffset = transform.position - (Vector3)mousePosition;

                }

                else if (Input.GetMouseButton(0))
                {
                    // Remain in Drag Mode
                    BoxTransform.localScale = new Vector3(0.95f, 0.95f, 0.8f);
                    Cursor.SetCursor(closedHandcursor, hotSpot, cursorMode);
                    
                }else if(Input.GetMouseButtonUp(0)){

                    // Exit drag mode
                    isBeingDragged = false;
                    Cursor.SetCursor(openHandcursor, hotSpot, cursorMode);

                    BoxTransform.localScale = new Vector3( 1f, 1f, 1f);

                    // Check if we've dropped this on a decision node
                    if(DialogueManager.dude.DoesThisOverlapDecisionNode(this.GetComponent<RectTransform>())){
                        Cursor.SetCursor(DialogueManager.dude.defaultcursor, hotSpot, cursorMode);
                        DialogueManager.dude.TextNodeParent.GetChild(0).GetComponent<TextNode>().RestyleDecisionNodeAsTextNode();
                        DialogueManager.dude.ArrangeMessages();
                    }else{
                        // Return to original position
                        DialogueManager.dude.ArrangeAnswers();
                    }

                }
            }else{

                
                if(ChangeCursor){
                    Cursor.SetCursor(DialogueManager.dude.defaultcursor, hotSpot, cursorMode);
                    ChangeCursor = false;
                }
            }

            

            // Relocate the dragged object
            if(isBeingDragged){

                Vector2 offset = new Vector2(0, 0.1f);
                
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                //Vector2 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                
                transform.position = mousePosition + dragOffset;
                //transform.position = objPosition + offset;  
            }
        }
    }

    void RestyleDecisionNodeAsTextNode(){
        NodeData = DialogueManager.dude.heldNode.NodeData;
        isDecisionNode = false;
        speaker = NodeData.speaker;
        myBox.sprite = (speaker == Speaker.Sumi)?blackBox:whiteBox;
        myText.color = (speaker == Speaker.Sumi)?SumiColor:PortiaColor;
        myText.alignment = (speaker == Speaker.Sumi)? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        myText.text = NodeData.text;
        myText.maxVisibleCharacters = myText.text.Length;
        textSize = TextTransform.sizeDelta;
        textSize.y = TextTransform.GetComponent<TextMeshProUGUI>().preferredHeight;
        TextTransform.sizeDelta = textSize;
        outpoints = NodeData.outpoints;
        Destroy(DialogueManager.dude.heldNode.gameObject);
        DialogueManager.dude.heldNode = null;
        DialogueManager.dude.ClickIntoPlace();
        Author.dude.currentNode = NodeData;
        // Empty all children of the AnswerNodeParent
        for(int i = DialogueManager.dude.AnswerNodeParent.childCount-1; i >= 0; i--){
            Destroy(DialogueManager.dude.AnswerNodeParent.GetChild(i).gameObject);
        }

        // Play Next Node
        DialogueManager.dude.WaitingForAnswer = false;
        StartCoroutine(DialogueManager.dude.CreateOneMessage());
 

    }
}
