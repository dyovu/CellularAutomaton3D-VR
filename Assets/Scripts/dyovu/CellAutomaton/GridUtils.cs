using UnityEngine;

public static class GridUtils
{
    private static Vector3Int gridSize;
    private static readonly int GAP_SIZE = 3; // 追加

    // Gridのサイズを取得
    public static void SetGridSize(Vector3Int size)
    {
        gridSize = size;
    }

    public static Vector3Int AdjustPosition(Vector3Int position)
    {
        // 修正前のコードを以下に置き換え
        int x = AdjustCoordinate(position.x, gridSize.x);
        int y = AdjustCoordinate(position.y, gridSize.y);
        int z = AdjustCoordinate(position.z, gridSize.z);
        return new Vector3Int(x, y, z);
    }

    // 面と面の空白を考慮した座標計算
    private static int AdjustCoordinate(int coord, int size)
    {
        if (coord < 0)
            return coord + size + GAP_SIZE;
        else if (coord >= size + GAP_SIZE)
            return coord - (size + GAP_SIZE);
        else
            return coord;
    }

    // 面と面の境界にいるセル華道家の判定
    public static bool IsInVisibleArea(Vector3Int position)
    {
        return position.x >= 0 && position.x < gridSize.x &&
               position.y >= 0 && position.y < gridSize.y &&
               position.z >= 0 && position.z < gridSize.z;
    }
}