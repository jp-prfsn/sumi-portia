using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialWindowManager : MonoBehaviour
{
    public Transform canvas;

    float verticalOffest = 1.5f;
    float horizontalOffest = 2.5f;

    public RectTransform LineOneText;

    public GameObject otherCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > 1){
            canvas.transform.localPosition = new Vector3(-horizontalOffest, canvas.transform.localPosition.y, 0);
        }else if(transform.position.x < -1){
            canvas.transform.localPosition = new Vector3(horizontalOffest, canvas.transform.localPosition.y, 0);
        }else{
            canvas.transform.localPosition = new Vector3(0, canvas.transform.localPosition.y, 0);
        }

        if(transform.position.y <= -3){
            canvas.transform.localPosition = new Vector3(canvas.transform.localPosition.x, verticalOffest, 0);   
        }else{
            canvas.transform.localPosition = new Vector3(canvas.transform.localPosition.x, -verticalOffest, 0);
        }


        //change rectTransform to preferredHeight
        Vector2 temp = LineOneText.sizeDelta;
        temp.y = LineOneText.GetComponent<TextMeshProUGUI>().preferredHeight;
        LineOneText.sizeDelta = temp; 
    }
}
