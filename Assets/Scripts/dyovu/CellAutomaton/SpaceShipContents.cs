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

    public enum PlaneMode
    {
        XZ,
        XY,
    }

    public struct GliderInfo
    {
        public HashSet<Vector3Int> AllCells { get; set; }
        public Dictionary<Vector3Int, List<int>> Collisions { get; set; }
        public Dictionary<int, Vector3Int[]> IDToCells { get; set; }
    }

    // これはx-z平面でのオフセットだから、x-y平面にしたい時には(x,z)→(x,y)に変換する
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


    public enum BaysPhase
    {
        Phase1,
        Phase2,
        Phase3,
        Phase4,
        Phase5,
        Phase6,
        Phase7,
        Phase8,
    }

    public enum BaysDirection
    {
        // Right,
        // Left,
        Up,
        Down,
        // Forward,
        // Backward
    }

    public struct BaysInfo
    {
        public HashSet<Vector3Int> AllCells { get; set; }
        public Dictionary<Vector3Int, List<int>> Collisions { get; set; }
        public Dictionary<int, Vector3Int[]> IDToCells { get; set; }
    }

    public static readonly Dictionary<BaysPhase, HashSet<Vector3Int>> BaysOffsets =
        new Dictionary<BaysPhase, HashSet<Vector3Int>>()
        {
            { BaysPhase.Phase1, new HashSet<Vector3Int>
                { // 0  
                    // 上の部分 z-
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, -1, -1),
                    new Vector3Int(0, -2, -2),
                    new Vector3Int(0, -3, -1),
                    new Vector3Int(0, -3, 0),
                    // 下の部分 z+
                    new Vector3Int(0, -1, 1),
                    new Vector3Int(0, -2, 3),
                    new Vector3Int(0, -3, 3),
                    // xの飛び出てるとこ
                    new Vector3Int(1, -2, 1),
                    new Vector3Int(1, -2, 2),
                }
            },
            { BaysPhase.Phase2, new HashSet<Vector3Int>
                {// 1
                    // 上の部分 z-
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, -1, -1),
                    new Vector3Int(0, -2, -2),
                    new Vector3Int(0, -3, -1),
                    new Vector3Int(0, -3, 0),
                    // 下のとこ
                    new Vector3Int(0, -1, 1),
                    new Vector3Int(0, -1, 2),
                    new Vector3Int(0, -2, 1),
                    new Vector3Int(0, -3, 2),
                }
            },
            { BaysPhase.Phase3, new HashSet<Vector3Int>
                {// 2
                    // 上の部分 z-
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, -1, -1),
                    new Vector3Int(0, -2, -2),
                    new Vector3Int(0, -2, -1),
                    // 下のとこ
                    new Vector3Int(0, 0, 1),
                    new Vector3Int(0, -1, 1),
                    new Vector3Int(0, -1, 2),
                    new Vector3Int(0, -2, 1),
                }
            },
            { BaysPhase.Phase4, new HashSet<Vector3Int>
                {// 3
                    // 上の部分 z-
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, -1, -1),
                    new Vector3Int(0, -1, -2),
                    new Vector3Int(0, -2, -1),
                    new Vector3Int(0, -2, -2),
                    // 下のとこ
                    new Vector3Int(0, 0, 1),
                    new Vector3Int(0, 0, 2),
                    new Vector3Int(0, -1, 2),
                    new Vector3Int(0, -2, 1),
                    new Vector3Int(0, -2, 2),
                    // xの飛び出てるとこ
                    new Vector3Int(1, -1, 0),
                }
            },
            { BaysPhase.Phase5, new HashSet<Vector3Int>
                {// 4
                    // 上の部分 z-
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, -1, -2),
                    new Vector3Int(0, -2, -2),
                    // 下の部分 z+
                    new Vector3Int(0, 1, 1),
                    new Vector3Int(0, 0, 2),
                    new Vector3Int(0, -1, 3),
                    new Vector3Int(0, -2, 1),
                    new Vector3Int(0, -2, 2),
                    // xの飛び出てるとこ
                    new Vector3Int(1, -1, 0),
                    new Vector3Int(1, -1, -1),
                }
            },

            { BaysPhase.Phase6, new HashSet<Vector3Int>
                {// 5
                    // 上の部分 z-
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, 0, -1),
                    new Vector3Int(0, -1, 0),
                    new Vector3Int(0, -2, -1),
                    // 下の部分 z+
                    new Vector3Int(0, 1, 1),
                    new Vector3Int(0, 0, 2),
                    new Vector3Int(0, -1, 3),
                    new Vector3Int(0, -2, 2),
                }
            },
            { BaysPhase.Phase7, new HashSet<Vector3Int>
                {
                    // 上の部分 z-
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, -1, 0),
                    new Vector3Int(0, -1, -1),
                    new Vector3Int(0, -2, 0),
                    // 下の部分 z+
                    new Vector3Int(0, 0, 1),
                    new Vector3Int(0, -1, 2),
                    new Vector3Int(0, -2, 2),
                    new Vector3Int(0, -2, 3),
                }
            },
            { BaysPhase.Phase8, new HashSet<Vector3Int>
                {
                    // 上の部分 z-
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, 0, -1),
                    new Vector3Int(0, -1, -1),
                    new Vector3Int(0, -2, 0),
                    new Vector3Int(0, -2, -1),
                    // 下の部分 z+
                    new Vector3Int(0, 0, 1),
                    new Vector3Int(0, -1, 2),
                    new Vector3Int(0, -1, 3),
                    new Vector3Int(0, -2, 2),
                    new Vector3Int(0, -2, 3),
                    // xの飛び出てるとこ
                    new Vector3Int(1, -1, 1),
                }
            },
        };

}