using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteMusic : MonoBehaviour
{
    public static MuteMusic Instance;
    public Sprite mutedImg;
    public Sprite unmutedImg;
    public SpriteRenderer img; 

    void Start(){
        Instance = this;
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

    public void Mute(){
        ScoreHolder.Instance.mainSong.enabled = false;
        img.sprite = mutedImg;
    }
}
