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
    public Color damageColor;
    public Color fireColor;

    bool invulnerable = false;

    public int BreakCount = 0;
    public int BreakingPoint = 6;
    public bool isInterior = false;

    public bool aflame;

    public GameObject flame;
    public GameObject select;
    public GameObject cracks;
    public GameObject collapse;
    public GameObject tank;
    public bool hasTank;

    public string nArray;

    public Block callLater;

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

    public GameObject breakage;


    public AudioSource aSource;
    public AudioClip dropSound;

    public bool isFalling;

    bool unbroken = true;

    public void SetFire(){
        if(!aflame){
            aflame = true;
            flame.SetActive(true);
            GameManager.gm.emberedBlocks.Add(this);
            Debug.Log("Flammable Neighbor Added to Ember List");
        }
    }

    
    
    // Start is called before the first frame update
    void Start()
    {
        blockColor = Color.white;
        sr.color = blockColor;
        BreakingPoint = isInterior?Random.Range(2,4):Random.Range(3,9);
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
        if(neighbours.Count == 0){
            return null;
        }
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
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0.1f));
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
        
        Gizmos.color = damageColor;

        Vector3 abovePos  = (IsCellAbovePossible())?GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y+1].transform.position:transform.position;
        Vector3 eastPos  = (IsCellWestPossible())?GridGenerator.gridder.gridCells[coOrdXY.x+1, coOrdXY.y].transform.position:transform.position;
        Vector3 belowPos  = (IsCellBelowPossible())?GridGenerator.gridder.gridCells[coOrdXY.x, coOrdXY.y-1].transform.position:transform.position;
        Vector3 westPos  = (IsCellEastPossible())?GridGenerator.gridder.gridCells[coOrdXY.x-1, coOrdXY.y].transform.position:transform.position;


        Gizmos.DrawLine(transform.position, abovePos);
        Gizmos.DrawLine(transform.position, eastPos);
        Gizmos.DrawLine(transform.position, belowPos);
        Gizmos.DrawLine(transform.position, westPos);
    }

    public void StartFall(){
        // Bottom Level, can't fall
        if(!IsCellBelowPossible()){
            return;
        }

        // Block Below, can't fall
        if(IsCellBelowPossible()){
            if(CellBelow().containedBlock){
                // There's a block below
                return;
            }
        }

        Cell FallToCell = GetLowestCellBelow();
        if(!GetLowestCellBelow()){
            return;
        }

        if(isFalling){
            return;
        }else{
            isFalling = true;
        }
        GameManager.gm.BlockFallCount++;
        StartCoroutine(BlockFall(FallToCell));
    }

    public IEnumerator BlockFall(Cell FallToCell){

        invulnerable = true;        

        float timeElapsed = 0;
        float duration = 0.3f;
        Vector2 start = transform.position;
        Vector2 end = FallToCell.transform.position;

        Cell originalCell = hostCell;

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
        invulnerable = false;
        ReassignHostCell(hostCell, FallToCell);
        
        // Both the falling cell & the cell that was underneath take damage from the collision
        this.Damage("structural");

        if(FallToCell.CellBelow()){
            if(FallToCell.CellBelow().containedBlock){
                FallToCell.CellBelow().containedBlock.Damage("structural");
                FallToCell.CellBelow().containedBlock.BreakCheck();
            }
        }
        
        // knock down the ones we land on
        if(IsCellBelowPossible()){
            if(CellBelow().containedBlock){
                if(CellBelow().coOrdXY.y > 0){// Not already on the bottom
                    if(CellBelow().containedBlock.gameObject.activeSelf){
                        CellBelow().containedBlock.StartFall();

                    }
                }
            }
        }

        // blocks above will follow
        if(callLater){
            if(callLater.gameObject.activeSelf){
                callLater.StartFall();
            }
            callLater = null;
        }

        isFalling = false;

        GameManager.gm.BlockFallCount--; /* Movement Over */

        BreakCheck();
    }

    public void FireSpread(){
        if(Random.value > 1-GameManager.gm.fireSpeed){

            int limiter = 0;
            Block nextFireBlock = GetRandomNeighborBlock();
            while((nextFireBlock == null || nextFireBlock.gameObject.activeSelf == false) && limiter < 20){
                
                limiter++;
                nextFireBlock = GetRandomNeighborBlock();

            }

            if((nextFireBlock != null && nextFireBlock.gameObject.activeSelf) && limiter <= 20){
                nextFireBlock.SetFire();
                Debug.Log("Flammable Neighbor Found");
                return;
            }

            Debug.Log("Flammable Neighbor Not Found: " + nextFireBlock);
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

    public void Damage(string cause){

        Color causeColour = cause=="structural"?damageColor:fireColor;

        if(BreakCount < BreakingPoint){
            BreakCount++;
            
            StartCoroutine(FlashCol(causeColour));
        }

        remainingKnocks.text = (BreakingPoint - BreakCount).ToString();

        if(BreakingPoint - BreakCount == 1){
            cracks.SetActive(true);
        }
    }

    public void BreakCheck(){
        if(BreakCount >= BreakingPoint){
            if(!GameManager.gm.destructionList.Contains(this)){
                GameManager.gm.destructionList.Add(this);
            }
        }
    }

    public void Break(bool intoPortal = false, bool dontTouchNeighbors = false){
        if(unbroken){
           
            if(!invulnerable){

                unbroken = false;

                // Release my Cell
                this.hostCell.containedBlock = null;

                // Tell above block to fall.
                if(intoPortal){
                    // Sent Into Portal
                    Debug.Log("Breaking a block into portal.");
                }else if(dontTouchNeighbors){
                    // Replacing a preexisting block
                    Debug.Log("Breaking a block to be replaced.");
                }else{
                    // Check if a block is above, and if so, cause it to drop.
                    Debug.Log("Breaking a block to be replaced.");
                    if(CellAbove()){
                        if(CellAbove().containedBlock){
                            if(CellAbove().containedBlock.gameObject.activeSelf){
                                CellAbove().containedBlock.StartFall();
                            }
                        }
                    }
                }

                if(this.hasTank){
                    if(CellWest()){
                        if(CellWest().containedBlock){
                            if(CellWest().containedBlock.gameObject.activeSelf){
                                CellWest().containedBlock.hasTank = true;
                                CellWest().containedBlock.Break();
                            }
                        }
                    }
                    if(CellEast()){
                        if(CellEast().containedBlock){
                            if(CellEast().containedBlock.gameObject.activeSelf){
                                CellEast().containedBlock.hasTank = true;
                                CellEast().containedBlock.Break();
                            }
                        }
                    }
                }

                if(this.isInterior){
                    // Kill Inhabitants
                    if(intoPortal){
                        GameManager.gm.livesSaved ++;
                        GameManager.gm.UpdatePeeps(true);
                        // play life saved particles
                    }else{
                        GameManager.gm.livesKilled ++;
                        GameManager.gm.UpdatePeeps(false);
                        // Play blood particles
                    }
                    
                    GameManager.gm.remainingCitizens--;
                    GameManager.gm.CheckIfOver();
                }

                //Destroy(this.gameObject);
                Instantiate(breakage, this.transform.position, Quaternion.identity);
                this.gameObject.SetActive(false);
            }
        }
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

    public IEnumerator FlashCol(Color flashCol){
        float timeElapsed = 0;
        float duration = 0.5f;

        Color start = flashCol;

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
