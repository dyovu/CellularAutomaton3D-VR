using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using static SpaceshipConstants;
using dyovu.Glider;

// GlidersManager - Unity依存を完全に除去
public class SpaceshipsManager
{
    private int nextGliderID = 0;
    private Dictionary<int, Glider> activeGliders;

    public SpaceshipsManager()
    {
        activeGliders = new Dictionary<int, Glider>();
    }

    public Dictionary<int, Glider> GetActiveGliders() => activeGliders;

    public Vector3Int[] CreateGlider(Vector3Int centerCell, GliderDirection direction, GliderPhase phase, SpaceshipType type = SpaceshipType.Glider)
    {
        Glider newGlider = new Glider(nextGliderID, centerCell, direction, phase);
        activeGliders[nextGliderID] = newGlider;
        Vector3Int[] initialCells = newGlider.GetCurrentPhaseCells();
        nextGliderID++;
        return initialCells;
    }
    
    public GliderInfo GetNextGenerationWithCollisions()
    {
        HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, List<int>> cellToGliders = new Dictionary<Vector3Int, List<int>>();
        Dictionary<int , Vector3Int[]> idToCells = new Dictionary<int, Vector3Int[]>();

        foreach (var Glider in activeGliders.Values)
        {
            Glider.UpdatePhase();
            Vector3Int[] cells = Glider.GetCurrentPhaseCells();

            if (cells != null && cells.Length > 0)
            {
                // これもforeachの中でやったらもうちょい計算量削減できる
                allCells.UnionWith(cells);
                idToCells[Glider.GetID()] = cells;

                // 同時に衝突検知用のデータ構築
                foreach (var cell in cells)
                {
                    if (!cellToGliders.ContainsKey(cell))
                    {
                        cellToGliders[cell] = new List<int>();
                    }
                    cellToGliders[cell].Add(Glider.GetID());
                }
            }
        }

        // 衝突のみ抽出
        var collisions = cellToGliders.Where(kvp => kvp.Value.Count > 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Debug.Log($"Collisions detected: {collisions.Count}");

        foreach (var collision in collisions)
        {
            Vector3Int cellPosition = collision.Key;
            List<int> GliderIDs = collision.Value;
            
            Debug.Log($"座標 ({cellPosition.x}, {cellPosition.y}, {cellPosition.z}) で {GliderIDs.Count}機のGliderが衝突:");

            foreach (int id in GliderIDs)
            {
                Debug.Log($"  - Glider ID: {id}");
                ChangeGliderColor(id, Color.red); // 衝突したGliderの色を赤に変更
            }
        }

        return new GliderInfo
        {
            AllCells = allCells,
            Collisions = collisions,
            IDToCells = idToCells
        };
    }


    void ChangeGliderColor(int id, Color newColor)
    {
        if (activeGliders.ContainsKey(id))
        {
            activeGliders[id].Color = newColor;
        }
        else
        {
            Debug.LogWarning($"Color cannot change, Glider with ID {id} does not exist.");
        }
    }

    // 
    public void RemoveGlider(int id)
    {
        if (activeGliders.ContainsKey(id))
        {
            activeGliders.Remove(id);
        }
        else
        {
            Debug.LogWarning($"Glider with ID {id} does not exist.");
        }
    }


    // 全Gliderの次世代セル位置を返す
    public HashSet<Vector3Int> GetNextGenerationCells()
    {
        HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();

        foreach (var Glider in activeGliders.Values)
        {
            Glider.UpdatePhase();
            Vector3Int[] cells = Glider.GetCurrentPhaseCells();
            if (cells != null && cells.Length > 0)
            {
                allCells.UnionWith(cells);
            }
        }

        return allCells;
    }

    // つかうかも
    // 全てのGliderをクリア
    //
    private void ClearAllGliders()
    {
        activeGliders.Clear();
        nextGliderID = 0;
    }
}