using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownGenerator : MonoBehaviour
{
    public static TownGenerator Instance;
    public int rows = 7; // Number of rows in the grid
    public int cols = 5; // Number of columns in the grid
    public float cellSize = 1.0f; // Size of each cell

    public float spacing = 0.1f;
    public GameObject TownTile;

    public List<Sprite> houses = new List<Sprite>();

    public Transform WitchSelector;
    public GameObject Portia;
    
    public GameObject[,] gridCells; // 2D array to hold references to each cell  - (col, row)
    public AudioClip success;
    public AudioClip failure;
    public AudioSource aSource;
    bool moving = false;

    

    public AnimationCurve moveCurve;
    public void PlaySuccess(){
        aSource.PlayOneShot(success,1);
    }

    public void PlayFailure(){
        aSource.PlayOneShot(failure,1);
    }

    public IEnumerator MoveWitches(Vector3 targetPos){

        
        
        float duration = 0.1f;
        float timeElapsed = 0;
        Vector3 startPos = WitchSelector.position;
        while(timeElapsed < duration){
            float curvePercent = moveCurve.Evaluate(timeElapsed/duration);
            WitchSelector.position = Vector3.Lerp(startPos, targetPos, curvePercent);
            timeElapsed += Time.deltaTime;
            yield return null;
        } 
    }
 

    public void Start(){

        if(ScoreHolder.Instance.PortiaMissing){
            Portia.SetActive(false);
        }

        Instance = this;
        ScoreHolder.Instance.PlayMenuMusic();

        // Generate the grid
        gridCells = new GameObject[cols, rows];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 cellPosition = transform.position + new Vector3(
                    (x * (cellSize + spacing) + cellSize / 2) - ((float)cols/2 * cellSize) - ((spacing * (cols-1)) / 2),  
                    y * (cellSize + spacing) + cellSize / 2 - ((float)rows/2 * cellSize) - ((spacing * (rows-1)) / 2), 
                    0);
                GameObject newCell = Instantiate(TownTile, cellPosition, Quaternion.identity);
                newCell.transform.parent = transform;
                gridCells[x, y] = newCell;

                    newCell.GetComponent<TownTile>().levelIndex = (y * cols) + x;
                if(ScoreHolder.Instance.levelUnlocked[(y * cols) + x] == 1){
                    // Draw Base
                    newCell.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
                    // Draw House
                    
                    newCell.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
                    WitchSelector.position = new Vector3(cellPosition.x, cellPosition.y, -1);
                    newCell.GetComponent<TownTile>().isUnlocked = true;
                }else{
                    // color is hexcode #666666
                    newCell.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.4f,0.4f,0.4f,1);
                    newCell.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(0.4f,0.4f,0.4f,1);
                }
                newCell.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = houses[Random.Range(0, houses.Count)];
                newCell.GetComponent<TownTile>().Setup();
            }
        }
    }

    private void OnDrawGizmos()
    {
    // Draw grid with gizmos
    Gizmos.color = Color.cyan;
    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            Vector3 cellPosition = transform.position + new Vector3(
                (x * (cellSize + spacing) + cellSize / 2) - ((float)cols/2 * cellSize) - ((spacing * (cols-1)) / 2),  
                y * (cellSize + spacing) + cellSize / 2 - ((float)rows/2 * cellSize) - ((spacing * (rows-1)) / 2), 
                0);



            Gizmos.DrawWireCube(cellPosition, new Vector3(cellSize, cellSize, 0.1f));
        }
    }
}
}
