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
        private PlaneMode plane;

        public Glider(int id, Vector3Int centerCell, GliderDirection direction, GliderPhase phase, Color color, PlaneMode plane)
        {
            ID = id;
            generation = 0;
            this.phase = phase;
            this.centerCell = centerCell;
            this.direction = direction;
            this.GliderColor = color;
            this.plane = plane;
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
                    if (plane == PlaneMode.XY)
                    {
                        centerCell = RotateOffsetToXY(centerCell); // x-z平面からx-y平面に変換
                    }
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
            Vector3Int rotatedOffset = direction switch
            {
                GliderDirection.RightBackward => new Vector3Int(offset_cell.x, offset_cell.y, offset_cell.z),
                GliderDirection.RightForward => new Vector3Int(-offset_cell.z, offset_cell.y, offset_cell.x),
                GliderDirection.LeftForward => new Vector3Int(-offset_cell.x, offset_cell.y, -offset_cell.z),
                GliderDirection.LeftBackward => new Vector3Int(offset_cell.z, offset_cell.y, -offset_cell.x),
                _ => new Vector3Int(offset_cell.x, offset_cell.y, offset_cell.z)
            };

            Vector3Int finalPosition = centerCell + rotatedOffset;
            return plane == PlaneMode.XY ? RotateOffsetToXY(finalPosition) : finalPosition;
        }

        // ディレクションを考慮した中心セルの位置調整
        private Vector3Int RotateCenterOffset(Vector3Int centerOffset)
        {
            Vector3Int rotatedOffset = direction switch
            {
                GliderDirection.RightBackward => new Vector3Int(centerOffset.x, centerOffset.y, centerOffset.z),
                GliderDirection.RightForward => new Vector3Int(-centerOffset.z, centerOffset.y, centerOffset.x),
                GliderDirection.LeftForward => new Vector3Int(-centerOffset.x, centerOffset.y, -centerOffset.z),
                GliderDirection.LeftBackward => new Vector3Int(centerOffset.z, centerOffset.y, -centerOffset.x),
                _ => new Vector3Int(centerOffset.x, centerOffset.y, centerOffset.z)
            };

            return plane == PlaneMode.XY ? RotateOffsetToXY(rotatedOffset) : rotatedOffset;
        }

        // x-z平面のオフセットをx-y平面のオフセットに変換するためのヘルパー関数
        public static Vector3Int RotateOffsetToXY(Vector3Int offset)
        {
            return new Vector3Int(offset.x, offset.z, offset.y);
        }
    }
    
}
