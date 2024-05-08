using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates{
    Regular,
    PortiaMissing,
    LivingInFantasy
}

public class ScoreHolder : MonoBehaviour
{
    public static ScoreHolder Instance;

    [Header("Game States")]

    public GameStates gameState = GameStates.Regular;
 
    [Space]
    [Header("Player Stats")]
    public int ratingNumber;
    public string ratingLetter;
    public string ratingTitle;
    public int careerAvg;

    [Space]
    [Header("Level Stats")]
    public int[] levelUnlocked = new int[15]{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
    public int currentLevel = 0;
    public int roundCount = 0;
    public int roundsPerLevel = 1;


    [Space]
    [Header("Audio")]
    public AudioSource mainSong;
    private AudioSource aSource;
    public AudioClip GameMusic;
    public AudioClip MenuMusic;

    [Space]
    [Header("Cursor")]
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;





    
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            // Destroy Self
            Destroy(this.gameObject);
        }
        else
        {
            // Become main KEEPER
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
    }

    public void PlayGameMusic(){
        if(mainSong.clip == GameMusic){
            return;
        }
        StartCoroutine(FadeOutSongThenPlay(GameMusic));
    }

    public void PlayMenuMusic(){
        if(mainSong.clip == MenuMusic){
            return;
        }
        StartCoroutine(FadeOutSongThenPlay(MenuMusic));
    }

    public IEnumerator FadeOutSongThenPlay(AudioClip clip){

        float duration = 1.0f;
        float timeElapsed = 0;
        while(mainSong.volume > 0){

            timeElapsed += Time.deltaTime;
            mainSong.volume = Mathf.Lerp(1, 0, timeElapsed/duration);
            yield return null;
        }
        mainSong.Stop();
        mainSong.volume = 1;
        mainSong.clip = clip;
        mainSong.Play();
    }

    public void ResetSongSpeed(){
        mainSong.pitch = 1;
    }

    
}
