// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// using static SpaceshipConstants;


// // ToroidalBoundsCellulerAutomaton - Grid操作を集約
// public partial class ToroidalBoundsCellularAutomaton : MonoBehaviour
// {
//     [SerializeField] GameObject cubePrefab;
//     [SerializeField] int width = 30;
//     [SerializeField] int height = 30;
//     [SerializeField] int depth = 30;
//     [SerializeField] float stepInterval = 1.3f;
//     [SerializeField] float animationDuration = 0.1f;
//     [System.Serializable]
//     public struct InitialSpaceshipConfig
//     {
//         public Vector3Int centerPosition;
//         public GliderDirection direction;
//         public Color color; 
//     }

//     // 初期配置設定
//     [SerializeField] InitialSpaceshipConfig[] initialSpaceships = new InitialSpaceshipConfig[]
//     {
//         new InitialSpaceshipConfig { centerPosition = new Vector3Int(15, 15, 0), direction = GliderDirection.RightBackward, color = Color.blue },
//     };

//     Dictionary<Vector3Int, GameObject> GRID = new Dictionary<Vector3Int, GameObject>();
//     Dictionary<int, Vector3Int[]> currentActiveCells = new Dictionary<int, Vector3Int[]>();
//     HashSet<Vector3Int> allCells_set = new HashSet<Vector3Int>();

//     private SpaceshipsManager SpaceshipsManager;

//     void Start()
//     {
//         GridUtils.SetGridSize(new Vector3Int(width, height, depth));
//         SpaceshipsManager = new SpaceshipsManager();
        
//         // ここで全てのセルを非アクティブでinstantiateして初期化している
//         InitializeGrid();
//         // 初期セルをアクティブに変更
//         Dictionary<int, Vector3Int[]> InitialCells = SetupInitialSpaceships();
//         // ActivateCells(InitialCells);

//         ActivateCells_set();

//         StartCoroutine(StepRoutine());
//     }

//     void InitializeGrid()
//     {
//         for (int x = 0; x < width; x++)
//         {
//             for (int y = 0; y < height; y++)
//             {
//                 for (int z = 0; z < depth; z++)
//                 {
//                     Vector3 position = new Vector3(x, y, z);
//                     GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, this.transform);
//                     Vector3Int gridPos = new Vector3Int(x, y, z);
//                     GRID[gridPos] = cube;

//                     cube.SetActive(false);
//                     cube.transform.localScale = Vector3.zero;
//                 }
//             }
//         }
//     }

//     Dictionary<int, Vector3Int[]> SetupInitialSpaceships()
//     {
//         allCells_set.Clear(); 

//         Dictionary<int, Vector3Int[]> initialCells = new Dictionary<int, Vector3Int[]>();
//         foreach (var config in initialSpaceships)
//         {
//             (int id, Vector3Int[] cells) = SpaceshipsManager.CreateSpaceship(config.centerPosition, config.color, config.direction);
//             initialCells[id] = cells;
//             allCells_set.UnionWith(cells); // 全てのセル位置をセットに追加
//         }
//         return initialCells;
//     }

//     IEnumerator StepRoutine()
//     {
//         while (true)
//         {
//             yield return new WaitForSeconds(stepInterval);
//             UpdateGeneration();
//         }
//     }

//     void UpdateGeneration()
//     {
//         // 現在のセルを非アクティブに
//         // DeactivateCurrentCells();

//         // 次世代のセル位置と衝突しているグライダーの座標とIDを取得
//         CellsInfo nextCellsInfo = SpaceshipsManager.GetNextGenerationWithCollisions();

//         Dictionary<int, Vector3Int[]> nextCells = nextCellsInfo.AllCells;
//         Dictionary<Vector3Int, List<int>> collisions = nextCellsInfo.Collisions;

//         // // 新しいセルをアクティブに
//         // ActivateCells(nextCells);
//         // // RemoveCollidedSpaceships(collisions);



//         /*
//         * setでためす
//         */

//         DeactivateCurrentCells_set(); // 全てのセルを非アクティブにする

//         allCells_set = nextCellsInfo.allCells_set; // 全てのセル位置を更新
//         Debug.Log($"All Cells Count: {allCells_set.Count}");
//         ActivateCells_set(); // 全てのセルをアクティブにする

//     }

//     void DeactivateCurrentCells_set()
//     {
//         // 全てのセルを非アクティブにする
//         foreach (Vector3Int position in allCells_set)
//         {
//             DeactivateCell(position);
//         }
//         currentActiveCells.Clear(); // 既存のアクティブセルをクリア
//     }



//     void DeactivateCurrentCells()
//     {
//         foreach (var cell in currentActiveCells.Values)
//         {
//             foreach (Vector3Int position in cell)
//             {
//                 DeactivateCell(position);
//             }
//         }
//     }

//     void DeactivateCell(Vector3Int position)
//     {
//         if (GRID.TryGetValue(position, out GameObject cube))
//         {
//             StartCoroutine(AnimateScale(cube, Vector3.one, Vector3.zero, () => cube.SetActive(false)));
//         }
//         else
//         {
//             Debug.LogWarning($"Cell at {position} does not exist in the grid.");
//         }
//     }

//     void ActivateCells_set()
//     {
//         // 全てのセルをアクティブにする
//         foreach (Vector3Int position in allCells_set)
//         {
//             ActivateCell(position, Color.white); // 色は適宜変更
//         }
//         currentActiveCells.Clear(); // 既存のアクティブセルをクリア
        
//     }



//     void ActivateCells(Dictionary<int, Vector3Int[]> cells)
//     {
//         foreach (var (id, cell) in cells)
//         {
//             Color col = SpaceshipsManager.GetActiveSpaceships()[id].color;
//             print($"Activating cells for Spaceship ID {id} with color {col}, Cells Count: {cell.Length}");
//             foreach (Vector3Int position in cell)
//             {
//                 ActivateCell(position, col);
//             }
//         }
//         currentActiveCells = cells; 
//     }

//     // ここの処理後で理解する
//     void ActivateCell(Vector3Int position, Color color)
//     {
//         if (GRID.TryGetValue(position, out GameObject cube))
//         {
//             cube.SetActive(true);
//             Renderer renderer = cube.GetComponent<Renderer>();
//             if (renderer != null)
//             {
//                 renderer.material.color = color;
//             }
//             else
//             {
//                 Debug.LogWarning($"Renderer not found on the cube at position {position}. Cannot set color.");
//             }
//             StartCoroutine(AnimateScale(cube, Vector3.zero, Vector3.one));
//         }
//         else
//         {
//             Debug.LogWarning($"Cell at {position} does not exist in the grid.");
//         }
//     }



//     void RemoveCollidedSpaceships(Dictionary<Vector3Int, List<int>> collisions)
//     {
//         foreach (var collision in collisions)
//         {
//             Vector3Int cell = collision.Key;
//             List<int> SpaceshipIDs = collision.Value;

//             // ここで衝突したスペースシップを削除
//             foreach (int id in SpaceshipIDs)
//             {
//                 SpaceshipsManager.RemoveSpaceship(id);
//                 Debug.Log($"Spaceship with ID {id} has been removed due to collision at cell {cell}.");
//             }

//             // セルを非アクティブにする
//             if (GRID.TryGetValue(cell, out GameObject cube))
//             {
//                 StartCoroutine(AnimateScale(cube, Vector3.one, Vector3.zero, () => cube.SetActive(false)));
//             }
//         }
//     }


//     IEnumerator AnimateScale(GameObject obj, Vector3 from, Vector3 to, System.Action onComplete = null)
//     {
//         float time = 0f;
//         obj.transform.localScale = from;

//         while (time < animationDuration)
//         {
//             time += Time.deltaTime;
//             float t = time / animationDuration;
//             obj.transform.localScale = Vector3.Lerp(from, to, t);
//             yield return null;
//         }

//         obj.transform.localScale = to;
//         onComplete?.Invoke();
//     }
// }