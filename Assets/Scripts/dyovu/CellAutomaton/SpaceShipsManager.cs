using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using static SpaceshipConstants;
using dyovu.Glider;

// GlidersManager - Unity依存を完全に除去
public class SpaceshipsManager
{
    private int nextGliderID = 0;
    private int nextBaysID = 0;
    private Dictionary<int, Glider> activeGliders;
    private Dictionary<int, Bays> activeBays;

    public SpaceshipsManager()
    {
        activeGliders = new Dictionary<int, Glider>();
        activeBays = new Dictionary<int, Bays>();
    }

    public Dictionary<int, Glider> GetActiveGliders() => activeGliders;
    public Dictionary<int, Bays> GetActiveBays() => activeBays;



    public Vector3Int[] CreateGlider(Vector3Int centerCell, GliderDirection direction, GliderPhase phase, SpaceshipType type = SpaceshipType.Glider)
    {
        Glider newGlider = new Glider(nextGliderID, centerCell, direction, phase);
        activeGliders[nextGliderID] = newGlider;
        Vector3Int[] initialCells = newGlider.GetCurrentPhaseCells();
        nextGliderID++;
        // Debug.Log($"Glider created with ID: {newGlider.GetID()} at position {centerCell} in direction {direction} and phase {phase}");
        return initialCells;
    }

    public void CreateBays(Vector3Int forwardCell, BaysDirection direction)
    {
        Bays newBays = new Bays(nextBaysID, forwardCell, direction);
        activeBays[nextBaysID] = newBays;
        nextBaysID++;
        Debug.Log($"Bays created with ID: {newBays.GetID()} at position {forwardCell} in direction {direction}");
    }
    


    public GliderInfo GetGliderInfo()
    {
        HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, List<int>> cellToGliders = new Dictionary<Vector3Int, List<int>>();
        Dictionary<int, Vector3Int[]> idToCells = new Dictionary<int, Vector3Int[]>();

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

        return new GliderInfo
        {
            AllCells = allCells,
            Collisions = collisions,
            IDToCells = idToCells
        };
    }

    public BaysInfo GetBaysInfo()
    {
        HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, List<int>> cellToBays = new Dictionary<Vector3Int, List<int>>();
        Dictionary<int, Vector3Int[]> idToCells = new Dictionary<int, Vector3Int[]>();

        foreach (var bays in activeBays.Values)
        {
            bays.UpdatePhase();
            Vector3Int[] cells = bays.GetCurrentPhaseCells();

            if (cells != null && cells.Length > 0)
            {
                allCells.UnionWith(cells);
                idToCells[bays.GetID()] = cells;

                // 同時に衝突検知用のデータ構築
                foreach (var cell in cells)
                {
                    if (!cellToBays.ContainsKey(cell))
                    {
                        cellToBays[cell] = new List<int>();
                    }
                    cellToBays[cell].Add(bays.GetID());
                }
            }
        }
        // 衝突のみ抽出
        var collisions = cellToBays.Where(kvp => kvp.Value.Count > 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return new BaysInfo
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
            Debug.Log($"Glider has been removed with ID {id}.");
        }
        else
        {
            Debug.LogWarning($"Glider does not exist with ID {id}.");
        }
    }

    public void RemoveBays(int id)
    {
        if (activeBays.ContainsKey(id))
        {
            activeBays.Remove(id);
            Debug.Log($"Bays has been removed with ID {id} .");
        }
        else
        {
            Debug.LogWarning($"Bays does not exist with ID {id}.");
        }
    }

    // // 全Gliderの次世代セル位置を返す
    // public HashSet<Vector3Int> GetNextGenerationCells()
    // {
    //     HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();

    //     foreach (var Glider in activeGliders.Values)
    //     {
    //         Glider.UpdatePhase();
    //         Vector3Int[] cells = Glider.GetCurrentPhaseCells();
    //         if (cells != null && cells.Length > 0)
    //         {
    //             allCells.UnionWith(cells);
    //         }
    //     }

    //     return allCells;
    // }

    // 
    // つかうかも
    // 全てのGliderをクリア
    //
    private void ClearAllGliders()
    {
        activeGliders.Clear();
        nextGliderID = 0;
    }
}