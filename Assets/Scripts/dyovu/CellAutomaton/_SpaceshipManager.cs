// using UnityEngine;
// using System.Collections.Generic;
// using System.Linq;

// using static SpaceShipConstants;


// // SpaceShipsManager - Unity依存を完全に除去
// public class SpaceShipsManager
// {
//     private int nextSpaceShipID = 0;
//     private Dictionary<int, Spaceship> activeSpaceShips;

//     public SpaceShipsManager()
//     {
//         activeSpaceShips = new Dictionary<int, Spaceship>();
//     }

//     public Vector3Int[] CreateSpaceShip(Vector3Int centerCell, GliderDirection direction = GliderDirection.RightBackward, SpaceShipsType type = SpaceShipsType.Glider)
//     {
//         Spaceship newSpaceShip = new Spaceship(nextSpaceShipID, centerCell, type, direction);
//         activeSpaceShips[nextSpaceShipID] = newSpaceShip;
//         Vector3Int[] initialCells = newSpaceShip.GetCurrentPhaseCells();
//         nextSpaceShipID++;
//         return initialCells;
//     }

//     public Dictionary<int, Spaceship> GetActiveSpaceShips() => activeSpaceShips;

//     public NextCellsInfo GetNextGenerationWithCollisions()
//     {
//         HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();
//         Dictionary<Vector3Int, List<int>> cellToSpaceShips = new Dictionary<Vector3Int, List<int>>();

//         foreach (var spaceShip in activeSpaceShips.Values)
//         {
//             spaceShip.UpdatePhase();
//             Vector3Int[] cells = spaceShip.GetCurrentPhaseCells();
            
//             if (cells != null && cells.Length > 0)
//             {
//                 // これもforeachの中でやったらもうちょい計算量削減できる
//                 allCells.UnionWith(cells);
                
//                 // 同時に衝突検知用のデータ構築
//                 foreach (var cell in cells)
//                 {
//                     if (!cellToSpaceShips.ContainsKey(cell))
//                     {
//                         cellToSpaceShips[cell] = new List<int>();
//                     }
//                     cellToSpaceShips[cell].Add(spaceShip.GetID());
//                 }
//             }
//         }

//         // 衝突のみ抽出
//         var collisions = cellToSpaceShips.Where(kvp => kvp.Value.Count > 1)
//                                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

//         return new NextCellsInfo 
//         { 
//             AllCells = allCells, 
//             Collisions = collisions 
//         };
//     }

//     // 
//     public void RemoveSpaceShip(int id)
//     {
//         if (activeSpaceShips.ContainsKey(id))
//         {
//             activeSpaceShips.Remove(id);
//         }
//         else
//         {
//             Debug.LogWarning($"SpaceShip with ID {id} does not exist.");
//         }
//     }


//     // 全SpaceShipの次世代セル位置を返す
//     public HashSet<Vector3Int> GetNextGenerationCells()
//     {
//         HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();

//         foreach (var spaceShip in activeSpaceShips.Values)
//         {
//             spaceShip.UpdatePhase();
//             Vector3Int[] cells = spaceShip.GetCurrentPhaseCells();
//             if (cells != null && cells.Length > 0)
//             {
//                 allCells.UnionWith(cells);
//             }
//         }

//         return allCells;
//     }

//     // つかうかも
//     // 全てのSpaceShipをクリア
//     //
//     private void ClearAllSpaceShips()
//     {
//         activeSpaceShips.Clear();
//         nextSpaceShipID = 0;
//     }
// }