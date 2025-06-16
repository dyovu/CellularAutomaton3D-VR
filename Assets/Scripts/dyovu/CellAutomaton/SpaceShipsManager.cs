using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using static SpaceshipConstants;


// SpaceshipsManager - Unity依存を完全に除去
public class SpaceshipsManager
{
    private int nextSpaceshipID = 0;
    private Dictionary<int, Spaceship> activeSpaceships;

    public SpaceshipsManager()
    {
        activeSpaceships = new Dictionary<int, Spaceship>();
    }

    public Vector3Int[] CreateSpaceship(Vector3Int centerCell, GliderDirection direction, GliderPhase phase, SpaceshipType type = SpaceshipType.Glider)
    {
        Spaceship newSpaceship = new Spaceship(nextSpaceshipID, centerCell, type, direction, phase);
        activeSpaceships[nextSpaceshipID] = newSpaceship;
        Vector3Int[] initialCells = newSpaceship.GetCurrentPhaseCells();
        nextSpaceshipID++;
        return initialCells;
    }

    public Dictionary<int, Spaceship> GetActiveSpaceships() => activeSpaceships;

    public CellsInfo GetNextGenerationWithCollisions()
    {
        HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, List<int>> cellToSpaceships = new Dictionary<Vector3Int, List<int>>();
        Dictionary<int , Vector3Int[]> idToCells = new Dictionary<int, Vector3Int[]>();

        foreach (var Spaceship in activeSpaceships.Values)
        {
            Spaceship.UpdatePhase();
            Vector3Int[] cells = Spaceship.GetCurrentPhaseCells();

            if (cells != null && cells.Length > 0)
            {
                // これもforeachの中でやったらもうちょい計算量削減できる
                allCells.UnionWith(cells);
                idToCells[Spaceship.GetID()] = cells;

                // 同時に衝突検知用のデータ構築
                foreach (var cell in cells)
                {
                    if (!cellToSpaceships.ContainsKey(cell))
                    {
                        cellToSpaceships[cell] = new List<int>();
                    }
                    cellToSpaceships[cell].Add(Spaceship.GetID());
                }
            }
        }

        // 衝突のみ抽出
        var collisions = cellToSpaceships.Where(kvp => kvp.Value.Count > 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Debug.Log($"Collisions detected: {collisions.Count}");

        foreach (var collision in collisions)
        {
            Vector3Int cellPosition = collision.Key;
            List<int> spaceshipIDs = collision.Value;
            
            Debug.Log($"座標 ({cellPosition.x}, {cellPosition.y}, {cellPosition.z}) で {spaceshipIDs.Count}機のSpaceshipが衝突:");

            foreach (int id in spaceshipIDs)
            {
                Debug.Log($"  - Spaceship ID: {id}");
                ChangeSpaceshipColor(id, Color.red); // 衝突したSpaceshipの色を赤に変更
            }
        }

        return new CellsInfo
        {
            AllCells = allCells,
            Collisions = collisions,
            IDToCells = idToCells
        };
    }


    void ChangeSpaceshipColor(int id, Color newColor)
    {
        if (activeSpaceships.ContainsKey(id))
        {
            activeSpaceships[id].Color = newColor;
        }
        else
        {
            Debug.LogWarning($"Color cannot change, Spaceship with ID {id} does not exist.");
        }
    }

    // 
    public void RemoveSpaceship(int id)
    {
        if (activeSpaceships.ContainsKey(id))
        {
            activeSpaceships.Remove(id);
        }
        else
        {
            Debug.LogWarning($"Spaceship with ID {id} does not exist.");
        }
    }


    // 全Spaceshipの次世代セル位置を返す
    public HashSet<Vector3Int> GetNextGenerationCells()
    {
        HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();

        foreach (var Spaceship in activeSpaceships.Values)
        {
            Spaceship.UpdatePhase();
            Vector3Int[] cells = Spaceship.GetCurrentPhaseCells();
            if (cells != null && cells.Length > 0)
            {
                allCells.UnionWith(cells);
            }
        }

        return allCells;
    }

    // つかうかも
    // 全てのSpaceshipをクリア
    //
    private void ClearAllSpaceships()
    {
        activeSpaceships.Clear();
        nextSpaceshipID = 0;
    }
}