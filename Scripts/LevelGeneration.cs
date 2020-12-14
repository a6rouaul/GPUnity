using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public Transform[] startingPositions;

    public enum CellMode {
        RB, RBL, BL, TRB, TRBL, TBL, TR, TRL, TL, R, L, RL, T, B, TB
    }

    private int direction;
    public float distanceLR;
    public float distanceTB;

    public float minX;
    public float maxX;
    public float minY;
    public float beginX;
    public float beginY;
    private bool stopGeneration = false;
    private bool firstCell = true;

    private List<float> indexsOfCellsInPath = new List<float>();
    private List<float> indexsOfCellsNotInPath = new List<float>();

    private const int nbCellsInLine = 4;
    private const int nbCellsInColumn = 6;

    //private int[15] cellsTypes = {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14};
    // private int[] L_choices = new int[] {0,6,5,13,10,11,4,14};
    // private int[] R_choices = new int[] {1,8,7,12,10,11,4,14};
    // private int[] T_choices = new int[] {2,9,5,7,11,13,12,14};
    // private int[] B_choices = new int[] {3,9,6,8,10,13,12,14};
    // private int[] BL_choices = new int[] {6,10,13,14};
    // private int[] TL_choices = new int[] {5,11,13,14};
    // private int[] LR_choices = new int[] {4,11,10,14};
    // private int[] BR_choices = new int[] {8,12,10,14};
    // private int[] TR_choices = new int[] {7,12,11,14};
    // private int[] BT_choices = new int[] {9,12,13,14};

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


    // Start is called before the first frame update
    void Start()
    {
        List<CellMode> cellModeList  = new List<CellMode>();
        cellModeList = generateMap();
    }

    public void logCellModeType(CellMode cell)
    {
        switch (cell)
        {
            case CellMode.T:
                Debug.Log("CellMode T");
                break;
            case CellMode.L:
                Debug.Log("CellMode L");
                break;
            case CellMode.R:
                Debug.Log("CellMode R");
                break;
            case CellMode.B:
                Debug.Log("CellMode B");
                break;
            case CellMode.RB:
                Debug.Log("CellMode RB");
                break;
            case CellMode.RBL:
                Debug.Log("CellMode RBL");
                break;
            case CellMode.BL:
                Debug.Log("CellMode BL");
                break;
            case CellMode.TRB:
                Debug.Log("CellMode TRB");
                break;
            case CellMode.TRBL:
                Debug.Log("CellMode TRBL");
                break;
            case CellMode.TBL:
                Debug.Log("CellMode TBL");
                break;
            case CellMode.TR:
                Debug.Log("CellMode TR");
                break;
            case CellMode.TRL:
                Debug.Log("CellMode TRL");
                break;
            case CellMode.TL:
                Debug.Log("CellMode TL");
                break;
            case CellMode.TB:
                Debug.Log("CellMode TB");
                break;
            case CellMode.RL:
                Debug.Log("CellMode RL");
                break;
            default:
                Debug.Log("No CellMode find");
                break;
        }
    }

    //Créé le path et ajoute les index des cell devant être dans le path dans un tableau
    public List<CellMode> generateMap()
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


    public void initPath()
    {
        int randomStartingPosition = UnityEngine.Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randomStartingPosition].position;

        indexsOfCellsInPath.Add(computeIndexOfCell(transform.position.x, transform.position.y));
        direction = UnityEngine.Random.Range(1, 4);
    }

    public void createPath()
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
    public List<CellMode> generateCellsInPath()
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
        return cellModeInPaths;
    }

    public List<CellMode> generateCellsNotInPath()
    {
        List<CellMode> cellModeNotInPaths = new List<CellMode>();
        
        addIdxOfCellsNotInPath();
        cellModeNotInPaths = finishMapWithRandomCells();

        return cellModeNotInPaths;
    }


    public List<CellMode> createFinaListOfCellMode(List<CellMode> cellsInPath, List<CellMode> cellsNotInPath)
    {
        List<CellMode> finalCellModeList = new List<CellMode>();
        for (int i = 0; i < (nbCellsInColumn*nbCellsInLine); i++)
        {
            if (isInList(indexsOfCellsInPath, i))
            {
                int posOfI = indexsOfCellsInPath.IndexOf(i);
                Debug.Log("posOfI : "+posOfI);
                finalCellModeList.Add(cellsInPath[posOfI]);
                logCellModeType(cellsInPath[posOfI]);
            } else 
            {
                finalCellModeList.Add(cellsNotInPath[0]);
                logCellModeType(cellsNotInPath[0]);
                cellsNotInPath.RemoveAt(0);
            }
        }
        return finalCellModeList;
    } 

    //Calcule l'index de la cell
    public float computeIndexOfCell(float positionX, float positionY)
    {
        float index = ((positionX-beginX)/distanceLR)+((beginY-positionY)/distanceTB)*(nbCellsInLine);
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
                case nbCellsInLine:     //Déplacement vers le bas
                    codeValue += 20;
                    break;
                case -nbCellsInLine:    //Déplacement vers le haut 
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
                case nbCellsInLine:     //Déplacement vers le bas
                    codeValue += 2;
                    break;
                case -nbCellsInLine:    //Déplacement vers le haut
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
    public CellMode createCellModeWithCode(int cellCode, int posInIndexsOfCellsInPath)
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
                typeOfCell = B_choices[rand];
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
        logCellModeType(typeOfCell);
        return typeOfCell;
    }


    public float computeXOfCellFromIndex(List<float> list, int idxInList)
    {
        float indexOfCell = list[idxInList];
        float idxY = (int)(Mathf.Floor(indexOfCell/(nbCellsInLine)));
        float idxX = indexOfCell-(nbCellsInLine*idxY);
        float posX = idxX*distanceLR + beginX; 
        return posX;
    }

    public float computeYOfCellFromIndex(List<float> list, int idxInList)
    {
        float indexOfCell = list[idxInList];
        float idxY = (int)(Mathf.Floor(indexOfCell/(nbCellsInLine)));
        float posY = -1*(idxY*distanceTB-beginY);
        return posY;
    }

   

    //Remplis une liste avec les indexs non présents dans la liste des cellInPath
    public void addIdxOfCellsNotInPath()
    {
        for (int i = 0; i <= (nbCellsInLine*nbCellsInColumn-1); i++)
        {
            if(isInList(indexsOfCellsInPath, i) == false)
            {
                Debug.Log("add idx : "+i);
                indexsOfCellsNotInPath.Add((float)i);
            }
        }
    }

    //Instancie des cell aléatoires pour compléter la map
    public List<CellMode> finishMapWithRandomCells()
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

    // Update is called once per frame
    void Update()
    {

    }
}
