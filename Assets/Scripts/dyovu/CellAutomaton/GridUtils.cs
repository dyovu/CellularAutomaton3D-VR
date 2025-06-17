using UnityEngine;

// public static class GridUtils
// {
//     private static Vector3Int gridSize;

//     public static void SetGridSize(Vector3Int size)
//     {
//         gridSize = size;
//         Debug.Log($"Grid size set to: {gridSize}");
//     }

//     //
//     // トーラス境界を考慮してセル位置を調整
//     // public関数としてSpaceshipの中で呼び出すよ
//     // 
//     public static Vector3Int AdjustPosition(Vector3Int position)
//     {
//         int x = ((position.x % gridSize.x) + gridSize.x) % gridSize.x;
//         int y = ((position.y % gridSize.y) + gridSize.y) % gridSize.y;
//         int z = ((position.z % gridSize.z) + gridSize.z) % gridSize.z;
//         return new Vector3Int(x, y, z);
//     }
// }

public static class GridUtils
{
    private static Vector3Int gridSize;
    private static readonly int GAP_SIZE = 3; // 追加

    // 既存メソッド - そのまま
    public static void SetGridSize(Vector3Int size)
    {
        gridSize = size;
    }

    // 既存メソッド - 修正
    public static Vector3Int AdjustPosition(Vector3Int position)
    {
        // 修正前のコードを以下に置き換え
        int x = AdjustCoordinate(position.x, gridSize.x);
        int y = AdjustCoordinate(position.y, gridSize.y);
        int z = AdjustCoordinate(position.z, gridSize.z);
        return new Vector3Int(x, y, z);
    }

    // 新規追加
    private static int AdjustCoordinate(int coord, int size)
    {
        if (coord < 0)
            return coord + size + GAP_SIZE;
        else if (coord >= size + GAP_SIZE)
            return coord - (size + GAP_SIZE);
        else
            return coord;
    }

    // 新規追加
    public static bool IsInVisibleArea(Vector3Int position)
    {
        return position.x >= 0 && position.x < gridSize.x &&
               position.y >= 0 && position.y < gridSize.y &&
               position.z >= 0 && position.z < gridSize.z;
    }
}