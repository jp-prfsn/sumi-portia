using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;


public class Block : MonoBehaviour
{
    public Vector2Int coOrdXY;

    public Cell hostCell;

    public AnimationCurve FallCurve;

    private Color dCol;
    private Color blockColor;

    public int BreakCount = 0;
    public int BreakingPoint = 6;
    public bool isInterior = false;

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
    public Sprite s_inside;

    public SpriteRenderer sr;

    public GameObject citizen;

    public TextMeshProUGUI remainingKnocks;


    public AudioSource aSource;
    public AudioClip dropSound;

    public void SetFire(){
        if(!aflame){
            aflame = true;
            flame.SetActive(true);
            GameManager.gm.emberedBlocks.Add(this);
        }
    }

    
    
    // Start is called before the first frame update
    void Start()
    {
        blockColor = Color.white;
        sr.color = blockColor;
        remainingKnocks.text = (BreakingPoint - BreakCount).ToString();
    }





    void OnMouseDown()
    {
        if(Summoner.magic.blockSelected){

            if(this.aflame){
                Summoner.magic.heldBlock.SetFire();
            }

            Summoner.magic.SelectCell(this.hostCell);

            
            
        }else{
            Summoner.magic.SelectBlock(this);
            sr.color = Color.white;
        }
    }

    public Block GetRandomNeighborBlock(){

        List<Block> neighbours = new List<Block>();
        
        if(IsCellAbovePossible()){
            if(CellAbove().containedBlock){
                neighbours.Add(CellAbove().containedBlock);
            }
        }
        if(IsCellBelowPossible()){
            if(CellBelow().containedBlock){
                neighbours.Add(CellBelow().containedBlock);
            }
        }
        if(IsCellWestPossible()){
            if(CellWest().containedBlock){
                neighbours.Add(CellWest().containedBlock);
            }
        }
        if(IsCellEastPossible()){
            if(CellEast().containedBlock){
                neighbours.Add(CellEast().containedBlock);
            }
        }
        Debug.Log(neighbours.Count);
        return neighbours[Random.Range(0, neighbours.Count)];
    }

    public Cell GetLowestCellBelow(){

        Cell returnCell = null;
        
        for(int row = coOrdXY.y; row >= 1; row--){

            Cell cellBelow = GridGenerator.gridder.gridCells[coOrdXY.x, row-1];

            if(cellBelow.containedBlock){
                return GridGenerator.gridder.gridCells[coOrdXY.x, row];
            }
        }
        return GridGenerator.gridder.gridCells[coOrdXY.x, 0];
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0.1f));
        //Handles.Label(transform.position, BreakCount.ToString());
        Handles.Label(transform.position, coOrdXY.ToString());
    }

    public bool IsCellAbovePossible(){
        return coOrdXY.y < GridGenerator.gridder.rows-1;
    }
    public bool IsCellBelowPossible(){
        return coOrdXY.y > 0;
    }
    public bool IsCellEastPossible(){
        return coOrdXY.x < GridGenerator.gridder.cols-1;
    }
    public bool IsCellWestPossible(){
        return coOrdXY.x > 0;
    }

    public Cell CellAbove(){
        if(IsCellAbovePossible()){
            return GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y + 1];
        }else{
            return null;
        }
    }
    public Cell CellBelow(){
        if(IsCellBelowPossible()){
            return GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y - 1];
        }else{
            return null;
        }
    }
    public Cell CellEast(){
        if(IsCellEastPossible()){
            return GridGenerator.gridder.gridCells[coOrdXY.x + 1, coOrdXY.y];
        }else{
            return null;
        }
    }
    public Cell CellWest(){
        if(IsCellWestPossible()){
            return GridGenerator.gridder.gridCells[coOrdXY.x - 1, coOrdXY.y];
        }else{
            return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;

        Vector3 abovePos  = (IsCellAbovePossible())?GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y+1].transform.position:transform.position;
        Vector3 eastPos  = (IsCellWestPossible())?GridGenerator.gridder.gridCells[coOrdXY.x+1, coOrdXY.y].transform.position:transform.position;
        Vector3 belowPos  = (IsCellBelowPossible())?GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y-1].transform.position:transform.position;
        Vector3 westPos  = (IsCellEastPossible())?GridGenerator.gridder.gridCells[coOrdXY.x-1, coOrdXY.y].transform.position:transform.position;


        Gizmos.DrawLine(transform.position, abovePos);
        Gizmos.DrawLine(transform.position, eastPos);
        Gizmos.DrawLine(transform.position, belowPos);
        Gizmos.DrawLine(transform.position, westPos);
    }

    public IEnumerator BlockFall(){

        sr.color = blockColor;

        if(IsCellBelowPossible()){
            if(CellBelow().containedBlock){
                // There's a block below
                yield break;
            }
        }

        Cell FallToCell = GetLowestCellBelow();
        if(!GetLowestCellBelow()){
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
        aSource.PlayOneShot(dropSound, 1);

        ReassignHostCell(hostCell, FallToCell);
        BreakCount++;
        remainingKnocks.text = (BreakingPoint - BreakCount).ToString();

        if(BreakCount == BreakingPoint){
            hostCell.containedBlock = null;
            // create rubble
        }

        // knock down the ones we land on
        if(IsCellBelowPossible()){
            if(CellBelow().containedBlock){
                StartCoroutine(GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y-1].containedBlock.BlockFall() );
            }
        }

        // blocks above will follow
        if(callLater){
            StartCoroutine(callLater.BlockFall() );
            callLater = null;
        }

        
        if(BreakCount == BreakingPoint){
            Break();
        }
    }

    public void ReassignHostCell(Cell oldCell, Cell newCell){
        callLater = IsCellAbovePossible()?CellAbove().containedBlock:null;
        oldCell.containedBlock = null;

        if(newCell){
            hostCell = newCell;
            coOrdXY = newCell.coOrdXY;
            newCell.containedBlock = this;
        }
    }

    public void Break(){
        // Release my Cell
        this.hostCell.containedBlock = null;

        // PlayAnimation

        // Tell above block to fall.
        if(CellAbove()){
            if(CellAbove().containedBlock){
                StartCoroutine(CellAbove().containedBlock.BlockFall());

            }
        }
    


        if(this.isInterior){
            // Kill Inhabitants
            GameManager.gm.livesKilled ++;
            GameManager.gm.remainingCitizens--;
            GameManager.gm.CheckIfOver();
        }

        Destroy(this.gameObject);
    }

    public void Release(){
        
    }

    public void StyleBlock(){

        if(isInterior){
            sr.sprite = s_inside;
            citizen.SetActive(true);
            return;
        }

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
        

        
        Sprite newVar = (Sprite)this.GetType().GetField("s_" + nArray).GetValue(this);
        sr.sprite = newVar;
    }

    public IEnumerator FlashGreen(){
        float timeElapsed = 0;
        float duration = 0.5f;

        Color start = Summoner.magic.SummonMagicColor;

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
