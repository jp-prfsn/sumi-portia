using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Speaker { Sumi , Portia }



public class WriteTextNode : MonoBehaviour
{
    public Speaker speaker;
    public Moods mood;
    // a spacer
    [Space(10)]
    [TextArea(3, 10)]
    public string text;
    public List<GameObject> outpoints = new List<GameObject>();
 
    public bool loopAnimation = true;
    public bool isDecisionNode = false;
    public bool hasBeenPlayed = false;
    public bool centralText = false;
    Color displayColor = new Color(1, 0.5f, 0.5f);

    Color SumiChatColor = new Color(0.5f, 0.5f, 1);
    Color PortiaChatColor = new Color(1, 0.5f, 0.5f);

    public Sprite SumiPortrait;
    public Sprite PortiaPortrait;

    public void CreateNewNode(){
        // Perform your action here
        // For example, you can call a method of WriteTextNode
        // myScript.YourMethodName();
        GameObject nodeholder = GameObject.Find("Author");
        GameObject newChildNode = Instantiate(this.gameObject, nodeholder.transform);
        newChildNode.transform.position = this.transform.position - new Vector3(0, 1, 0);
        newChildNode.GetComponent<WriteTextNode>().RefreshNode();
        outpoints.Add(newChildNode);

        newChildNode.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);

        // select the new node
        Selection.activeGameObject = newChildNode;

        // distribute all linked nodes evenly below this node
        float distanceBetweenOutpoints = 1;
        float totalWidth = outpoints.Count * distanceBetweenOutpoints;
        Vector3 startPosition = this.transform.position - new Vector3(totalWidth / 2, 1, 0);


        outpoints.RemoveAll(item => item == null);

        for (int i = 0; i < outpoints.Count; i++)
        {
            GameObject outpoint = outpoints[i];
            outpoint.transform.position = startPosition + new Vector3((i) + (0.5f), 0, 0);
        }
    }

    public void RefreshNode(){
        text = "New Node";
        outpoints.Clear();
        hasBeenPlayed = false;
        displayColor = (speaker == Speaker.Sumi)?PortiaChatColor:SumiChatColor;
        mood = Moods.Neutral;
        isDecisionNode = false;
    }

     

    // Update is called once per frame
    void OnDrawGizmos()
    {

        if(text == null || text == "" ){
            text = "[ SELECT ANSWER ]";
        }

        if(text == "[ SELECT ANSWER ]"){
            isDecisionNode = true;
        }else{
            isDecisionNode = false;
        }
        this.gameObject.name = text;
        Gizmos.color = (speaker == Speaker.Sumi)?PortiaChatColor:SumiChatColor;
        Gizmos.DrawWireSphere(transform.position, 0.1f );

        if(text != null){
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = (speaker == Speaker.Sumi)?PortiaChatColor:SumiChatColor;
            style.alignment = TextAnchor.MiddleCenter;


            #if UNITY_EDITOR
                Handles.Label(transform.position, text, style);
            #endif
            
        }

         

        outpoints.RemoveAll(item => item == null);

        

        int i = 0;
        foreach (GameObject outpoint in outpoints)
        {
            if (outpoint != null){
                Gizmos.color = (speaker == Speaker.Sumi)?PortiaChatColor:SumiChatColor;
                Gizmos.DrawLine(transform.position, outpoint.transform.position);
            }

            i++;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (outpoints.Count == 0)
        {
            return;
        }

        // Draw a circle around this one
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        foreach (GameObject outpoint in outpoints)
        {
            if (outpoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, outpoint.transform.position);
            }
        }
    }
}

 
