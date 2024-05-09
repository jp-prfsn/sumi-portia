using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class PatchwareLogo : MonoBehaviour
{
    public Sprite logo;
    private List<Sprite> logoSprites = new List<Sprite>();
    public float framerate = 0.1f;
    public AudioSource audioSource;
    public int CueSoundOnFrame = 0;


    void Start()
    {
        // Load Scene called instructions
        StartCoroutine(PlayAnimation());
        
        
    }

    private IEnumerator PlayAnimation()
    {
        // Clip the logo spritesheet into sprites (1 columns, equally split vertically based on height)
        int columns = 1;
        int rows = 9;
        float width = 45;
        float height = 32;
        logo.texture.filterMode = FilterMode.Point;
        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = columns - 1; j >= 0; j--)
            {
                Rect rect = new Rect(j * width, i * height, width, height);
                Sprite sprite = Sprite.Create(logo.texture, rect, new Vector2(0.5f, 0.5f), 32);
                logoSprites.Add(sprite);
            }
        }

        // Play the animation
        yield return new WaitForSeconds(1);
        
        int f = 0;
        foreach (Sprite sprite in logoSprites)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            yield return new WaitForSeconds(framerate);
            if (audioSource != null && f == CueSoundOnFrame)
            {
                audioSource.Play();
            }
            f++;
        }
        

        yield return new WaitForSeconds(1);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions");
    }

    void OnDrawGizmos(){
        if(GetComponent<SpriteRenderer>().sprite == null){
            Debug.Log("Adding logo sprite");
            float width = logo.texture.width / 1;
            float height = logo.texture.height / 9;
            Rect rect = new Rect(0, 0, width, height);
            Sprite sprite = Sprite.Create(logo.texture, rect, new Vector2(0.5f, 0.5f), 32);
            // set sprite to 32 PPU
            logo.texture.filterMode = FilterMode.Point;
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}
