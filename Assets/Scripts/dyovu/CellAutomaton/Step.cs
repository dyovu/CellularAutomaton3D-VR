// using UnityEngine;

// public class AAAA : MonoBehaviour
// {
//     void Step()
//     {
//         // 次の世代の状態を計算
//         for (int x = 0; x < width; x++)
//         {
//             for (int y = 0; y < height; y++)
//             {
//                 for (int z = 0; z < depth; z++)
//                 {
//                     int aliveNeighbors = CountAliveNeighbors(x, y, z);
//                     bool isAlive = currentState[x, y, z];

//                     if (isAlive)
//                     {
//                         // 生存ルール：5〜7個の生きた近傍があれば生き残る
//                         nextState[x, y, z] = (aliveNeighbors >= 5 && aliveNeighbors <= 7);
//                     }
//                     else
//                     {
//                         // 誕生ルール：6個の生きた近傍があれば誕生
//                         nextState[x, y, z] = (aliveNeighbors >= 6 && aliveNeighbors <= 6);
//                     }
//                 }
//             }
//         }

//         // グリッド更新（アニメーション付き）
//         for (int x = 0; x < width; x++)
//         {
//             for (int y = 0; y < height; y++)
//             {
//                 for (int z = 0; z < depth; z++)
//                 {
//                     bool wasAlive = currentState[x, y, z];
//                     bool willBeAlive = nextState[x, y, z];

//                     if (wasAlive != willBeAlive)
//                     {
//                         Vector3Int pos = new Vector3Int(x, y, z);
//                         GameObject cube = grid[pos];

//                         if (willBeAlive)
//                         {
//                             // 誕生アニメーション
//                             cube.SetActive(true);
//                             StartCoroutine(AnimateScale(cube, Vector3.zero, Vector3.one));
//                         }
//                         else
//                         {
//                             // 消滅アニメーション
//                             StartCoroutine(AnimateScale(cube, Vector3.one, Vector3.zero, () => cube.SetActive(false)));
//                         }
//                     }

//                     currentState[x, y, z] = willBeAlive;
//                 }
//             }
//         }
//     }

//     int CountAliveNeighbors(int x, int y, int z)
//     {
//         int count = 0;

//         // 境界セルかどうかを判定
//         bool isBoundary = (x == 0 || x == width - 1 ||
//                          y == 0 || y == height - 1 ||
//                          z == 0 || z == depth - 1);

//         if (isBoundary)
//         {
//             // 境界セル：事前計算されたインデックスを使用
//             Vector3Int[] neighbors = boundaryNeighbors[new Vector3Int(x, y, z)];
//             for (int i = 0; i < neighbors.Length; i++)
//             {
//                 Vector3Int neighbor = neighbors[i];
//                 if (currentState[neighbor.x, neighbor.y, neighbor.z])
//                 {
//                     count++;
//                 }
//             }
//         }
//         else
//         {
//             // 内部セル：単純なオフセット計算（キャッシュ効率良好）
//             for (int i = 0; i < neighborOffsets.Length; i++)
//             {
//                 Vector3Int offset = neighborOffsets[i];
//                 if (currentState[x + offset.x, y + offset.y, z + offset.z])
//                 {
//                     count++;
//                 }
//             }
//         }
//         return count;
//     }
// }
