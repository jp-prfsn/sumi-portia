using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NighttimeAudio : MonoBehaviour
{
    // Start is called before the first frame update

    public static NighttimeAudio Instance;
    public float UpVolume = 0.5f;
    void Start()
    {
        UpVolume = GetComponent<AudioSource>().volume;
        GetComponent<AudioSource>().volume = 0;
        StartCoroutine(FadeAudioIn());
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private IEnumerator FadeAudioIn(){
        float duration = 4.0f;
        float timeElapsed = 0;
        while(GetComponent<AudioSource>().volume < UpVolume){
            timeElapsed += Time.deltaTime;
            GetComponent<AudioSource>().volume = Mathf.Lerp(0, UpVolume, timeElapsed/duration);
            yield return null;
        }
    }


}
