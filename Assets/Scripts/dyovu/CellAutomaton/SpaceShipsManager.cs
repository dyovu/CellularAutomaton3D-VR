using UnityEngine;
using System.Collections.Generic;

using static SpaceShipConstants;


// SpaceShipsManager - Unity依存を完全に除去
public class SpaceShipsManager
{
    private int nextSpaceShipID = 0;
    private Dictionary<int, Spaceship> activeSpaceShips;
    private Vector3Int gridSize;

    public SpaceShipsManager(Vector3Int gridSize)
    {
        this.gridSize = gridSize;
        activeSpaceShips = new Dictionary<int, Spaceship>();
    }

    public void CreateSpaceShip(Vector3Int centerCell, GliderDirection direction = GliderDirection.RightBackward, SpaceShipsType type=SpaceShipsType.Glider)
    {
        Spaceship newSpaceShip = new Spaceship(nextSpaceShipID, centerCell, type, direction);
        activeSpaceShips[nextSpaceShipID] = newSpaceShip;
        nextSpaceShipID++;
    }

    // 全SpaceShipの次世代セル位置を返す
    public HashSet<Vector3Int> GetNextGenerationCells()
    {
        HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();
        
        foreach (var spaceShip in activeSpaceShips.Values)
        {
            Vector3Int[] cells = spaceShip.UpdatePhase();
            if (cells != null && cells.Length > 0)
            {
                allCells.UnionWith(cells);
            }
        }
        
        return allCells;
    }

    public Dictionary<int, Spaceship> GetActiveSpaceShips() => activeSpaceShips;
}