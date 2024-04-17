using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHolder : MonoBehaviour
{
    public static ScoreHolder Instance;

    public int ratingNumber;
    public string ratingLetter;
    public string ratingTitle;
    public int careerAvg;

    public int gameCount = 0;

    public AudioSource mainSong;
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
        }
    }

    public void ResetSongSpeed(){
        mainSong.pitch = 1;
    }

    
}
