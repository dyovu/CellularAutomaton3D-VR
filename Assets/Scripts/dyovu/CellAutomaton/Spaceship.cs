using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using static SpaceShipConstants;
using static GridUtils;
using UnityEngine.PlayerLoop;

public class Spaceship
{
    private int ID;
    private Vector3Int centerCell;
    private int generation;
    private SpaceShipsType spaceShipsType;
    private GliderPhase phase;
    private GliderDirection direction;

    public Spaceship(int id, Vector3Int centerCell, SpaceShipsType type, GliderDirection direction)
    {
        ID = id;
        spaceShipsType = type;
        generation = 0;
        phase = GliderPhase.Phase1;
        this.centerCell = centerCell;
        this.direction = direction;
    }

    public int GetID() { return ID; }
    public Vector3Int CenterPosition { get { return centerCell; } }
    public SpaceShipsType Type { get { return spaceShipsType; } }

    public void UpdatePhase() // 次の世代のセルを更新してその配列を返す
    {
        switch (phase)
        {
            case GliderPhase.Phase1:
                phase = GliderPhase.Phase2;
                /*
                * // 中心セルの移動もdirectionを考慮して
                */
                centerCell += RotateCenterOffset(new Vector3Int(0, 0, -1));
                break;
            case GliderPhase.Phase2:
                phase = GliderPhase.Phase3;
                break;
            case GliderPhase.Phase3:
                phase = GliderPhase.Phase4;
                centerCell += RotateCenterOffset(new Vector3Int(1, 0, 0));
                break;
            case GliderPhase.Phase4:
                phase = GliderPhase.Phase1;
                break;
        }
        AdjustPosition(centerCell); // トーラス境界を考慮して位置調整
        generation++;
    }

    public Vector3Int[] GetCurrentPhaseCells()
    {
        HashSet<Vector3Int> offsets = GliderOffsets[phase];
        Vector3Int[] currentGenerationCells = new Vector3Int[offsets.Count * 2];

        foreach (var (offset, index) in offsets.Select((value, index) => (value, index)))
        {
            for (int i = 0; i < 2; i++) // 上下二段
            {
                Vector3Int ofset_cell = new Vector3Int(offset.x, i, offset.z);
                Vector3Int rotatedCell = RotateCells(ofset_cell);
                currentGenerationCells[index * 2 + i] = AdjustPosition(rotatedCell);
            }
        }
        return currentGenerationCells;
    }

    // トーラス境界を考慮して調整したセルの位置を返す
    private Vector3Int RotateCells(Vector3Int offset_cell)
    {
        switch (direction)
        {
            case GliderDirection.RightBackward:
                return new Vector3Int(centerCell.x + offset_cell.x, centerCell.y + offset_cell.y, centerCell.z + offset_cell.z);
            case GliderDirection.RightForward:
                return new Vector3Int(centerCell.x - offset_cell.z, centerCell.y + offset_cell.y, centerCell.z + offset_cell.x);
            case GliderDirection.LeftBackward:
                return new Vector3Int(centerCell.x - offset_cell.x, centerCell.y + offset_cell.y, centerCell.z - offset_cell.z);
            case GliderDirection.LeftForward:
                return new Vector3Int(centerCell.x + offset_cell.z, centerCell.y + offset_cell.y, centerCell.z - offset_cell.x);
            default:
                return new Vector3Int(centerCell.x + offset_cell.x, centerCell.y + offset_cell.y, centerCell.z + offset_cell.z);
        }
    }

    // トーラス境界を考慮して中心セルのオフセットを返す
    private Vector3Int RotateCenterOffset(Vector3Int centerOffset)
    {
        switch (direction)
        {
            case GliderDirection.RightBackward:
                return new Vector3Int(centerOffset.x, centerOffset.y, centerOffset.z);
            case GliderDirection.RightForward:
                return new Vector3Int(-centerOffset.z, centerOffset.y, centerOffset.x);
            case GliderDirection.LeftBackward:
                return new Vector3Int(-centerOffset.x, centerOffset.y, -centerOffset.z);
            case GliderDirection.LeftForward:
                return new Vector3Int(centerOffset.z, centerOffset.y, -centerOffset.x);
            default:
                return new Vector3Int(centerOffset.x, centerOffset.y, centerOffset.z);
        }
    }
}