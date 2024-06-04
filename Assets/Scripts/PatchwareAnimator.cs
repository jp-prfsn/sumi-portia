using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;


public enum AnimationStyle
{
    Loop,
    OutAndBack,
    Random
}

public class PatchwareAnimator : MonoBehaviour
{

    [Tooltip("Spritesheet used for the animation")]
    public Sprite spritesheet;
    private List<Sprite> frames = new List<Sprite>();
    public int frameWidth = 32; // Width of each frame
    public int frameHeight = 32; // Height of each frame

    [Space(10)]

    [Tooltip("Style of the animation")]
    public AnimationStyle animationStyle = AnimationStyle.Loop;


    [Header("Looping Options")]
    [Tooltip("If true, the animation will loop continuously")]
    public bool continuousLoop = true;


    [Tooltip("Gap in seconds between animation repetitions")]
    [Range(0, 30)]
    public int repeatGapSeconds = 1;


    [Tooltip("Randomness factor for animation repetition gap")]
    [Range(0, 1)]
    public float repeatGapLengthRandomness = 0f;


    // Indexes of frames where the animation stops for a certain amount of time
    [Tooltip("List of frames where the animation will pause (seconds)")]
    public List<int> pauseFrames = new List<int>();
    public int pauseLength = 1;

    
    
    public float startDelay = 0;
    [Range(1, 30)]
    public float framerate = 12;

    [Tooltip("Event to trigger the animation")]
    public UnityEvent TriggeredBy;


    void Start()
    {
        // Clip the spritesheet into a list of frames
        ClipSpritesheetIntoFrames();

        // Start the animation
        StartCoroutine(Animate());
    }

    private void ClipSpritesheetIntoFrames()
    {
        Texture2D texture = spritesheet.texture;
        int spriteCount = (texture.width / frameWidth) * (texture.height / frameHeight);
        for (int i = spriteCount-1; i > 0; i--)
        {
            int x = (i * frameWidth) % texture.width;
            int y = (i * frameWidth / texture.width) * frameHeight;
            Sprite newSprite = Sprite.Create(texture, new Rect(x, y, frameWidth, frameHeight), new Vector2(0.5f, 0.5f));
            frames.Add(newSprite);
        }
    }

    public IEnumerator Animate()
    {
        yield return new WaitForSeconds(startDelay);

        do
        {
            if(animationStyle == AnimationStyle.Loop)
            {

                for (int i = 0; i < frames.Count; i++)
                {
                    // Display the current frame
                    GetComponent<SpriteRenderer>().sprite = frames[i];

                    // Wait for a certain amount of time
                    yield return new WaitForSeconds(1f / framerate);

                    // If the current frame is a hang frame, wait for a longer time
                    if (pauseFrames.Contains(i))
                    {
                        yield return new WaitForSeconds(pauseLength);
                    }
                }

            }
            else if(animationStyle == AnimationStyle.OutAndBack)
            {
                // Play the animation forward
                for (int i = 0; i < frames.Count; i++)
                {
                    // Display the current frame
                    GetComponent<SpriteRenderer>().sprite = frames[i];

                    // Wait for a certain amount of time
                    yield return new WaitForSeconds(1f / framerate);

                    // If the current frame is a hang frame, wait for a longer time
                    if (pauseFrames.Contains(i))
                    {
                        yield return new WaitForSeconds(pauseLength);
                    }
                }
                // Reverse the animation
                for (int i = frames.Count - 1; i >= 0; i--)
                {
                    // Display the current frame
                    GetComponent<SpriteRenderer>().sprite = frames[i];

                    // Wait for a certain amount of time
                    yield return new WaitForSeconds(1f / framerate);

                    // If the current frame is a hang frame, wait for a longer time
                    if (pauseFrames.Contains(i))
                    {
                        yield return new WaitForSeconds(pauseLength);
                    }
                }
                
            }else if(animationStyle == AnimationStyle.Random){
                // Play the animation in a random order

                List<Sprite> temp = new List<Sprite>();
                List<Sprite> shuffled = new List<Sprite>();
                temp.AddRange(frames);

                for (int i = 0; i < frames.Count; i++)
                {
                    int index = Random.Range(0, temp.Count - 1);
                    shuffled.Add(temp[index]);
                    temp.RemoveAt(index);
                    // display the shuffled frame
                    GetComponent<SpriteRenderer>().sprite = shuffled[i];
                    // Wait for a certain amount of time
                    yield return new WaitForSeconds(1f / framerate);

                    // If the current frame is a hang frame, wait for a longer time
                    if (pauseFrames.Contains(i))
                    {
                        yield return new WaitForSeconds(pauseLength);
                    }
                }


            }

            // Wait for a certain amount of time before repeating the animation
            yield return new WaitForSeconds(repeatGapSeconds + Random.Range(-repeatGapLengthRandomness, repeatGapLengthRandomness));
            yield return null;

        } while (continuousLoop);
    }

    public void TriggerAnimation()
    {
        if (TriggeredBy != null)
        {
            TriggeredBy.Invoke();
        }

        StartCoroutine(Animate());
    }
}

