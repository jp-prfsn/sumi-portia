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

    public int[,] buildingMap  = new int[7,5]{ {0,0,0,0,0},{0,1,0,1,0},{0,1,1,1,0},{1,1,1,1,1},{0,1,2,1,0},{1,1,1,1,1},{0,1,1,1,0} };

    public Cell[,] gridCells; // 2D array to hold references to each cell  - (col, row)

    void Awake(){
        gridder = this;
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

                }else if(buildingMap[(rows-1)-y,x] == 2){
                    // Create Citizen
                    Block newBlock  = CreateObjectAtCenterOfCell(x,y,block).GetComponent<Block>();
                    newBlock.hostCell = newCell;
                    newBlock.coOrdXY = new Vector2Int(x, y);
                    newCell.containedBlock = newBlock;

                    newBlock.isBlock = false;
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
        Gizmos.color = Color.cyan;
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 cellPosition = transform.position + new Vector3((x * cellSize + cellSize / 2) - ((float)cols/2),  y * cellSize + cellSize / 2, 0);
                Gizmos.DrawWireCube(cellPosition, new Vector3(cellSize, cellSize, 0.1f));
            }
        }
    }
}
