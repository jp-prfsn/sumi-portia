using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteMusic : MonoBehaviour
{

    public Sprite mutedImg;
    public Sprite unmutedImg;
    public SpriteRenderer img; 

    void Start(){
        if(ScoreHolder.Instance.mainSong.enabled){
            img.sprite = unmutedImg;
        }else{
            img.sprite = mutedImg;
        }
    }
 
    private void OnMouseDown()
    {
        

        ScoreHolder.Instance.mainSong.enabled = !ScoreHolder.Instance.mainSong.enabled;

        if(ScoreHolder.Instance.mainSong.enabled){
            img.sprite = unmutedImg;
        }else{
            img.sprite = mutedImg;
        }
    }
}
