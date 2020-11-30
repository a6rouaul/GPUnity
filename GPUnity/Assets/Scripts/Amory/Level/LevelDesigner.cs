using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// From RandomWalker Godot demo
// https://github.com/GDQuest/godot-procedural-generation






public class LevelDesigner : MonoBehaviour
{

    /*

    - Define the size of the level (Ex. 5 cells x 5 cells)
    - Define the size of a cell (Ex. 20 tiles x 10 tiles)
    - Define enumeration CellMode {LB, LBR, BR, TLB, TLBR, TBR, TL, TLR, TR}
    
    1. Have a list of all kind of level cell
        Opening on  LB      LBR     BR
                    TLB     TLBR    TBR
                    TL      TLR     TR

                    R       L       RL

    2. Have a procedural generation algorythm that tells us :
        Here you have a LB opening, here a TBR opening,...
        List of CellMode of size X * Y cells

    3. Take a random opening from lists above and create a level

    */

    private int LEVEL_WIDTH = 3; // Cells
    private int LEVEL_HEIGHT = 4; // Cells

    private int CELL_WIDTH = 20; // Tiles
    private int CELL_HEIGHT = 10; // Tiles

    private enum CellMode {
        RB, RBL, BL, TRB, TRBL, TBL, TR, TRL, TL, R, L, RL
    }


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

    // Start is called before the first frame update
    void Start()
    {

        // Should be given by an algorithm
        // CellMode[] proceduralGeneration = {
        //     CellMode.RB,
        //     CellMode.RBL,
        //     CellMode.BL,

        //     CellMode.TRB,
        //     CellMode.TRBL,
        //     CellMode.TBL,

        //     CellMode.TR,
        //     CellMode.TRL,
        //     CellMode.TL,

        //     CellMode.R,
        //     CellMode.RL,
        //     CellMode.L
        // };

        CellMode[] proceduralGeneration = {
            CellMode.R,
            CellMode.RL,
            CellMode.BL,

            CellMode.RB,
            CellMode.RL,
            CellMode.TL,

            CellMode.TR,
            CellMode.RL,
            CellMode.BL,

            CellMode.R,
            CellMode.RL,
            CellMode.TL
        };

        for (int i = 0; i < proceduralGeneration.Length; i++)
        {
            instantiateFor(proceduralGeneration[i], i);
        }

        // Draw border

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void instantiateFor(CellMode mode, int index)
    {
        GameObject cell;

        switch (mode)
        {
            case CellMode.RB:
                cell = RBCells[0];
                break;
            case CellMode.RBL:
                cell = RBLCells[0];
                break;
            case CellMode.BL:
                cell = BLCells[0];
                break;

            case CellMode.TRB:
                cell = TRBCells[0];
                break;
            case CellMode.TRBL:
                cell = TRBLCells[0];
                break;
            case CellMode.TBL:
                cell = TBLCells[0];
                break;

            case CellMode.TR:
                cell = TRCells[0];
                break;
            case CellMode.TRL:
                cell = TRLCells[0];
                break;
            case CellMode.TL:
                cell = TLCells[0];
                break;

            case CellMode.R:
                cell = RCells[0];
                break;
            case CellMode.L:
                cell = LCells[0];
                break;
            case CellMode.RL:
                cell = RLCells[0];
                break;

            default:
                cell = RBCells[0];
                break;
        }

        int x = index % LEVEL_WIDTH;
        int y = (int)(index / LEVEL_WIDTH);

        Instantiate(cell, new Vector3(x * CELL_WIDTH, -y * CELL_HEIGHT, 0), Quaternion.identity);

        // cell.transform.position = new Vector3(x * CELL_WIDTH, y * CELL_HEIGHT, 0);

    }


}
