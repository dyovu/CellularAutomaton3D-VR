using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using static SpaceshipConstants;
using static GridUtils;

public class Bays
{
    private int ID;
    private BaysPhase phase;
    private int generation;
    private Vector3Int forwardCell; // 進行方向の一番手前で、真ん中ぐらいの面のセル
    private BaysDirection direction;

    public Bays(int id, Vector3Int forwardCell, BaysDirection direction)
    {
        ID = id;
        phase = BaysPhase.Phase1;
        generation = 0;
        this.forwardCell = forwardCell;
        this.direction = direction;
    }

    public int GetID() { return ID; }
    public Vector3Int ForwardCell { get { return forwardCell; } }


    public void UpdatePhase()
    {
        // Baysのフェーズ更新ロジックはまだ実装されていないので、仮の実装
        switch (phase)
        {
            case BaysPhase.Phase1:
                phase = BaysPhase.Phase2;
                break;
            case BaysPhase.Phase2:
                phase = BaysPhase.Phase3;
                break;
            case BaysPhase.Phase3:
                phase = BaysPhase.Phase4;
                break;
            case BaysPhase.Phase4:
                phase = BaysPhase.Phase5;
                break;
            case BaysPhase.Phase5:
                phase = BaysPhase.Phase6;
                break;
            case BaysPhase.Phase6:
                phase = BaysPhase.Phase7;
                forwardCell += RotateForwardOffset(new Vector3Int(0, 1, 0));
                break;
            case BaysPhase.Phase7:
                phase = BaysPhase.Phase8;
                break;
            case BaysPhase.Phase8:
                phase = BaysPhase.Phase1;
                forwardCell += RotateForwardOffset(new Vector3Int(0, 1, 0));
                break;
        }
        forwardCell = AdjustPosition(forwardCell); // トーラス境界を考慮して位置調整
        generation++;
    }


    public Vector3Int[] GetCurrentPhaseCells()
    {
        HashSet<Vector3Int> offsets = BaysOffsets[phase];
        Vector3Int[] currentGenerationCells = new Vector3Int[offsets.Count * 2];

        foreach (var (offset, index) in offsets.Select((value, index) => (value, index)))
        {
            for (int i = 0; i < 2; i++) // x=1を軸に面対称
            {
                int offset_x;
                if (i == 0)
                {
                    offset_x = offset.x; 
                }
                else
                {
                    offset_x = -(offset.x + 1); // +1ではなく+iの間違いかもしれません
                }
                
                Vector3Int offset_cell = new Vector3Int(offset_x, offset.y, offset.z);
                Vector3Int rotatedCell = RotateCell(offset_cell); // 結果を使用
                currentGenerationCells[index * 2 + i] = AdjustPosition(rotatedCell); // index*2+1ではなくindex*2+i
            }
        }

        return currentGenerationCells;
    }

    private Vector3Int RotateCell(Vector3Int offset_cell)
    {
        Vector3Int result = direction switch
        {
            BaysDirection.Down => new Vector3Int(-offset_cell.x, -offset_cell.y, offset_cell.z),
            _ => new Vector3Int(offset_cell.x, offset_cell.y, offset_cell.z)
        };
        
        return forwardCell + result;
    }
    

    private Vector3Int RotateForwardOffset(Vector3Int offset)
    {
        return direction switch
        {
            BaysDirection.Down => new Vector3Int(offset.x, -offset.y, offset.z),
            _ => offset
        };
    }

}
