using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Block : MonoBehaviour
{
    public Vector2Int coOrdXY;

    public Cell hostCell;

    public AnimationCurve FallCurve;

    private Color dCol;
    private Color blockColor;

    public int BreakCount = 0;
    public int BreakingPoint = 6;
    public bool isBlock = true;

    public bool aflame;

    public GameObject flame;
    public GameObject select;

    public string nArray;

    Block callLater;

    //Cell NorthCell => (coOrdXY.y < GridGenerator.gridder.rows)?GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y+1]:null;



    [Header("Appearance")]
    public Sprite s_0000;
    public Sprite s_1111;

    public Sprite s_1000;
    public Sprite s_0100;
    public Sprite s_0010;
    public Sprite s_0001;

    public Sprite s_1100;
    public Sprite s_1010;
    public Sprite s_1001;
    public Sprite s_0110;
    public Sprite s_0101;
    public Sprite s_0011;

    public Sprite s_1110;
    public Sprite s_1101;
    public Sprite s_1011;
    public Sprite s_0111;

    public SpriteRenderer sr;

    public void SetFire(){
        aflame = true;
        flame.SetActive(true);
        GameManager.gm.emberedBlocks.Add(this);
    }

    
    
    // Start is called before the first frame update
    void Start()
    {
        blockColor = Color.white;
        sr.color = blockColor;
    }



    void OnMouseDown()
    {
        if(Summoner.magic.blockSelected){
            Summoner.magic.SelectCell(this.hostCell);
            
        }else{
            Summoner.magic.SelectBlock(this);
            sr.color = Color.white;
        }
    }

    public Block GetRandomNeighborBlock(){
        List<Block> neighbours = new List<Block>();
        Cell above  = (coOrdXY.y < GridGenerator.gridder.rows)?GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y+1]:null;
        Cell below  = (coOrdXY.y > 0)?GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y-1]:null;
        Cell west  = (coOrdXY.x > 0)?GridGenerator.gridder.gridCells[coOrdXY.x-1, coOrdXY.y]:null;
        Cell east  = (coOrdXY.x < GridGenerator.gridder.cols)?GridGenerator.gridder.gridCells[coOrdXY.x+1, coOrdXY.y]:null;

        if(above){
            if(above.containedBlock){
                neighbours.Add(above.containedBlock);
            }
        }
        if(below){
            if(below.containedBlock){
                neighbours.Add(below.containedBlock);
            }
        }
        if(west){
            if(west.containedBlock){
                neighbours.Add(west.containedBlock);
            }
        }
        if(east){
            if(east.containedBlock){
                neighbours.Add(east.containedBlock);
            }
        }
        return neighbours[Random.Range(0, neighbours.Count)];
    }

  

    public bool CellBelowIsEmpty(){
        if(coOrdXY.y > 0){

            Cell underCell = GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y-1];
            if(underCell.containedBlock){
                return false;
            }
            
        }
        return true;
    }
    public Block BlockAbove(){
        if(coOrdXY.y < GridGenerator.gridder.rows-1){
            Cell overCell = GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y+1];
            if(overCell.containedBlock){
                return overCell.containedBlock;
            }
        }
        return null;
    }

    public Cell GetLowCell(){

        Cell returnCell = null;
        
        for(int row = coOrdXY.y; row >= 1; row--){

            Debug.Log("Checking Row " + coOrdXY.y);

            Cell cellBelow = GridGenerator.gridder.gridCells[coOrdXY.x, row-1];

            if(cellBelow.containedBlock){
                Debug.Log("Blocked!");
                return GridGenerator.gridder.gridCells[coOrdXY.x, row];
            }
        }
        return GridGenerator.gridder.gridCells[coOrdXY.x, 0];
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0.1f));
        Handles.Label(transform.position, BreakCount.ToString());
    }

    private void OnDrawGizmosSelected()
    {
        if(coOrdXY.y > 0){
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y-1].transform.position, new Vector3(1, 1, 0.1f));
        }
    }

    public IEnumerator BlockFall(){

        sr.color = blockColor;

        if(!CellBelowIsEmpty()){
            // There's a block below
            yield break;
        }

        Cell FallToCell = GetLowCell();
        if(!GetLowCell()){
            yield break;
        }
        

        float timeElapsed = 0;
        float duration = 0.3f;
        Vector2 start = transform.position;
        Vector2 end = FallToCell.transform.position;

        while (timeElapsed < duration)
        {
            float percent = Mathf.Clamp01( timeElapsed / duration);
            float curvePercent = FallCurve.Evaluate( percent);
            transform.position = Vector3.LerpUnclamped( start, end, curvePercent);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        ReassignHostCell(hostCell, FallToCell);
        BreakCount++;

        if(BreakCount == BreakingPoint){
            hostCell.containedBlock = null;
            // create rubble
        }

        // knock down the ones we land on
        if(!CellBelowIsEmpty() && coOrdXY.y > 0){
            StartCoroutine(GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y-1].containedBlock.BlockFall() );
        }

        // blocks above will follow
        if(callLater){
            StartCoroutine(callLater.BlockFall() );
            callLater = null;
        }

        
        if(BreakCount == BreakingPoint){
            Destroy(this.gameObject);
        }
    }

    public void ReassignHostCell(Cell oldCell, Cell newCell){
        callLater = BlockAbove();
        oldCell.containedBlock = null;
        hostCell = newCell;
        coOrdXY = newCell.coOrdXY;
        newCell.containedBlock = this;
    }

    public void StyleBlock(){
        Cell above  = (coOrdXY.y < GridGenerator.gridder.rows-1)?GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y+1]:null;
        Cell below  = (coOrdXY.y > 0)?GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y-1]:null;
        Cell west  = (coOrdXY.x > 0)?GridGenerator.gridder.gridCells[coOrdXY.x-1, coOrdXY.y]:null;
        Cell east  = (coOrdXY.x < GridGenerator.gridder.cols-1)?GridGenerator.gridder.gridCells[coOrdXY.x+1, coOrdXY.y]:null;

        nArray = "0000";

        if(above){
            string v = above.containedBlock?"1":"0";
            nArray = v + nArray[1].ToString() +nArray[2].ToString()+nArray[3].ToString()+"";
        }
        if(east){
            string v = east.containedBlock?"1":"0";
            nArray = nArray[0].ToString()+  v  +nArray[2].ToString()+nArray[3].ToString()+"";
        }
        if(below){
            string v = below.containedBlock?"1":"0";
            nArray = nArray[0].ToString()+nArray[1].ToString()+ v +nArray[3].ToString()+"";
        }
        if(west){
            string v = west.containedBlock?"1":"0";
            nArray = nArray[0].ToString()+nArray[1].ToString()+nArray[2].ToString()+ v +"";
        }
        

        Debug.Log(nArray);

        Sprite newVar = (Sprite)this.GetType().GetField("s_" + nArray).GetValue(this);
        sr.sprite = newVar;
    }

    public IEnumerator FlashGreen(){
        float timeElapsed = 0;
        float duration = 0.5f;

        Color start = Color.green;;

        while (timeElapsed < duration)
        {
            float percent = Mathf.Clamp01( timeElapsed / duration);
            float curvePercent = FallCurve.Evaluate( percent);
            sr.color = Color.LerpUnclamped( start, Color.white, curvePercent);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        sr.color = Color.white;
        yield return null;
    }
}
