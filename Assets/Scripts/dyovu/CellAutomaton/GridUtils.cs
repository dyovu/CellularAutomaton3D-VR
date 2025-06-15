using UnityEngine;

public static class GridUtils
{
    private static Vector3Int gridSize;

    public static void SetGridSize(Vector3Int size)
    {
        gridSize = size;
    }

    //
    // トーラス境界を考慮してセル位置を調整
    // public関数としてSpaceShipの中で呼び出すよ
    // 
    public static Vector3Int AdjustPosition(Vector3Int position)
    {
        int x = ((position.x % gridSize.x) + gridSize.x) % gridSize.x;
        int y = ((position.y % gridSize.y) + gridSize.y) % gridSize.y;
        int z = ((position.z % gridSize.z) + gridSize.z) % gridSize.z;
        return new Vector3Int(x, y, z);
    }
}