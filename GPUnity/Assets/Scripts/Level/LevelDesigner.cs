using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// Wafe Function Collapse
using UnityEngine.Tilemaps;
using WaveFunctionCollaps;

public enum Direction
{
    top, right, left, bottom, none
}

public class LevelDesigner : MonoBehaviour
{

    private int LEVEL_WIDTH = 7; // Cells
    private int LEVEL_HEIGHT = 7; // Cells

    private int CELL_WIDTH = 20; // Tiles
    private int CELL_HEIGHT = 10; // Tiles

    private enum CellMode {
        RB, RBL, BL, TRB, TRBL, TBL, TR, TRL, TL, R, L, RL, TB, T, B
    }

    private WaveFunctionCollapse wfc;

    public GameObject endPanel;
    public GameObject portal;
    public PlayerPlatformerController player;

    public Tilemap inputTilemap;
    public Tilemap outupTilemap;

    public GameObject[] RBCells;
    public GameObject[] RBLCells;
    public GameObject[] BLCells;

    public GameObject[] TRBCells;
    public GameObject[] TRBLCells;
    public GameObject[] TBLCells;

    public GameObject[] TRCells;
    public GameObject[] TRLCells;
    public GameObject[] TLCells;

    public GameObject[] RCells;
    public GameObject[] LCells;
    public GameObject[] RLCells;

    public GameObject[] TBCells;
    public GameObject[] TCells;
    public GameObject[] BCells;

    // Start is called before the first frame update
    void Start()
    {

        Mode gameMode = LevelModeSetup.mode;
        List<CellMode> proceduralGeneration = new List<CellMode>();
        
        /*
            WAVE FUNCTION COLLAPSE
        */
        if (gameMode == Mode.Wfc)
        {
            wfc = new WaveFunctionCollapse(inputTilemap, outupTilemap, 1, 7, 7, 500, true);
            wfc.CreateNewTileMap();
            Tilemap tl = wfc.GetOutputTileMap();
            BoundsInt bounds = tl.cellBounds;
            TileBase[] allTiles = tl.GetTilesBlock(bounds);


            // 5 tries to create a level with wfc
            int triesMax = 5;
            int tries = 0;

            while (allTiles.Length == 0 || tries < triesMax)
            {
                Debug.Log("tries");
                Debug.Log(tries);
                wfc.CreateNewTileMap();
                tl = wfc.GetOutputTileMap();
                bounds = tl.cellBounds;
                allTiles = tl.GetTilesBlock(bounds);

                tries++;
            }

            // Create a level with wfc tiles
            if (allTiles.Length > 0) {

                setPGFromWFCOutput(allTiles, proceduralGeneration);

            } else {

                // Use Loris code
                for (int i = 0; i < LEVEL_WIDTH * LEVEL_HEIGHT; i++) {
                    proceduralGeneration.Add(CellMode.R);
                }

            }
        }

        /*
            Safety
        */
        if (gameMode == Mode.Safety)
        {
            // Use loris code
        }

        // Instantiate level
        for (int i = 0; i < proceduralGeneration.Count; i++)
        {
            instantiateFor(proceduralGeneration[i], i);
        }

        // Place end portal
        PlacePortal(proceduralGeneration);

    }

    /*
        Intantiate cell from cellmode
    */
    private void instantiateFor(CellMode mode, int index)
    {
        GameObject cell;
        int random = 0;

        switch (mode)
        {
            case CellMode.RB:
                random = Random.Range(0, RBCells.Length);
                cell = RBCells[random];
                break;
            case CellMode.RBL:
                random = Random.Range(0, RBLCells.Length);
                cell = RBLCells[random];
                break;
            case CellMode.BL:
                random = Random.Range(0, BLCells.Length);
                cell = BLCells[random];
                break;

            case CellMode.TRB:
                random = Random.Range(0, TRBCells.Length);
                cell = TRBCells[random];
                break;
            case CellMode.TRBL:
                random = Random.Range(0, TRBLCells.Length);
                cell = TRBLCells[random];
                break;
            case CellMode.TBL:
                random = Random.Range(0, TBLCells.Length);
                cell = TBLCells[random];
                break;

            case CellMode.TR:
                random = Random.Range(0, TRCells.Length);
                cell = TRCells[random];
                break;
            case CellMode.TRL:
                random = Random.Range(0, TRLCells.Length);
                cell = TRLCells[random];
                break;
            case CellMode.TL:
                random = Random.Range(0, TLCells.Length);
                cell = TLCells[random];
                break;

            case CellMode.R:
                random = Random.Range(0, RCells.Length);
                cell = RCells[random];
                break;
            case CellMode.L:
                random = Random.Range(0, LCells.Length);
                cell = LCells[random];
                break;
            case CellMode.RL:
                random = Random.Range(0, RLCells.Length);
                cell = RLCells[random];
                break;

            case CellMode.TB:
                random = Random.Range(0, TBCells.Length);
                cell = TBCells[random];
                break;
            case CellMode.T:
                random = Random.Range(0, TCells.Length);
                cell = TCells[random];
                break;
            case CellMode.B:
                random = Random.Range(0, BCells.Length);
                cell = BCells[random];
                break;

            default:
                cell = RBCells[0];
                break;
        }

        int x = index % LEVEL_WIDTH;
        int y = (int)(index / LEVEL_WIDTH);

        Instantiate(cell, new Vector3(x * CELL_WIDTH, -y * CELL_HEIGHT, 0), Quaternion.identity);
    }

    /*
        Place end portal
        This is a custom algorithm so it is not optimize but it seems to work
    */
    private List<int> visitedCellsIndex = new List<int>();
    private void PlacePortal(List<CellMode> proceduralGeneration) 
    {

        // FROM procedural generation 
        // Find the longest path and place portal
        //portal.transform.position = new Vector3(0, -CELL_HEIGHT*(LEVEL_HEIGHT - 1), 0);

        bool findEnd = false;
        int currentCellIndex = 0;
        Direction previousDirection = Direction.left;

        List<Direction> availableDirections = new List<Direction>{Direction.top, Direction.right, Direction.bottom, Direction.left};

        while (!findEnd)
        {

            CellMode currentCell = proceduralGeneration[currentCellIndex];
            List<Direction> currentCellAvailableDirections = getDirections(currentCell, currentCellIndex);
            currentCellAvailableDirections.Remove(previousDirection);
            
            // Get an available direction from current cell
            Direction direction;
            if (currentCellAvailableDirections.Count > 0) 
            {
                int random = Random.Range(0, currentCellAvailableDirections.Count - 1);
                direction = currentCellAvailableDirections[random]; 
            }
            else 
            {
                direction = Direction.right;
            }

            previousDirection = getOpposite(direction);

            // Move to the next cell
            int nextCellIndex = GetNextCellIndex(currentCellIndex, direction);
            if (nextCellIndex < 0 || nextCellIndex > LEVEL_WIDTH * LEVEL_HEIGHT - 1) 
            {
                findEnd = true; 
                break;
            }

            // If the algorithm find a path we can continue
            if (IsPathAvailable(proceduralGeneration, direction, nextCellIndex))
            {
                visitedCellsIndex.Add(currentCellIndex);
                currentCellIndex = nextCellIndex;
            } 
            else
            {
                findEnd = true;    
            }

        }

        // Place the portal where this algorithm tells us
        int cellXPosition = currentCellIndex % LEVEL_WIDTH;
        int cellYPosition = currentCellIndex / LEVEL_WIDTH;
        portal.transform.position = new Vector3(cellXPosition * CELL_WIDTH, -cellYPosition * CELL_HEIGHT, 0);

    }

    /*
        Check if the path is available from the next cell and the direction
    */
    private bool IsPathAvailable(List<CellMode> cells, Direction direction, int nextCellIndex)
    {

        if (visitedCellsIndex.Contains(nextCellIndex)) 
        {
            return false;
        }

        int nextCellX = nextCellIndex % LEVEL_WIDTH;
        int nextCellY = nextCellIndex / LEVEL_WIDTH;

        if (nextCellX < 0 || nextCellX >= LEVEL_WIDTH || nextCellY < 0 || nextCellY >= LEVEL_HEIGHT)
        {
            return false;
        }

        if (getDirections(cells[nextCellIndex], -1).Contains(getOpposite(direction)))
        {
            return true;
        } 
        else
        {
            return false;
        }
        
    }

    /*
        Get next cell index from current cell and next direction
    */
    private int GetNextCellIndex(int currentCellIndex, Direction direction)
    {
        int x = 0;
        int y = 0;
        switch (direction)
        {
            case Direction.top:
                y = -1;
                break;
            case Direction.right:
                x = 1;
                break;
            case Direction.bottom:
                y = 1;
                break;
            case Direction.left:
                x = -1;
                break;
            default:
                break;
        }

        int currentX = currentCellIndex % LEVEL_WIDTH;
        int currentY = currentCellIndex / LEVEL_WIDTH;

        int nextX = currentX + x;
        int nextY = currentY + y;

        return nextY * LEVEL_WIDTH + nextX; 
    }

    /*
        Get opposite direction
    */
    private Direction getOpposite(Direction direction) 
    {
        Direction opposite;
        switch (direction)
        {
            case Direction.top:
                opposite = Direction.bottom;
                break;
            case Direction.right:
                opposite = Direction.left;
                break;
            case Direction.bottom:
                opposite = Direction.top;
                break;
            case Direction.left:
                opposite = Direction.right;
                break;
            default:
                opposite = Direction.none;
                break;
        }
        return opposite;
    }

    /*
        Get all the directions for a cellmode
    */
    private List<Direction> getDirections(CellMode cm, int index) 
    {

        List<Direction> directions = new List<Direction>();

        switch (cm)
        {
            case CellMode.RB:
                directions.Add(Direction.right);
                directions.Add(Direction.bottom);
                break;
            case CellMode.RBL:
                directions.Add(Direction.right);
                directions.Add(Direction.bottom);
                directions.Add(Direction.left);
                break;
            case CellMode.BL:
                directions.Add(Direction.bottom);
                directions.Add(Direction.left);
                break;

            case CellMode.TRB:
                directions.Add(Direction.top);
                directions.Add(Direction.right);
                directions.Add(Direction.bottom);
                break;
            case CellMode.TRBL:
                directions.Add(Direction.top);
                directions.Add(Direction.right);
                directions.Add(Direction.bottom);
                directions.Add(Direction.left);
                break;
            case CellMode.TBL:
                directions.Add(Direction.top);
                directions.Add(Direction.bottom);
                directions.Add(Direction.left);
                break;

            case CellMode.TR:
                directions.Add(Direction.top);
                directions.Add(Direction.right);
                break;
            case CellMode.TRL:
                directions.Add(Direction.top);
                directions.Add(Direction.right);
                directions.Add(Direction.left);
                break;
            case CellMode.TL:
                directions.Add(Direction.right);
                directions.Add(Direction.left);
                break;

            case CellMode.R:
                directions.Add(Direction.right);
                break;
            case CellMode.L:
                directions.Add(Direction.left);
                break;
            case CellMode.RL:
                directions.Add(Direction.right);
                directions.Add(Direction.left);
                break;

            default:
                break;
        }

        if (index != -1) {
            int x = index % LEVEL_WIDTH;
            int y = index / LEVEL_WIDTH;

            if (x == 0)
            {
                directions.Remove(Direction.left);
            }
            if (x == LEVEL_WIDTH)
            {
                directions.Remove(Direction.right);
            }
            if (y == 0)
            {
                directions.Remove(Direction.top);
            }
            if (y == LEVEL_HEIGHT)
            {
                directions.Remove(Direction.bottom);
            }
        }

        return directions;
    }

    /*
        This function create the list of cellmode from wfc output
    */
    private void setPGFromWFCOutput(TileBase[] tiles, List<CellMode> proceduralGeneration) 
    {
        
        if (tiles.Length == LEVEL_WIDTH * LEVEL_WIDTH) {

            // WFC output and procedural generation don't handle cell in the same order
            // So I fix it here by reversing tiles rows

            List<List<TileBase>> ll = new List<List<TileBase>>();

            // 1. Get tiles as List of List
            for (int j = 0; j < LEVEL_HEIGHT; j++) {

                List<TileBase> row = new List<TileBase>();
                for (int i = 0; i < LEVEL_WIDTH; i++) {

                    row.Add(tiles[j*LEVEL_WIDTH + i]);

                }
                ll.Add(row);

            }

            // 2. Rearange List (as row)
            List<List<TileBase>> llreverse = new List<List<TileBase>>();
            llreverse.Add(new List<TileBase>());
            llreverse.Add(new List<TileBase>());
            llreverse.Add(new List<TileBase>());
            llreverse.Add(new List<TileBase>());
            llreverse.Add(new List<TileBase>());
            llreverse.Add(new List<TileBase>());
            llreverse.Add(new List<TileBase>());


            for (int index = 0; index < ll.Count; index++)
            {
                llreverse[LEVEL_HEIGHT - index - 1] = ll[index];
            }

            // 3. Get through List of List and ADD to procedural generation
            TileBase[] orderedTiles = new TileBase[LEVEL_WIDTH * LEVEL_HEIGHT];
            for (int collumn = 0; collumn < llreverse.Count; collumn++) 
            {
                for (int row = 0; row < llreverse[collumn].Count; row++)
                {
                    orderedTiles[collumn*LEVEL_WIDTH + row] = llreverse[collumn][row];
                }
            }

            
            // Loop through ordered tiles and add cellmode
            foreach (TileBase tile in orderedTiles) 
            {
                string name = tile.name;
                Debug.Log(name);
                switch (name)
                {
                    case "RB":
                        proceduralGeneration.Add(CellMode.RB);
                        break;
                    case "RBL":
                        proceduralGeneration.Add(CellMode.RBL);
                        break;
                    case "BL":
                        proceduralGeneration.Add(CellMode.BL);
                        break;

                    case "TRB":
                        proceduralGeneration.Add(CellMode.TRB);
                        break;
                    case "TRBL":
                        proceduralGeneration.Add(CellMode.TRBL);
                        break;
                    case "TBL":
                        proceduralGeneration.Add(CellMode.TBL);
                        break;

                    case "TR":
                        proceduralGeneration.Add(CellMode.TR);
                        break;
                    case "TRL":
                        proceduralGeneration.Add(CellMode.TRL);
                        break;
                    case "TL":
                        proceduralGeneration.Add(CellMode.TL);
                        break;

                    case "R":
                        proceduralGeneration.Add(CellMode.R);
                        break;
                    case "L":
                        proceduralGeneration.Add(CellMode.L);
                        break;
                    case "RL":
                        proceduralGeneration.Add(CellMode.RL);
                        break;

                    
                    case "TB":
                        proceduralGeneration.Add(CellMode.TB);
                        break;

                    default:
                        proceduralGeneration.Add(CellMode.RB);
                        break;
                }
            }

        }

    }

    /*
        Callback when the level is completed
    */
    public void LevelCompleted() 
    {
        player.FoundPortal();
        endPanel.SetActive(true);
    }

    /*
        Load menu scene
    */
    public void GoToMenu() 
    {
        // 0 is the index of the menu scene on build settings;
        SceneManager.LoadScene(0);
    }

}
