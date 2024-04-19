using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int rows = 7; // Number of rows in the grid
    public int cols = 5; // Number of columns in the grid
    public float cellSize = 1.0f; // Size of each cell

    public bool ready = false;

    public static GridGenerator gridder;

    public GameObject cell;
    public GameObject block;
    public GameObject citizen;

    public int[,] buildingMap1  = new int[7,5]{ {0,0,0,0,0},{0,1,0,1,0},{0,2,1,1,0},{1,1,1,1,1},{0,1,2,1,0},{1,1,1,2,1},{0,1,1,1,0} };
    public int[,] buildingMap2  = new int[7,5]{ {0,0,0,0,0},{0,1,0,1,0},{0,2,0,1,0},{1,2,1,1,1},{1,1,0,2,1},{1,1,0,2,1},{1,1,0,1,1} };
    public int[,] buildingMap3  = new int[7,5]{ {0,0,0,0,0},{0,0,0,1,0},{0,1,1,1,0},{0,2,2,1,0},{0,1,1,1,0},{0,1,2,2,0},{0,1,1,1,1} };
    public int[,] buildingMap4  = new int[7,5]{ {0,0,0,0,0},{1,1,1,1,1},{1,1,2,1,1},{1,2,1,2,1},{1,1,2,1,1},{1,0,1,0,1},{1,0,1,0,1} };
    public int[,] buildingMap5  = new int[7,5]{ {0,0,0,0,0},{0,0,0,0,0},{0,2,0,2,0},{1,1,2,1,1},{0,1,1,1,0},{0,1,2,1,0},{0,0,1,0,0} };
    public int[,] buildingMap6  = new int[7,5]{ {0,0,0,0,1},{0,0,0,0,2},{1,1,1,0,1},{2,0,1,0,2},{1,1,1,0,1},{1,0,0,0,2},{1,0,1,1,1} };
    public int[,] buildingMap7  = new int[7,5]{{1,1,1,1,1},{1,2,1,2,1},{1,1,2,1,1},{1,2,1,2,1},{1,1,1,1,1},{1,1,1,1,1},{1,1,1,1,1} };
    public int[,] buildingMap8  = new int[7,5]{ {1,1,1,1,1},{1,0,0,0,1},{2,0,2,0,2},{1,1,2,1,1},{0,1,2,1,0},{0,2,2,2,0},{0,1,0,1,0} };







    private int[,] buildingMap;

    public Cell[,] gridCells; // 2D array to hold references to each cell  - (col, row)

    void Awake(){
        gridder = this;
        int mapSelector = Random.Range(1,9);

        if(mapSelector == 1){
            buildingMap = buildingMap1;
        }
        else if(mapSelector == 2){
            buildingMap = buildingMap2;
        }
        else if(mapSelector == 3){
            buildingMap = buildingMap3;
        }
        else if(mapSelector == 4){
            buildingMap = buildingMap4;
        }
        else if(mapSelector == 5){
            buildingMap = buildingMap5;
        }
        else if(mapSelector == 6){
            buildingMap = buildingMap6;
        }
        else if(mapSelector == 7){
            buildingMap = buildingMap7;
        }
        else if(mapSelector == 8){
            buildingMap = buildingMap8;
        }
    }

    void Start(){
        gridCells = new Cell[cols,rows];
        
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 cellPosition = transform.position + new Vector3((x * cellSize + cellSize / 2) - ((float)cols/2), y * cellSize + cellSize / 2, 0);

                // Create Cell.
                Cell newCell = CreateObjectAtCenterOfCell(x,y,cell).GetComponent<Cell>();
                gridCells[x, y] = newCell;
                newCell.coOrdXY = new Vector2Int(x, y);

                // Create Block.
                //if(Random.value > 0.3f){
                if(buildingMap[(rows-1)-y,x] == 1){
                    // Create Block
                    Block newBlock = CreateObjectAtCenterOfCell(x,y,block).GetComponent<Block>();
                    newBlock.hostCell = newCell;
                    newBlock.coOrdXY = new Vector2Int(x, y);
                    newCell.containedBlock = newBlock;

                    if(Random.value <= 0.05f){
                        newBlock.hasTank = true;
                        newBlock.tank.SetActive(true);
                    }

                }else if(buildingMap[(rows-1)-y,x] == 2){
                    // Create Citizen
                    Block newBlock  = CreateObjectAtCenterOfCell(x,y,block).GetComponent<Block>();
                    newBlock.hostCell = newCell;
                    newBlock.coOrdXY = new Vector2Int(x, y);
                    newCell.containedBlock = newBlock;

                    newBlock.isInterior = true;
                    GameManager.gm.totalCitizens ++;
                    newBlock.BreakingPoint = 2;
                }

                if(y == 0){
                    newCell.grass.SetActive(true);
                }

                
            }
        }

        for (int row = 0; row < gridCells.GetLength(1); row++)
        {
            for (int col = 0; col < gridCells.GetLength(0); col++)
            {
                Cell thiscell = gridCells[col, row];
                if(thiscell.containedBlock != null){
                    thiscell.containedBlock.StyleBlock();
                }
            }
        }

        GameManager.gm.remainingCitizens = GameManager.gm.totalCitizens;
        GameManager.gm.DrawPeepScoreboard();

        ready = true;
    }

    public GameObject CreateObjectAtCenterOfCell(int col, int row, GameObject objPrefab)
    {
        // Calculate position of the center of the specified cell
        float xPos = (col * cellSize + cellSize / 2) - ((float)cols/2);
        float yPos = transform.position.y + row * cellSize + cellSize / 2;
        Vector3 position = new Vector3(xPos, yPos, 0);

        // Instantiate objectPrefab at the calculated position
        GameObject newObj = Instantiate(objPrefab, position, Quaternion.identity);

        return newObj;
    }

    


    private void OnDrawGizmos()
    {
        // Draw grid with gizmos
        /*Gizmos.color = Color.cyan;
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 cellPosition = transform.position + new Vector3((x * cellSize + cellSize / 2) - ((float)cols/2),  y * cellSize + cellSize / 2, 0);
                Gizmos.DrawWireCube(cellPosition, new Vector3(cellSize, cellSize, 0.1f));
            }
        }*/
    }
}
