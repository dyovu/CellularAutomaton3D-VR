using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using static SpaceshipConstants;
using static GridUtils;

namespace dyovu.Glider
{
    public class Glider
    {
        private int ID;
        private Vector3Int centerCell; // 3x3の中心のセルを受け取る
        private int generation;
        private GliderPhase phase;
        private GliderDirection direction;
        private Color GliderColor;

        public Glider(int id, Vector3Int centerCell, GliderDirection direction, GliderPhase phase)
        {
            ID = id;
            generation = 0;
            this.phase = phase;
            this.centerCell = centerCell;
            this.direction = direction;
            this.GliderColor = Color.white; // デフォルトの色を設定
        }

        public int GetID() { return ID; }
        public Vector3Int CenterPosition { get { return centerCell; } }
        public Color Color { get { return GliderColor; } set { GliderColor = value; } }

        public void UpdatePhase()
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
            centerCell = AdjustPosition(centerCell); // トーラス境界を考慮して位置調整
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

        // ディレクションを考慮したセルの位置調整
        private Vector3Int RotateCells(Vector3Int offset_cell)
        {
            switch (direction)
            {
                case GliderDirection.RightBackward:
                    return new Vector3Int(centerCell.x + offset_cell.x, centerCell.y + offset_cell.y, centerCell.z + offset_cell.z);
                case GliderDirection.RightForward:
                    return new Vector3Int(centerCell.x - offset_cell.z, centerCell.y + offset_cell.y, centerCell.z + offset_cell.x);
                case GliderDirection.LeftForward:
                    return new Vector3Int(centerCell.x - offset_cell.x, centerCell.y + offset_cell.y, centerCell.z - offset_cell.z);
                case GliderDirection.LeftBackward:
                    return new Vector3Int(centerCell.x + offset_cell.z, centerCell.y + offset_cell.y, centerCell.z - offset_cell.x);
                default:
                    return new Vector3Int(centerCell.x + offset_cell.x, centerCell.y + offset_cell.y, centerCell.z + offset_cell.z);
            }
        }

        // ディレクションを考慮した中心セルの位置調整
        private Vector3Int RotateCenterOffset(Vector3Int centerOffset)
        {
            switch (direction)
            {
                case GliderDirection.RightBackward:
                    return new Vector3Int(centerOffset.x, centerOffset.y, centerOffset.z);
                case GliderDirection.RightForward:
                    return new Vector3Int(-centerOffset.z, centerOffset.y, centerOffset.x);
                case GliderDirection.LeftForward:
                    return new Vector3Int(-centerOffset.x, centerOffset.y, -centerOffset.z);
                case GliderDirection.LeftBackward:
                    return new Vector3Int(centerOffset.z, centerOffset.y, -centerOffset.x);
                default:
                    return new Vector3Int(centerOffset.x, centerOffset.y, centerOffset.z);
            }
        }
    }
    
}
