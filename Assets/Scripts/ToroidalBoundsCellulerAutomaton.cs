using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToroidalBoundsCellulerAutomaton : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    [SerializeField] int width = 30;
    [SerializeField] int height = 30;
    [SerializeField] int depth = 30;
    [SerializeField] float stepInterval = 0.3f;
    [SerializeField] float animationDuration = 0.1f;

    Dictionary<Vector3Int, GameObject> grid = new Dictionary<Vector3Int, GameObject>();
    bool[,,] currentState;
    bool[,,] nextState;

    // 境界セルのみの隣接インデックス（メモリ効率向上）
    Dictionary<Vector3Int, Vector3Int[]> boundaryNeighbors = new Dictionary<Vector3Int, Vector3Int[]>();
    
    // 基本的な26隣接オフセット（内部セル用）
    static readonly Vector3Int[] neighborOffsets = new Vector3Int[]
    {
        new Vector3Int(-1,-1,-1), new Vector3Int(-1,-1,0), new Vector3Int(-1,-1,1),
        new Vector3Int(-1,0,-1),  new Vector3Int(-1,0,0),  new Vector3Int(-1,0,1),
        new Vector3Int(-1,1,-1),  new Vector3Int(-1,1,0),  new Vector3Int(-1,1,1),
        new Vector3Int(0,-1,-1),  new Vector3Int(0,-1,0),  new Vector3Int(0,-1,1),
        new Vector3Int(0,0,-1),                            new Vector3Int(0,0,1),
        new Vector3Int(0,1,-1),   new Vector3Int(0,1,0),   new Vector3Int(0,1,1),
        new Vector3Int(1,-1,-1),  new Vector3Int(1,-1,0),  new Vector3Int(1,-1,1),
        new Vector3Int(1,0,-1),   new Vector3Int(1,0,0),   new Vector3Int(1,0,1),
        new Vector3Int(1,1,-1),   new Vector3Int(1,1,0),   new Vector3Int(1,1,1)
    };


    void Start()
    {
        PrecomputeBoundaryNeighbors();
        InitializeGrid();
        StartCoroutine(StepRoutine());
    }

    void PrecomputeBoundaryNeighbors()
    {
        // 境界セルのみ事前計算（大幅なメモリ削減）
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    // 境界セルかどうかを判定
                    bool isBoundary = (x == 0 || x == width - 1 || 
                                     y == 0 || y == height - 1 || 
                                     z == 0 || z == depth - 1);
                    
                    if (isBoundary)
                    {
                        List<Vector3Int> neighbors = new List<Vector3Int>();
                        
                        for (int i = 0; i < neighborOffsets.Length; i++)
                        {
                            Vector3Int offset = neighborOffsets[i];
                            int nx = (x + offset.x + width) % width;
                            int ny = (y + offset.y + height) % height;
                            int nz = (z + offset.z + depth) % depth;
                            
                            neighbors.Add(new Vector3Int(nx, ny, nz));
                        }
                        
                        boundaryNeighbors[new Vector3Int(x, y, z)] = neighbors.ToArray();
                    }
                }
            }
        }
    }

    void InitializeGrid()
    {
        currentState = new bool[width, height, depth];
        nextState = new bool[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, this.transform);
                    Vector3Int gridPos = new Vector3Int(x, y, z);
                    grid[gridPos] = cube;

                    // 初期状態：ランダムに生死決定
                    // bool alive = Random.value > 0.8f; // 20%の確率で生きる
                    bool alive = false; // 初期は全て死んでいる
                    currentState[x, y, z] = alive;

                    cube.SetActive(alive);
                    cube.transform.localScale = alive ? Vector3.one : Vector3.zero;
                }
            }
        }
        PlaceGlider(1, 15, 15, 3);
        PlaceGlider(7, 5, 15, 2);
        PlaceGlider(21, 19, 5, 1);
        PlaceGlider(11, 6, 5, 5);
    }

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
    void SetCells(Vector3Int basePos, Vector3Int[] pattern, bool alive = true)
    {
        foreach (var offset in pattern)
        {
            SetCell(basePos.x + offset.x, basePos.y + offset.y, basePos.z + offset.z, alive);
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
        Vector3Int[] rotatedPattern = new Vector3Int[basePattern.Length];
        for (int i = 0; i < basePattern.Length; i++)
        {
            rotatedPattern[i] = RotatePattern(basePattern[i], direction);
        }
        
        SetCells(basePos, rotatedPattern);
    }

    Vector3Int RotatePattern(Vector3Int original, int direction)
    {
        int x = original.x;
        int y = original.y;
        int z = original.z;
        
        switch (direction)
        {
            case 0: // +X方向（デフォルト）
                return new Vector3Int(x, y, z);
                
            case 1: // -X方向（X軸反転）
                return new Vector3Int(-x, y, z);
                
            case 2: // +Y方向（Y軸周りに90度回転: X→Z, Z→-X）
                return new Vector3Int(-z, y, x);
                
            case 3: // -Y方向（Y軸周りに-90度回転: X→-Z, Z→X）
                return new Vector3Int(z, y, -x);
                
            case 4: // +Z方向（Z軸周りに90度回転: X→Y, Y→-X）
                return new Vector3Int(-y, x, z);
                
            case 5: // -Z方向（Z軸周りに-90度回転: X→-Y, Y→X）
                return new Vector3Int(y, -x, z);
                
            default:
                return original;
        }
    }

    

    IEnumerator StepRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(stepInterval);
            Step();
        }
    }

    void Step()
    {
        // 次の世代の状態を計算
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    int aliveNeighbors = CountAliveNeighbors(x, y, z);
                    bool isAlive = currentState[x, y, z];

                    if (isAlive)
                    {
                        // 生存ルール：5〜7個の生きた近傍があれば生き残る
                        nextState[x, y, z] = (aliveNeighbors >= 5 && aliveNeighbors <= 7);
                    }
                    else
                    {
                        // 誕生ルール：6〜8個の生きた近傍があれば誕生
                        nextState[x, y, z] = (aliveNeighbors >= 6 && aliveNeighbors <= 6);
                    }
                }
            }
        }

        // グリッド更新（アニメーション付き）
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    bool wasAlive = currentState[x, y, z];
                    bool willBeAlive = nextState[x, y, z];

                    if (wasAlive != willBeAlive)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        GameObject cube = grid[pos];

                        if (willBeAlive)
                        {
                            // 誕生アニメーション
                            cube.SetActive(true);
                            StartCoroutine(AnimateScale(cube, Vector3.zero, Vector3.one));
                        }
                        else
                        {
                            // 消滅アニメーション
                            StartCoroutine(AnimateScale(cube, Vector3.one, Vector3.zero, () => cube.SetActive(false)));
                        }
                    }

                    currentState[x, y, z] = willBeAlive;
                }
            }
        }
    }

    int CountAliveNeighbors(int x, int y, int z)
    {
        int count = 0;
        
        // 境界セルかどうかを判定
        bool isBoundary = (x == 0 || x == width - 1 || 
                         y == 0 || y == height - 1 || 
                         z == 0 || z == depth - 1);
        
        if (isBoundary)
        {
            // 境界セル：事前計算されたインデックスを使用
            Vector3Int[] neighbors = boundaryNeighbors[new Vector3Int(x, y, z)];
            for (int i = 0; i < neighbors.Length; i++)
            {
                Vector3Int neighbor = neighbors[i];
                if (currentState[neighbor.x, neighbor.y, neighbor.z])
                {
                    count++;
                }
            }
        }
        else
        {
            // 内部セル：単純なオフセット計算（キャッシュ効率良好）
            for (int i = 0; i < neighborOffsets.Length; i++)
            {
                Vector3Int offset = neighborOffsets[i];
                if (currentState[x + offset.x, y + offset.y, z + offset.z])
                {
                    count++;
                }
            }
        }
        
        return count;
    }

    IEnumerator AnimateScale(GameObject obj, Vector3 from, Vector3 to, System.Action onComplete = null)
    {
        float time = 0f;
        obj.transform.localScale = from;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            obj.transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        obj.transform.localScale = to;

        onComplete?.Invoke();
    }


    
}