// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using WaveFunctionCollaps;

// public class WFCExample : MonoBehaviour
// {
//     public Tilemap inputImage;
//     public Tilemap outputImage;
//     [Tooltip("For tiles usualy set to 1. If tile contain just a color can set to higher value")]
//     public int patternSize;
//     [Tooltip("How many times algorithm will try creating the output before quiting")]
//     public int maxIterations;
//     [Tooltip("Output image width")]
//     public int outputWidth = 5;
//     [Tooltip("Output image height")]
//     public int outputHeight = 5;
//     [Tooltip("Don't use tile frequency - each tile has equal weight")]
//     public bool equalWeights = false;
//     WaveFunctionCollapse wfc;

//     // Start is called before the first frame update
//     void Start()
//     {
//         // CreateWFC();
//         // CreateTilemap();
//         // SaveTilemap();
//     }

//     public void CreateWFC()
//     {
//         wfc = new WaveFunctionCollapse(this.inputImage, this.outputImage, patternSize, this.outputWidth, this.outputHeight, this.maxIterations, this.equalWeights);
//     }

//     public void CreateTilemap()
//     {
//         wfc.CreateNewTileMap();

//         // I don't want to draw output tilemap
//         Tilemap tl = wfc.GetOutputTileMap();

//         BoundsInt bounds = tl.cellBounds;
//         TileBase[] allTiles = tl.GetTilesBlock(bounds);

//         Debug.Log(tl.GetTilesBlock(bounds).Length);

//         if (allTiles.Length > 8) {
//             Debug.Log(allTiles[7].name);
//         }


//         // for (int x = 0; x < bounds.size.x; x++) {
//         //     for (int y = 0; y < bounds.size.y; y++) {
//         //         TileBase tile = allTiles[x + y * bounds.size.x];
//         //         if (tile != null) {
//         //             Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
//         //         } else {
//         //             Debug.Log("x:" + x + " y:" + y + " tile: (null)");
//         //         }
//         //     }
//         // } 


//     }

//     public void SaveTilemap()
//     {
//         var output = wfc.GetOutputTileMap();
//         if (output != null)
//         {
//             outputImage = output;
//             GameObject objectToSave = outputImage.gameObject;

//             PrefabUtility.SaveAsPrefabAsset(objectToSave, "Assets/Resources/output.prefab");
//         }
//     }

// }
