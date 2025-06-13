using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ToroidalBoundsCellulerAutomaton : MonoBehaviour
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
        // 
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

        PlaceGlider(15, 0, 15, 2);
        PlaceGlider(15, 5, 15, 3);
        PlaceGlider(15, 10, 15, 0);
        PlaceGlider(15, 15, 15, 1);
    }

    // Star
    IEnumerator StepRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(stepInterval);
            Step();
        }
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