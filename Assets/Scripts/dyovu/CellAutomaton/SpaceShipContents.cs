using UnityEngine;
using System.Collections.Generic;

public static class SpaceshipConstants
{
    public enum SpaceshipType
    {
        Glider,
        Bays
    }

    public enum GliderPhase
    {
        Phase1,
        Phase2,
        Phase3,
        Phase4
    }

    public enum GliderDirection
    {
        RightBackward,
        RightForward,
        LeftBackward,
        LeftForward
    }

    public struct CellsInfo
    {
        public HashSet<Vector3Int> AllCells { get; set; }
        public Dictionary<Vector3Int, List<int>> Collisions { get; set; }
        public Dictionary<int , Vector3Int[]> IDToCells { get; set; }
    }

    // readonly staticで不変性を保証
    public static readonly Dictionary<GliderPhase, HashSet<Vector3Int>> GliderOffsets =
        new Dictionary<GliderPhase, HashSet<Vector3Int>>()
        {
            {GliderPhase.Phase1, new HashSet<Vector3Int>
                {
                    new Vector3Int(-1, 0, -1),
                    new Vector3Int(0, 0, 1),
                    new Vector3Int(0, 0, -1),
                    new Vector3Int(1, 0, 0),
                    new Vector3Int(1, 0, -1),
                }
            },
            { GliderPhase.Phase2, new HashSet<Vector3Int>
                {
                    new Vector3Int(-1, 0, 1),
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, 0, -1),
                    new Vector3Int(1, 0, 1),
                    new Vector3Int(1, 0, 0)
                }
            },
            {GliderPhase.Phase3, new HashSet<Vector3Int>
                {
                    new Vector3Int(-1, 0, 0),
                    new Vector3Int(0, 0, -1),
                    new Vector3Int(1, 0, -1),
                    new Vector3Int(1, 0, 0),
                    new Vector3Int(1, 0, 1)
                }
            },
            { GliderPhase.Phase4, new HashSet<Vector3Int>
                {
                    new Vector3Int(-1, 0, -1),
                    new Vector3Int(-1, 0, 1),
                    new Vector3Int(0, 0, -1),
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(1, 0, 0)
                }
            }
        };
}