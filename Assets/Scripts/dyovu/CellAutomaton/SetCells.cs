using UnityEngine;


public partial class ToroidalBoundsCellulerAutomaton : MonoBehaviour
{
    // 汎用的なセル配置関数
    void SetCell(int x, int y, int z, bool alive)
    {
        // トーラス境界を考慮した座標正規化
        int nx = ((x % width) + width) % width;
        int ny = ((y % height) + height) % height;
        int nz = ((z % depth) + depth) % depth;
        
        currentState[nx, ny, nz] = alive;
        
        GameObject cube = grid[new Vector3Int(nx, ny, nz)];
        cube.SetActive(alive);
        cube.transform.localScale = alive ? Vector3.one : Vector3.zero;
    }

    // 複数セルを一度に配置する関数
    void SetCells(Vector3Int[] rotatedPattern, bool alive = true)
    {
        foreach (var cell in rotatedPattern)
        {
            SetCell(cell.x, cell.y, cell.z, alive);
        }
    }

    // グライダーを配置する関数（y=y'とy=y'+1平面に配置）
    void PlaceGlider(int centerX, int centerY, int centerZ, int direction=0)
    {
        Vector3Int basePos = new Vector3Int(centerX, centerY, centerZ);
        
        // 3Dグライダーパターン（xとz正方向に移動）
        // y=y'平面のパターン
        Vector3Int[] basePattern = new Vector3Int[]
        {
            // y=0平面（基準平面）
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(2, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 2),
            
            // y=1平面
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(2, 1, 0),
            new Vector3Int(0, 1, 1),
            new Vector3Int(1, 1, 2)
        };
 

        // 方向に応じてパターンを変換
        Vector3Int[] rotatedPattern = new Vector3Int[basePattern.Length+1];
        rotatedPattern[0] = RotatePattern(basePos, Vector3Int.zero, direction);
        for (int i = 0; i < basePattern.Length; i++)
        {
            rotatedPattern[i+1] = RotatePattern(basePos, basePattern[i], direction);
        }
        
        SetCells(rotatedPattern);
    }

    Vector3Int RotatePattern(Vector3Int basePos, Vector3Int original, int direction)
    {
        int x = basePos.x + original.x;
        int y = basePos.y + original.y;  
        int z = basePos.z + original.z;

        // 軸の座標
        int centerX = basePos.x - 1;
        int centerZ = basePos.z - 1;
        
        // 軸からの相対位置にする
        int relativeX = x - centerX;
        int relativeZ = z - centerZ;
        
        int rotatedX, rotatedZ;
        switch (direction)
        {
            case 0: 
                rotatedX = relativeX;
                rotatedZ = relativeZ;
                break;
            case 1: 
                rotatedX = relativeZ;
                rotatedZ = -relativeX;
                break;
            case 2: 
                rotatedX = -relativeX;
                rotatedZ = -relativeZ;
                break;
            case 3: 
                rotatedX = -relativeZ;
                rotatedZ = relativeX;
                break;
            default:
                rotatedX = relativeX;
                rotatedZ = relativeZ;
                break;
        }
        
        int finalX = centerX + rotatedX;
        int finalZ = centerZ + rotatedZ;
        
        // 絶対座標で返す
        return new Vector3Int(finalX, y, finalZ);
    }
    
}
