using UnityEngine;
using System.Collections.Generic;

using static SpaceShipConstants;


// SpaceShipsManager - Unity依存を完全に除去
public class SpaceShipsManager
{
    private int nextSpaceShipID = 0;
    private Dictionary<int, Spaceship> activeSpaceShips;

    public SpaceShipsManager()
    {
        activeSpaceShips = new Dictionary<int, Spaceship>();
    }

    public Vector3Int[] CreateSpaceShip(Vector3Int centerCell, GliderDirection direction = GliderDirection.RightBackward, SpaceShipsType type = SpaceShipsType.Glider)
    {
        Spaceship newSpaceShip = new Spaceship(nextSpaceShipID, centerCell, type, direction);
        activeSpaceShips[nextSpaceShipID] = newSpaceShip;
        Vector3Int[] initialCells = newSpaceShip.GetCurrentPhaseCells();
        nextSpaceShipID++;
        return initialCells;
    }
    
    public Dictionary<int, Spaceship> GetActiveSpaceShips() => activeSpaceShips;

    // 全SpaceShipの次世代セル位置を返す
    public HashSet<Vector3Int> GetNextGenerationCells()
    {
        HashSet<Vector3Int> allCells = new HashSet<Vector3Int>();

        foreach (var spaceShip in activeSpaceShips.Values)
        {
            spaceShip.UpdatePhase();
            Vector3Int[] cells = spaceShip.GetCurrentPhaseCells();
            if (cells != null && cells.Length > 0)
            {
                allCells.UnionWith(cells);
            }
        }

        return allCells;
    }
}