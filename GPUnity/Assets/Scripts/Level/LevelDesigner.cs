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

    private const int LEVEL_WIDTH = 7; // Cells
    private const int LEVEL_HEIGHT = 7; // Cells

    private int CELL_WIDTH = 20; // Tiles
    private int CELL_HEIGHT = 10; // Tiles

    private enum CellMode {
        RB, RBL, BL, TRB, TRBL, TBL, TR, TRL, TL, R, L, RL, TB, T, B
    }

    private WaveFunctionCollapse wfc;

    public Transform[] startingPositions; // Make gameObject at zero zero
    public GameObject endPanel;
    public GameObject portal;
    private int portalIndex = 0;
    public PlayerPlatformerController player;

    private int direction;
    private float distanceLR = 20.0f;
    private float distanceTB = 10.0f;

    private float minX = 0.0f;
    private float maxX = 120.0f;
    private float minY = -60.0f;
    private float beginX = 0.0f;
    private float beginY = 0.0f;
    private bool stopGeneration = false;
    private bool firstCell = true;

    private List<float> indexsOfCellsInPath = new List<float>();
    private List<float> indexsOfCellsNotInPath = new List<float>();

    private CellMode[] All_choices = new CellMode[] {CellMode.L,CellMode.R,CellMode.T,CellMode.B,CellMode.RL,CellMode.TL,CellMode.BL,CellMode.TR,CellMode.RB,CellMode.TB,CellMode.TBL,CellMode.RBL,CellMode.TRL,CellMode.TRB,CellMode.TRBL};
    private CellMode[] L_choices = new CellMode[] {CellMode.L,CellMode.BL,CellMode.TL,CellMode.TBL,CellMode.RBL,CellMode.TRL,CellMode.RL,CellMode.TRBL};
    private CellMode[] R_choices = new CellMode[] {CellMode.R,CellMode.RB,CellMode.TR,CellMode.TRB,CellMode.RBL,CellMode.TRL,CellMode.RL,CellMode.TRBL};
    private CellMode[] T_choices = new CellMode[] {CellMode.T,CellMode.TB,CellMode.TL,CellMode.TR,CellMode.TRL,CellMode.TBL,CellMode.TRB,CellMode.TRBL};
    private CellMode[] B_choices = new CellMode[] {CellMode.B,CellMode.TB,CellMode.BL,CellMode.RB,CellMode.RBL,CellMode.TBL,CellMode.TRB,CellMode.TRBL};
    private CellMode[] BL_choices = new CellMode[] {CellMode.BL,CellMode.RBL,CellMode.TBL,CellMode.TRBL};
    private CellMode[] TL_choices = new CellMode[] {CellMode.TL,CellMode.TRL,CellMode.TBL,CellMode.TRBL};
    private CellMode[] LR_choices = new CellMode[] {CellMode.RL,CellMode.TRL,CellMode.RBL,CellMode.TRBL};
    private CellMode[] BR_choices = new CellMode[] {CellMode.RB,CellMode.TRB,CellMode.RBL,CellMode.TRBL};
    private CellMode[] TR_choices = new CellMode[] {CellMode.TR,CellMode.TRB,CellMode.TRL,CellMode.TRBL};
    private CellMode[] BT_choices = new CellMode[] {CellMode.TB,CellMode.TRB,CellMode.TBL,CellMode.TRBL};

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
            wfc = new WaveFunctionCollapse(inputTilemap, outupTilemap, 1, LEVEL_WIDTH, LEVEL_HEIGHT, 500, true);
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

                proceduralGeneration = generateMap();

            }
        }

        /*
            Safety
        */
        if (gameMode == Mode.Safety)
        {
            proceduralGeneration = generateMap();
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
        Mode gameMode = LevelModeSetup.mode;
        if (gameMode == Mode.Wfc)
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

            portalIndex = currentCellIndex;

        }


        // Place the portal where this algorithm tells us
        int cellXPosition = portalIndex % LEVEL_WIDTH;
        int cellYPosition = portalIndex / LEVEL_WIDTH;
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

    //Créé le path et ajoute les index des cell devant être dans le path dans un tableau
    private List<CellMode> generateMap()
    {
        List<CellMode> finalCellModeList = new List<CellMode>();
        
        initPath();
        
        while (stopGeneration == false)
        {
            createPath();
        }
        List<CellMode> cellModeInPaths = new List<CellMode>();
        cellModeInPaths = generateCellsInPath();
        //generateCellsInPath();

        List<CellMode> cellModeNotInPaths = new List<CellMode>();
        cellModeNotInPaths = generateCellsNotInPath();
        //generateCellsNotInPath();

        finalCellModeList = createFinaListOfCellMode(cellModeInPaths, cellModeNotInPaths);
        return finalCellModeList;
    }


    private void initPath()
    {
        int randomStartingPosition = UnityEngine.Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randomStartingPosition].position;

        indexsOfCellsInPath.Add(computeIndexOfCell(transform.position.x, transform.position.y));
        direction = UnityEngine.Random.Range(1, 4);
    }

    private void createPath()
    {
        if (direction == 1) //Move right
        {
            if (transform.position.x < maxX)
            {
                Vector2 newPosition = new Vector2(transform.position.x + distanceLR, transform.position.y);
                transform.position = newPosition;
                //Ajoute dans un tableau l'emplacement de la nouvelle cell 
                indexsOfCellsInPath.Add(computeIndexOfCell(transform.position.x, transform.position.y));
       
                direction = UnityEngine.Random.Range(1, 4);
                while (direction == 2)  //Tout sauf Move left
                {
                    direction = UnityEngine.Random.Range(1, 4);
                }
            }
            else {  //Move Bottom ou left 
                if (firstCell) //Point de départ 
                {
                    direction = UnityEngine.Random.Range(2, 4);
                } else // Move bottom
                {
                    direction = 3;
                }  
            }
        } else if (direction == 2) //Move left
        {
            if (transform.position.x > minX)
            {
                Vector2 newPosition = new Vector2(transform.position.x - distanceLR, transform.position.y);
                transform.position = newPosition;
                //Ajoute dans un tableau l'emplacement de la nouvelle cell 
                indexsOfCellsInPath.Add(computeIndexOfCell(transform.position.x, transform.position.y));

                direction = UnityEngine.Random.Range(2, 4); //Tout sauf Move right
            }
            else {  //Move Bottom ou right
                if (firstCell) //Point de départ 
                {
                    direction = UnityEngine.Random.Range(1, 4);
                    while (direction == 2)  //Tout sauf Move left
                    {
                        direction = UnityEngine.Random.Range(1, 4);
                    }
                } else // Move bottom
                {
                    direction = 3;
                } 
            }
        } else if  (direction == 3) //Move bottom
        {
            // A VERIFIER
            if (transform.position.y > minY)
            {
                Vector2 newPosition = new Vector2(transform.position.x, transform.position.y - distanceTB);
                transform.position = newPosition;
                //Ajoute dans un tableau l'emplacement de la nouvelle cell 
                indexsOfCellsInPath.Add(computeIndexOfCell(transform.position.x, transform.position.y));

                direction = UnityEngine.Random.Range(1, 4); 
            } else 
            {
                //STOP GENERATION
                stopGeneration = true;
            }
        }
        if(firstCell == true)
        {
            firstCell = false;
        }
    }



    //Pour chaque index de la liste, on créé un code qui permettra de définir le type de CellMode qui doit correspondre
    private List<CellMode> generateCellsInPath()
    //public void generateCellsInPath()
    {
        List<CellMode> cellModeInPaths = new List<CellMode>();
        Debug.Log("nb d'index : "+indexsOfCellsInPath.Count);
        for (int i = 0; i < indexsOfCellsInPath.Count; i++)
        {
            int codeCell = computeCellCode(i);
            cellModeInPaths.Add(createCellModeWithCode(codeCell, i));
            //createCellModeWithCode(codeCell, i);
        }
        portalIndex = (int)indexsOfCellsInPath[indexsOfCellsInPath.Count - 1];
        return cellModeInPaths;
    }

    private List<CellMode> generateCellsNotInPath()
    {
        List<CellMode> cellModeNotInPaths = new List<CellMode>();
        
        addIdxOfCellsNotInPath();
        cellModeNotInPaths = finishMapWithRandomCells();

        return cellModeNotInPaths;
    }


    private List<CellMode> createFinaListOfCellMode(List<CellMode> cellsInPath, List<CellMode> cellsNotInPath)
    {
        List<CellMode> finalCellModeList = new List<CellMode>();
        for (int i = 0; i < (LEVEL_HEIGHT*LEVEL_WIDTH); i++)
        {
            if (isInList(indexsOfCellsInPath, i))
            {
                int posOfI = indexsOfCellsInPath.IndexOf(i);
                Debug.Log("posOfI : "+posOfI);
                finalCellModeList.Add(cellsInPath[posOfI]);
            } else 
            {
                finalCellModeList.Add(cellsNotInPath[0]);
                cellsNotInPath.RemoveAt(0);
            }
        }
        return finalCellModeList;
    } 

    //Calcule l'index de la cell
    public float computeIndexOfCell(float positionX, float positionY)
    {
        float index = ((positionX-beginX)/distanceLR)+((beginY-positionY)/distanceTB)*(LEVEL_WIDTH);
        Debug.Log("pos X :"+positionX+" Y : "+positionY+" index = "+index);
        return index;
    }

    //Créé un code par cell qui sera décodé pour déterminer le type de cell à instancier
    public int computeCellCode(float index)
    {
        int codeValue = 0;
        if (index != 0) //Si c'est pas la première cellule on fait le diffPrevious
        {
            int diffPrevious = (int)(indexsOfCellsInPath[(int)index]-indexsOfCellsInPath[(int)(index-1)]);
            switch (diffPrevious)
            {
                case 1:    //Déplacement vers la droite
                    codeValue += 10;
                    break; 
                case LEVEL_WIDTH:     //Déplacement vers le bas
                    codeValue += 20;
                    break;
                case -LEVEL_WIDTH:    //Déplacement vers le haut 
                    codeValue += 30;
                    break;
                case -1:   //Déplacement vers la gauche
                    codeValue += 40;
                    break;
                default:
                    Debug.Log("faut case in switch)");
                    break;
            }
        }

        if (index != indexsOfCellsInPath.Count-1)
        {
            int diffAfter = (int)(indexsOfCellsInPath[(int)(index+1)]-indexsOfCellsInPath[(int)index]);
            switch (diffAfter)
            {
                case 1:    //Déplacement vers la droite
                    codeValue += 1;
                    break; 
                case LEVEL_WIDTH:     //Déplacement vers le bas
                    codeValue += 2;
                    break;
                case -LEVEL_WIDTH:    //Déplacement vers le haut
                    codeValue += 3;
                    break;
                case -1:   //Déplacement vers la gauche
                    codeValue += 4;
                    break;
                default:
                    Debug.Log("faut case in switch)");
                    break;
            }
        }
        Debug.Log("index : "+index+" codeValue : "+codeValue);
        return codeValue;
    }



    //En fonction du code créé pour la cell, on définit le type de cell à instancier et on l'instancie
    private CellMode createCellModeWithCode(int cellCode, int posInIndexsOfCellsInPath)
    {
        CellMode typeOfCell = CellMode.TRBL;
        //int typeOfCell = 0;
        int rand;
        switch (cellCode)
        {
            case 10:    //1 déplacement vers la droite -> L
                rand = UnityEngine.Random.Range(0, 8);
                typeOfCell = L_choices[rand];
                break;
            case 20:    //1 déplacement vers le bas -> B
                rand = UnityEngine.Random.Range(0, 8);
                typeOfCell = T_choices[rand];
                break;
            case 30:    //1 déplacement vers le haut -> T
                rand = UnityEngine.Random.Range(0, 8);
                typeOfCell = T_choices[rand];
                break;
            case 40:    //1 déplacement vers la gauche -> R
                rand = UnityEngine.Random.Range(0, 8);
                typeOfCell = R_choices[rand];
                break;

            case 1:    //1 déplacement vers la droite -> R
                rand = UnityEngine.Random.Range(0, 8);
                typeOfCell = R_choices[rand];
                break;
            case 2:    //1 déplacement vers le bas -> B
                rand = UnityEngine.Random.Range(0, 8);
                typeOfCell = B_choices[rand];
                break;
            case 3:    //1 déplacement vers le haut -> T
                rand = UnityEngine.Random.Range(0, 8);
                typeOfCell = T_choices[rand];
                break;
            case 4:    //1 déplacement vers la gauche -> L
                rand = UnityEngine.Random.Range(0, 8);
                typeOfCell = L_choices[rand];
                break;

            case 11:    //2 déplacement vers la droite -> LR
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = LR_choices[rand];
                break;
            case 12:    //1 déplacement vers la droite puis 1 vers le bas -> LB
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = BL_choices[rand];
                break;
            case 13:    //1 déplacement vers la droite puis 1 vers le haut -> LT
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = TL_choices[rand];
                break;
            case 14:    //1 déplacement vers la droite puis 1 vers la gauche -> Normalement impossible
                Debug.Log("Case 14, normalement pas possible");
                // rand = UnityEngine.Random.Range(0, 4);
                // typeOfCell = LB_choices[rand];
                break;
            
            case 21:    //1 déplacement vers le bas puis 1 vers la droite -> TR
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = TR_choices[rand];
                break;
            case 22:    //2 déplacement vers le bas -> TB
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = BT_choices[rand];
                break;
            case 23:    //1 déplacement vers le bas puis 1 vers le haut -> Normalement impossible
                Debug.Log("case 23, normalement impossible");
                // rand = UnityEngine.Random.Range(0, 4);
                // typeOfCell = LB_choices[rand];
                break;
            case 24:    //1 déplacement vers le bas puis 1 vers la gauche -> TL
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = TL_choices[rand];
                break;   
            
            case 31:    //1 déplacement vers le haut puis 1 vers la droite -> BR
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = BR_choices[rand];
                break;
            case 32:    //1 déplacement vers le haut puis un vers le bas -> Normalement impossible
                Debug.Log("case 32, normalement impossible");
                // rand = UnityEngine.Random.Range(0, 4);
                // typeOfCell = TB_choices[rand];
                break;
            case 33:    //2 déplacements vers le haut -> BT  
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = BT_choices[rand];
                break;
            case 34:    //1 déplacement vers le haut puis 1 vers la gauche -> BL
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = BL_choices[rand];
                break;  
            
            case 41:    //1 déplacement vers la gauche puis 1 vers la droite -> Normalement impossible
                Debug.Log("case 41, normalement impossible");
                // rand = UnityEngine.Random.Range(0, 4);
                // typeOfCell = BR_choices[rand];
                break;
            case 42:    //1 déplacement vers la gauche puis un vers le bas -> BR
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = BR_choices[rand];
                break;
            case 43:    //1 déplacement vers la gauche puis un vers le haut -> TR  
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = TR_choices[rand];
                break;
            case 44:    //2 déplacements vers la gauche -> LR
                rand = UnityEngine.Random.Range(0, 4);
                typeOfCell = LR_choices[rand];
                break;    
        }
        return typeOfCell;
    }


    public float computeXOfCellFromIndex(List<float> list, int idxInList)
    {
        float indexOfCell = list[idxInList];
        float idxY = (int)(Mathf.Floor(indexOfCell/(LEVEL_WIDTH)));
        float idxX = indexOfCell-(LEVEL_WIDTH*idxY);
        float posX = idxX*distanceLR + beginX; 
        return posX;
    }

    public float computeYOfCellFromIndex(List<float> list, int idxInList)
    {
        float indexOfCell = list[idxInList];
        float idxY = (int)(Mathf.Floor(indexOfCell/(LEVEL_WIDTH)));
        float posY = -1*(idxY*distanceTB-beginY);
        return posY;
    }

   

    //Remplis une liste avec les indexs non présents dans la liste des cellInPath
    public void addIdxOfCellsNotInPath()
    {
        for (int i = 0; i <= (LEVEL_WIDTH*LEVEL_HEIGHT-1); i++)
        {
            if(isInList(indexsOfCellsInPath, i) == false)
            {
                Debug.Log("add idx : "+i);
                indexsOfCellsNotInPath.Add((float)i);
            }
        }
    }

    //Instancie des cell aléatoires pour compléter la map
    private List<CellMode> finishMapWithRandomCells()
    {
        List<CellMode> cellModeNotInPaths = new List<CellMode>();
        for (int i = 0; i < indexsOfCellsNotInPath.Count; i++)
        {
            int randomCellMode = UnityEngine.Random.Range(0, 15);//Valeur brut pour le moment mais doit correspondre au nb de choix possibles dans l'enum
            CellMode typeOfCell = All_choices[randomCellMode];
            cellModeNotInPaths.Add(typeOfCell); 
        }  
        return cellModeNotInPaths;
    }

    // Renvoie true si l'index est présent dans la liste list
    public bool isInList(List<float> list, float index)
    {
        bool result = list.Exists(idx => idx == index);
        return result;
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
